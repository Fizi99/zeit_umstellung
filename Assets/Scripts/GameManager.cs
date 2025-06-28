using System.Collections.Generic;
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

    public GameState gameState;

    [Header("Managed Components")]
    [Space(10)]
    [SerializeField] public BusTimeScraper busTimeScraper;
    [SerializeField] public StreetViewMap streetViewMapGetter;
    [SerializeField] public TMP_InputField busSearchInputField;
    [SerializeField] public UIManager uiManager;
    [SerializeField] public GPSTracker gpsTracker;
    [SerializeField] public WaveSpawner waveSpawner;
    [SerializeField] public RouteManager routeManager;
    [SerializeField] public Player player;
    [SerializeField] public buildManager buildManager;
    [SerializeField] public GameObject floatingTextPrefab;
    [SerializeField] public GameObject mainCamera;

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

    void Start()
    {
        this.gameState = GameState.MAINMENU;
        InvokeRepeating("UpdateBusInformation", 1, 5);
        //LocationService location = new LocationService();
        //location.Start();
        //Debug.Log(location.status);
    }

    private void Update()
    {
        
    }

    public void TriggerScreenEffect()
    {
        this.mainCamera.GetComponent<PlayerHitEffect>().TriggerEffect();
    }

    public void ChangeGameState(GameState gameState)
    {
        this.gameState = gameState;
        if(gameState == GameState.LEVELEND)
        {
            ClearScene();
            this.player.SaveUhranium();
        }
        this.uiManager.UpdateUI();
        this.player.ResetUhraniumGain();
    }

    // Remove all enemies and turrets from scene
    public void ClearScene()
    {
        // Destroy all enemies
        List<Transform> enemies = new List<Transform>();
        for(int i = 0; i < enemyContainer.transform.childCount; i++)
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

    public void SpawnFloatingText(Vector3 pos, string text, Color color)
    {
        GameObject floatingText = Instantiate(this.floatingTextPrefab);
        floatingText.transform.position = pos;
        floatingText.GetComponent<FloatingText>().SetFloatingText(text, color);
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

    public double GetPlayerLat()
    {
        return this.player.lat;
    }
    public double GetPlayerLon()
    {
        return this.player.lon;
    }

    public double GetDistanceThreshhold()
    {
        return this.gpsTracker.GetDistanceThreshhold();
    }

    //TODO: aufräumen
    public Vector3 GetClosestPointToBusStop(Vector3 p, Vector3 a, Vector3 b)
    {
        if(this.busStopGO == null)
        {
            return this.routeManager.GetClosestPointOnLine(Vector3.zero, a, b);
        }
        return this.routeManager.GetClosestPointOnLine(this.busStopGO.transform.position, a, b);

    }

    public void UpdateStreetsWithLastWaypoint()
    {

    }


}
