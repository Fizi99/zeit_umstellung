using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Controlled components")]

    [SerializeField] private TMP_InputField busSearchInputField;
    [SerializeField] private List<TMP_Text> countdown;
    [SerializeField] private GameObject scrollerContent;
    [SerializeField] private GameObject busSelectorBtnPrefab;
    [SerializeField] private TMP_Text zeitsandText;
    [SerializeField] private TMP_Text uhraniumText;
    [SerializeField] private TMP_Text lvlFinishedText;
    [SerializeField] private GameObject mainCamera;
    [SerializeField] private Toggle screenShakeToggle;
    [SerializeField] private Toggle vignetteToggle;
    [SerializeField] private Button startLevelButton;
    [SerializeField] private GameObject visualizeLoadoutParent; // referenziert "VisualizeLoadout"
    [SerializeField] private List<Sprite> turretSprites;
    [SerializeField] private GameObject zeitsandContainer;

    [Space(10)]
    [Header("Panel for navigation")]
    [SerializeField] private GameObject lvlSelectionPanel;
    [SerializeField] private GameObject lvlPlayingPanel;
    [SerializeField] private GameObject startMenuPanel;
    [SerializeField] private GameObject upgradingMenuPanel;
    [SerializeField] private GameObject lvlEndPanel;
    [SerializeField] private GameObject settingsPanel;

    [SerializeField] private GameObject distanceToStopText;

    [SerializeField] private GameObject debugText;

    private GameManager gameManager;
    private List<Bus> busses;
    private List<GameObject> busSelectionBtns = new List<GameObject>();
    private bool busInfoUpdated = false;
    private double distanceToStop = 0;

    private Bus selectedBus;

    private Dictionary<TurretType, Sprite> turretTypeToSprite;
    private List<Image> loadoutImages = new List<Image>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        initLoadout();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateSelectedBus();
        UpdateCountdown();
        CheckForBusInfoUpdate();
        UpdateLoadoutVisualization();

        if (busInfoUpdated)
        {
            GenerateBusSelection();
        }

        if(this.gameManager.gameState == GameState.LEVELPLAYING)
        {
            UpdateUhraniumText();
            UpdateZeitsandText();
        }

        if (this.gameManager.player.zeitsand >= this.gameManager.player.maxZeitsand)
        {
            SetZeitsandShake(true);
        } else
        {
            SetZeitsandShake(false);
        }
    }

    public void ShowDebug(string msg)
    {
        this.debugText.GetComponent<TMP_Text>().text = msg;
        Invoke("HideDebug", 5);
    }

    private void HideDebug()
    {
        this.debugText.GetComponent<TMP_Text>().text = "";
    }

    public void ToggleScreenShake()
    {
        this.mainCamera.GetComponent<PlayerHitEffect>().SetToggleScreenshake(this.screenShakeToggle.isOn);
    }

    public void ToggleVignette()
    {
        this.mainCamera.GetComponent<PlayerHitEffect>().SetToggleVignette(this.vignetteToggle.isOn);
    }

    // check if bus information got updated for example to update bus selection buttons or countdown. use value later
    private void CheckForBusInfoUpdate()
    {
        if(this.gameManager.busses != this.busses)
        {
            this.busInfoUpdated = true;
        }
        else
        {
            this.busInfoUpdated = false;
        }

        this.busses = this.gameManager.busses;
    }

    // check if a bus was selected
    private void UpdateSelectedBus()
    {
        if(this.gameManager.selectedBus != this.selectedBus)
        {
            this.selectedBus = this.gameManager.selectedBus;

            // Button aktivieren oder deaktivieren
            startLevelButton.interactable = (this.selectedBus != null);
        }
    }

    // Update countdown text
    private void UpdateCountdown()
    {
        if (this.selectedBus == null)
        {
            return;
        }
        string text = "";
        TimeSpan timeSpan = (System.DateTimeOffset.FromUnixTimeSeconds(this.selectedBus.realtime).LocalDateTime - System.DateTime.Now);
        // handle countdown if bus already departed
        if(timeSpan.Seconds < 0)
        {
            // if player is playing and bus departs, finish level, else dont allow levelstart
            if(this.gameManager.gameState == GameState.LEVELPLAYING)
            {
                text = "Bus departed! Level finished!";
                this.gameManager.ChangeGameState(GameState.LEVELEND);
            }
            //else
            //{
                //text = "Bus already departed. pick another";
            //}
            //foreach(TMP_Text countdownDisplay in this.countdown)
            //    countdownDisplay.fontSize = 12;
        }
        else
        {
            text = $"Verbleibend: {timeSpan:hh':'mm':'ss}";
            //foreach (TMP_Text countdownDisplay in this.countdown)
            //    countdownDisplay.fontSize = 36;
        }

        // show countdown
        foreach (TMP_Text countdownDisplay in this.countdown)
            countdownDisplay.text = text;

    }

    private void UpdateUhraniumText()
    {
        this.uhraniumText.text = "Gesammelt: "+(int)this.gameManager.player.uhranium;
    }

    private void UpdateZeitsandText()
    {
        this.zeitsandText.text = "" + (int)this.gameManager.player.zeitsand;
    }

    private void UpdateLvlFinishedText()
    {
        this.lvlFinishedText.text = "Congratulations!\nLevel finished, your bus is arriving!\n+" + this.gameManager.player.uhraniumGain + " Uhranium!\ntotal: " + this.gameManager.player.savedUhranium;
    }

    // update navigation depending on gamestate
    public void UpdateUI()
    {
        switch (this.gameManager.gameState)
        {
            case GameState.LEVELSELECTION:
                this.lvlSelectionPanel.SetActive(true);
                this.lvlPlayingPanel.SetActive(false);
                this.startMenuPanel.SetActive(false);
                this.upgradingMenuPanel.SetActive(false);
                this.lvlEndPanel.SetActive(false);
                this.settingsPanel.SetActive(false);
                break;
            case GameState.LEVELPLAYING:
                this.lvlSelectionPanel.SetActive(false);
                this.lvlPlayingPanel.SetActive(true);
                this.startMenuPanel.SetActive(false);
                this.upgradingMenuPanel.SetActive(false);
                this.lvlEndPanel.SetActive(false);
                this.settingsPanel.SetActive(false);
                break;
            case GameState.UPGRADING:
                this.lvlSelectionPanel.SetActive(false);
                this.lvlPlayingPanel.SetActive(false);
                this.startMenuPanel.SetActive(false);
                this.upgradingMenuPanel.SetActive(true);
                this.lvlEndPanel.SetActive(false);
                this.settingsPanel.SetActive(false);
                break;
            case GameState.MAINMENU:
                this.lvlSelectionPanel.SetActive(false);
                this.lvlPlayingPanel.SetActive(false);
                this.startMenuPanel.SetActive(true);
                this.upgradingMenuPanel.SetActive(false);
                this.lvlEndPanel.SetActive(false);
                this.settingsPanel.SetActive(false);
                break;
            case GameState.LEVELEND:
                this.lvlSelectionPanel.SetActive(false);
                this.lvlPlayingPanel.SetActive(false);
                this.startMenuPanel.SetActive(false);
                this.upgradingMenuPanel.SetActive(false);
                this.lvlEndPanel.SetActive(true);
                this.settingsPanel.SetActive(false);
                break;
            case GameState.SETTINGS:
                this.lvlSelectionPanel.SetActive(false);
                this.lvlPlayingPanel.SetActive(false);
                this.startMenuPanel.SetActive(false);
                this.upgradingMenuPanel.SetActive(false);
                this.lvlEndPanel.SetActive(false);
                this.settingsPanel.SetActive(true);
                break;
            default:
                break;
        }
        UpdateComponentsOnGameStateChange();
    }

    // update single components that need to be updated on specific gamestate change
    private void UpdateComponentsOnGameStateChange()
    {
        switch (this.gameManager.gameState)
        {
            case GameState.LEVELSELECTION:
                GenerateBusSelection();
                break;
            case GameState.LEVELPLAYING:
                break;
            case GameState.UPGRADING:
                break;
            case GameState.MAINMENU:
                this.selectedBus = null;
                this.gameManager.selectedBus = null;
                this.startLevelButton.interactable = false; // Standard state for the START btn
                break;
            case GameState.SETTINGS:
                break;
            case GameState.LEVELEND:
                UpdateLvlFinishedText();
                break;
            default:
                break;
        }
    }

    public void StartLevel()
    {
        // dont want to start level, if selected bus already departed or no bus is selected or player is too far away from busstop
        if(this.selectedBus == null)
        {
            return;
        }
        TimeSpan timeSpan = (System.DateTimeOffset.FromUnixTimeSeconds(this.selectedBus.realtime).LocalDateTime - System.DateTime.Now);
        if (timeSpan.Seconds < 0)
        {
            return;
        }

        // check if bus stop is near player. comment for testing later remove comment
        /*if (this.distanceToStop > this.gameManager.GetDistanceThreshhold())
        {
            return;
        }*/
            // empty bus selection list
            foreach (GameObject busSelectionBtn in this.busSelectionBtns)
        {
            Destroy(busSelectionBtn);
        }

        // Update UI
        this.gameManager.ChangeGameState(GameState.LEVELPLAYING);
    }

    public void NavigateToForge()
    {
        // Update UI
        this.gameManager.ChangeGameState(GameState.UPGRADING);
    }

    public void NavigateToLevelSelection()
    {
        // Update UI
        this.gameManager.ChangeGameState(GameState.LEVELSELECTION);
    }

    public void NavigateToSettings()
    {
        // Update UI
        this.gameManager.ChangeGameState(GameState.SETTINGS);
    }

    public void NavigateToMainMenu()
    {
        // Update UI
        this.gameManager.ChangeGameState(GameState.MAINMENU);
    }

    public void NavigateToEndLvl()
    {
        // when level is ended, regenerate bus selection: TODO: LATER CHANGE SO SHOWN UI DEPENDS ON GAMESTATE
        //GenerateBusSelection();

        // Update UI
        this.gameManager.ChangeGameState(GameState.LEVELEND);

    }

    // call, when busstop is searched in searchfield
    public void BusSearchInputFieldChanged()
    {
        this.gameManager.SearchBusStop(this.busSearchInputField.text);
    }

    private void GenerateBusSelection()
    {
        UpdateDistanceToStopText();

        // empty bus selection list
        foreach (GameObject busSelectionBtn in this.busSelectionBtns)
        {
            Destroy(busSelectionBtn);
        }
        // add new bus selection buttons

        List<Bus> newList = new List<Bus>(this.gameManager.busses.Count);
        this.gameManager.busses.ForEach((item) =>
        {
            newList.Add(item);
        });

        for (int i = 0; i < newList.Count; i++)
        {
            // create new selection button and set grid as parent
            GameObject btn = GameObject.Instantiate(this.busSelectorBtnPrefab);
            btn.transform.SetParent(this.scrollerContent.transform, false);
            btn.GetComponent<BusSelectorBtn>().bus = newList[i];
            
            // Display bus line infos
            var bus = newList[i];
            string timeStr = $"{System.DateTimeOffset.FromUnixTimeSeconds(bus.time).LocalDateTime.TimeOfDay:hh\\:mm}";
            btn.transform.Find("Route").Find("RectBlack").Find("BusLabel").GetComponent<TMP_Text>().text = "Bus " + bus.line;
            btn.transform.Find("Route").Find("RouteLabel").GetComponent<TMP_Text>().text = bus.headsign;
            btn.transform.Find("TimeLabel").GetComponent<TMP_Text>().text = $"{timeStr} (+ {System.DateTimeOffset.FromUnixTimeSeconds(bus.realtime - bus.time).Minute})";

            // Display the play time for each bus line
            TimeSpan playTime = (System.DateTimeOffset.FromUnixTimeSeconds(bus.realtime).LocalDateTime - System.DateTime.Now);
            int playTimeInMinutes = (int) playTime.TotalMinutes;
            btn.transform.Find("TimeLabel").GetComponent<TMP_Text>().text += $"\n{playTimeInMinutes} Min Spielzeit";

            // change color of button of selected bus
            if (this.gameManager.selectedBus != null && this.gameManager.busses[i].line == this.gameManager.selectedBus.line && this.gameManager.busses[i].time == this.gameManager.selectedBus.time)
            {
                // Set 'selected' color
                /*Button button = btn.GetComponent<Button>();
                ColorBlock cb = button.colors;
                cb.selectedColor = new Color32(39, 135, 195, 255);
                button.colors = cb;*/

                // Set the state to 'selected'
                btn.GetComponent<Button>().Select();
            }
            this.busSelectionBtns.Add(btn);
        }
    }

    private void UpdateDistanceToStopText()
    {
        //double pLat = this.gameManager.GetPlayerLat();
        //double pLon = this.gameManager.GetPlayerLon();
        double pLat = 53.837951;
        double pLon = 10.700337;
        double sLat = this.gameManager.busStop.lat;
        double sLon = this.gameManager.busStop.lon;
        this.distanceToStop = this.gameManager.CalcDistanceBetweenCordsInM(pLat, pLon, sLat, sLon);
            
        if (this.distanceToStop > this.gameManager.GetDistanceThreshhold())
        {
            distanceToStopText.GetComponent<TMP_Text>().text = this.distanceToStop + "m, please get closer to the bus stop.";
        }
        else
        {
            distanceToStopText.GetComponent<TMP_Text>().text = this.distanceToStop + "m";
        }
    }

    private void initLoadout()
    {
        // Mapping erstellen
        turretTypeToSprite = new Dictionary<TurretType, Sprite>();
        TurretType[] types = (TurretType[]) System.Enum.GetValues(typeof(TurretType));

        for (int i = 0; i < types.Length && i < turretSprites.Count; i++)
            turretTypeToSprite[types[i]] = turretSprites[i];

        // Kinder-Images automatisch finden
        foreach (Transform child in visualizeLoadoutParent.transform)
        {
            Image img = child.GetComponent<Image>();
            if (img != null)
                loadoutImages.Add(img);
        }

        UpdateLoadoutVisualization(); // Initial anzeigen
    }

    private void UpdateLoadoutVisualization()
    {
        List<TurretType> loadout = this.gameManager.player.chosenLoadout;

        for (int i = 0; i < loadoutImages.Count && i < loadout.Count; i++)
        {
            if (turretTypeToSprite.TryGetValue(loadout[i], out Sprite sprite))
            {
                loadoutImages[i].sprite = sprite;
            }
        }
    }

    private Coroutine shakeCoroutine;
    private bool isShaking = false;
    private Quaternion originalRotation;

    public void SetZeitsandShake(bool shouldShake)
    {
        if (shouldShake && !isShaking)
        {
            originalRotation = zeitsandContainer.transform.localRotation;
            shakeCoroutine = StartCoroutine(ShakeRotationRoutine());
            isShaking = true;
        }
        else if (!shouldShake && isShaking)
        {
            StopCoroutine(shakeCoroutine);
            zeitsandContainer.transform.localRotation = originalRotation;
            isShaking = false;
        }
    }

    IEnumerator ShakeRotationRoutine()
    {
        float angleMagnitude = 2f; // max degrees left/right
        float shakeFrequency = 0.05f;

        while (true)
        {
            float zAngle = UnityEngine.Random.Range(-angleMagnitude, angleMagnitude);
            zeitsandContainer.transform.localRotation = originalRotation * Quaternion.Euler(0f, 0f, zAngle);

            yield return new WaitForSeconds(shakeFrequency);
        }
    }
}
