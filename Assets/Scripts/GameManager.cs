using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public List<Bus> busses;
    [HideInInspector]
    public List<GameObject> streets;
    public List<List<GameObject>> routes = new List<List<GameObject>>();
    public Bus selectedBus;
    public BusStop busStop;
    [HideInInspector]
    public GameObject busStopGO = null;
    public Dictionary<long, Vector3> nodeLocationDictionary;

    private readonly string[] epochs = { "pharaohs", "medieval", "prehistoric" };

    public GameState gameState;

    [Header("Managed Components")]
    [Space(10)]
    [SerializeField] public BusTimeScraper busTimeScraper;
    [SerializeField] public StreetViewMap streetViewMapGetter;
    [SerializeField] public TMP_InputField busSearchInputField;
    [SerializeField] public UIManager uiManager;
    [SerializeField] public AudioManager audioManager;
    [SerializeField] public GPSTracker gpsTracker;
    [SerializeField] public WaveSpawner waveSpawner;
    [SerializeField] public RouteManager routeManager;
    [SerializeField] public TutorialManager tutorialManager;
    [SerializeField] public DifficultyManager difficultyManager;
    [SerializeField] public Player player;
    [SerializeField] public buildManager buildManager;
    [SerializeField] public GameObject floatingTextPrefab;
    [SerializeField] public GameObject floatingEpochTitlePrefab;
    [SerializeField] public GameObject mainCamera;
    [SerializeField] public GameObject highscoreTracker;
    [SerializeField] public GameObject backGroundPlane;
    [SerializeField] public GameObject placeableZoneManager;
    [SerializeField] public GameObject trophy;

    public EpochChooser epochChooser = new EpochChooser();

    [Header("Container")]
    [Space(10)]
    [SerializeField] public GameObject enemyContainer;
    [SerializeField] public GameObject turretContainer;

    [SerializeField] public GameObject artilleryPrefab;
    [SerializeField] public GameObject dronePrefab;
    [SerializeField] public GameObject freezePrefab;
    [SerializeField] public GameObject bombPrefab;
    [SerializeField] public GameObject laserPrefab;

    [Space(10)]
    [Header("Content for balancing export")]
    [Space(10)]
    [SerializeField] public List<GameObject> enemyPrefabs;
    [SerializeField] public List<GameObject> turretPrefabs;
    [SerializeField] private Sprite[] allEpochSpecificSprites;

    [SerializeField] public GameObject bombButton;
    [SerializeField] public GameObject freezeButton;


    [SerializeField] public int enemiesSinceLastDrop = 0;

    public Epoch currentEpoch;

    private bool firstStart = true;

    void Awake()
    {
        // Reset all persistently saved data if first time started the app on a build
        if (!Application.isEditor)
        {
            if (SaveManager.IsFreshInstall())
            {
                PlayerPrefs.DeleteAll();
                SaveManager.MarkFreshInstallHandled();
                SaveManager.SaveFirstTimePlaying(true);
                PlayerPrefs.Save();
            }
        }

        // List all the data we want to load on app start
        SaveManager.LoadUhraniumHighscore();
    }

    void Start()
    {
        this.gameState = GameState.MAINMENU;
        this.uiManager.ApplySavedSettingsToUI();
        this.uiManager.ApplySettingsToSystem();
        CheckForTrophyDisplay();
        InvokeRepeating("UpdateBusInformation", 1, 5);
        // get busstop location based

        //LocationService location = new LocationService();
        //location.Start();
        //Debug.Log(location.status);
    }

    private void Update()
    {
    }

    public void CenterCameraToBusstop()
    {

    }

    public void TriggerScreenEffect()
    {
        this.mainCamera.GetComponent<PlayerHitEffect>().TriggerEffect();
    }

    public void ChangeGameState(GameState gameState)
    {
        this.gameState = gameState;
        this.audioManager.PlaySfx(this.audioManager.soundLibrary.sfxButtonTapped);

        if (gameState == GameState.MAINMENU)
        {
            // If the player has all turrets that are listed inside TurretType (except DRONE), show the trophy
            CheckForTrophyDisplay();
            this.player.ResetUhraniumGain();
        }

        if (gameState == GameState.LEVELSELECTION)
        {

            if (this.firstStart)
            {
                this.uiManager.SearchClosestStopBtnPressed();
                this.firstStart = false;
            }
        }

        if (gameState == GameState.LEVELPLAYING)
        {
            // Epoch changes and initialisations only if game is not continued from pause
            if (!this.uiManager.gameStartedFromPause)
            {
                this.currentEpoch = this.epochChooser.GetNextEpoch();
                this.backGroundPlane.GetComponent<BackgroundTilePainter>().GenerateBackground(currentEpoch);
                this.routeManager.UpdateStreetMaterial();
                highscoreTracker.GetComponent<HighscoreTracker>().SetHighscoreDisplayVisibility(false);
                this.tutorialManager.PlayTutorial();
                this.audioManager.PlaySfx(this.audioManager.soundLibrary.sfxLevelStarted);
            }

            buildManager.SetBuyButtons();
        }

        if (gameState == GameState.LEVELEND)
        {
            this.audioManager.PlaySfx(this.audioManager.soundLibrary.sfxLevelFinished);
            ClearScene();
            this.player.SaveUhranium();
        }

        this.uiManager.UpdateUI();

        if(gameState == GameState.LEVELEND)
        {
            this.player.ResetUhraniumGain();
        }
        //this.player.ResetUhraniumGain();
    }

    // Remove all enemies and turrets from scene
    public void ClearScene()
    {
        // Destroy all enemies
        List<Transform> enemies = new List<Transform>();
        for (int i = 0; i < enemyContainer.transform.childCount; i++)
        {
            enemies.Add(enemyContainer.transform.GetChild(i));
        }
        foreach (Transform child in enemies)
        {
            Destroy(child.gameObject);
        }

        // Destroy all turrets
        List<Transform> turrets = new List<Transform>();
        for (int i = 0; i < turretContainer.transform.childCount; i++)
        {
            turrets.Add(turretContainer.transform.GetChild(i));
        }
        foreach (Transform child in turrets)
        {
            Destroy(child.gameObject);
        }

        // Clean up all the drops lying around
        GameObject[] toDeleteDrops = GameObject.FindGameObjectsWithTag("Drop");
        foreach (GameObject obj in toDeleteDrops)
            Destroy(obj);

        // reset player zeitsand
        this.player.zeitsand = this.player.zeitsandStartValue;
        this.waveSpawner.waveBudget = this.waveSpawner.initialWaveBudget;
    }

    public void UpdateBusStopData(BusStop newData)
    {
        this.busStop = newData;
    }

    public void UpdateBusStopGameObject(GameObject newBusStop)
    {
        this.busStopGO = newBusStop;
    }

    public void CheckForTrophyDisplay()
    {
        var purchasedTurrets = SaveManager.LoadPurchasedTurrets();
        TurretType excludeThisTurret = TurretType.DRONE;
        var allPurchasableTurrets = Enum.GetValues(typeof(TurretType))
                             .Cast<TurretType>()
                             .Where(f => f != excludeThisTurret)
                             .ToHashSet();
        bool showTrophy = allPurchasableTurrets.SetEquals(purchasedTurrets);
        trophy.SetActive(showTrophy);
    }

    public void SpawnFloatingText(Vector3 pos, string text, Color color)
    {
        GameObject floatingText = Instantiate(this.floatingTextPrefab);
        floatingText.transform.position = pos;
        floatingText.GetComponent<FloatingText>().SetFloatingText(text, color);
    }

    public void SpawnEpochText(Vector3 pos, string text, Color color)
    {
        GameObject floatingText = Instantiate(this.floatingEpochTitlePrefab);
        floatingText.transform.position = pos;
        floatingText.GetComponent<FloatingEpochTitle>().SetFloatingText(text, color);
    }

    // generate new streets. Set them so other scripts can access them, get the closest street to the bus stop and add that point as last destination to every street
    public void SetStreets(List<GameObject> streets)
    {
        this.streets = streets;
        ConvertStreetsToRoutes();
    }

    public void ScaleStreetGrid(float scale)
    {
        this.streetViewMapGetter.ScaleStreetGrid(scale);
    }

    public void ConvertStreetsToRoutes()
    {
        // init waves for enemies
        // TODO: put at end, so only eligable routes are used (now every street spawns enemies)
        this.waveSpawner.setAmountOfRoutes(routes.Count);

        this.routeManager.InitiateRouteManagement();

        //this.routeManager.AddLastWaypointToRoutes(this.streets);
    }
    public void SearchBusStop(string busStop)
    {
        this.busTimeScraper.SearchBusStop(busStop);
    }

    public void SearchStreetsAroundCenter(double searchCenterLat, double searchCenterLon)
    {
        this.streetViewMapGetter.SearchStreetsAroundCenter(searchCenterLat, searchCenterLon);
    }

    public void UpdateBusInformation()
    {
        this.busTimeScraper.UpdateBusInformation();
    }

    public double CalcDistanceBetweenCordsInM(double lat1, double lon1, double lat2, double lon2)
    {
        return this.gpsTracker.CalcDistanceBetweenCordsInM(lat1, lon1, lat2, lon2);
    }

    public void SetPlayerCoords(double lat, double lon)
    {
        this.player.SetPlayerCoords(lat, lon);
    }

    public void SearchClosestStopToPLayer()
    {
        this.busTimeScraper.FindClosestBusStop();
    }

    // for testing: hardcoded coords of l�beck zob
    public double GetPlayerLat()
    {
        //return 53.8656862;
        return this.player.lat;
    }
    public double GetPlayerLon()
    {
        //return 10.6704442;
        return this.player.lon;
    }

    public void UpdateBusStopSearchInputFieldInUI(string text)
    {
        this.uiManager.UpdateInputFieldText(text);

    }
    public double GetDistanceThreshhold()
    {
        return this.gpsTracker.GetDistanceThreshhold();
    }

    //TODO: aufr�umen
    public Vector3 GetClosestPointToBusStop(Vector3 p, Vector3 a, Vector3 b)
    {
        if (this.busStopGO == null)
        {
            return this.routeManager.GetClosestPointOnLine(Vector3.zero, a, b);
        }
        return this.routeManager.GetClosestPointOnLine(this.busStopGO.transform.position, a, b);

    }

    public void UpdateStreetsWithLastWaypoint()
    {

    }

    public void ResetProgress()
    {
        Debug.Log("Resetting all progress ...");
        PlayerPrefs.DeleteAll();

        // Set default values
        SaveManager.SaveToggle(SettingOption.AudioMute, false);
        SaveManager.SaveToggle(SettingOption.MusicMute, false);
        SaveManager.SaveToggle(SettingOption.SFXMute, false);
        SaveManager.SaveToggle(SettingOption.ScreenShake, true);
        SaveManager.SaveToggle(SettingOption.Vignette, true);
        SaveManager.SaveToggle(SettingOption.Pulse, true);
        SaveManager.SaveToggle(SettingOption.Vibration, true);
        SaveManager.SaveToggle(SettingOption.ShowTutorialEveryTime, false);

        SaveManager.SaveVolume(SettingOption.AudioVolume, 1.0f);
        SaveManager.SaveVolume(SettingOption.SfxVolume, 0.3f);
        SaveManager.SaveVolume(SettingOption.MusicVolume, 0.3f);


        SaveManager.SavePurchasedTurrets(new List<TurretType>()
            {
                TurretType.STANDARD,
                TurretType.MISSILE,
                TurretType.DRONEBASE,
                TurretType.LASER,
            });
        player.chosenLoadout = new List<TurretType>()
            {

                TurretType.STANDARD,
                TurretType.MISSILE,
                TurretType.DRONEBASE,
                TurretType.LASER,
            };
        if (bombButton != null)
        {
            bombButton.GetComponent<displayTurretCost>().showPosession(false);
        }
        if (freezeButton != null)
        {
            freezeButton.GetComponent<displayTurretCost>().showPosession(false);
        }

        PlayerPrefs.Save();

        uiManager.ApplySavedSettingsToUI();     // synchronizes UI with PlayerPrefs
        uiManager.ApplySettingsToSystem();      // Synchronizes system (tutorial manager, etc.)
        Debug.Log("Highscore is now (after reset): " + PlayerPrefs.GetFloat("Highscore", 0f));
    }

    public void QuitGame()
    {
        // Funktioniert auf mobilen Ger�ten
        Application.Quit();

        // Optional f�r Editor-Tests
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
