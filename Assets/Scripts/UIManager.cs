using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.EventSystems;

public class UIManager : MonoBehaviour
{
    [Header("Controlled components")]
    [SerializeField] private TMP_InputField busSearchInputField;
    [SerializeField] private List<TMP_Text> countdown;
    [SerializeField] private GameObject scrollerContent;
    [SerializeField] private GameObject busSelectorBtnPrefab;
    [SerializeField] private TMP_Text zeitsandText;
    [SerializeField] private TMP_Text uhraniumText;
    [SerializeField] private TMP_Text uhraniumTextLoadout;
    [SerializeField] private TMP_Text lvlFinishedText;
    [SerializeField] private GameObject mainCamera;
    [SerializeField] private Toggle screenShakeToggle;
    [SerializeField] private Toggle vignetteToggle;
    [SerializeField] private Toggle pulseOnPlacableZoneToggle;
    [SerializeField] private Toggle audioToggle;
    [SerializeField] private Toggle sfxToggle;
    [SerializeField] private Toggle musicToggle;
    [SerializeField] private Slider audioVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;
    [SerializeField] private Toggle tutorialToggle;
    [SerializeField] private Toggle vibrationToggle;
    [SerializeField] private GameObject startLevelButton;
    [SerializeField] private GameObject visualizeLoadoutParent;
    [SerializeField] private List<Sprite> turretSprites;
    [SerializeField] private GameObject zeitsandContainer;
    [SerializeField] private GameObject locationTrackedIcon;
    [SerializeField] private GameObject loadingBusstopIcon;
    [SerializeField] private GameObject loadingStreetsIcon;
    [SerializeField] private TMP_Text uhraniumTextPausePanel;
    [SerializeField] private TMP_Text highscoreTextMainMenu;

    [Space(10)]

    [Header("Panel for navigation")]
    [SerializeField] private GameObject lvlSelectionPanel;
    [SerializeField] private GameObject lvlPlayingPanel;
    [SerializeField] private GameObject startMenuPanel;
    [SerializeField] private GameObject upgradingMenuPanel;
    [SerializeField] private GameObject lvlEndPanel;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] public GameObject loadoutPanel;
    [SerializeField] public GameObject pausePanel;

    [SerializeField] public TMPro.TMP_Dropdown dropdown;

    [SerializeField] private GameObject distanceToStopText;

    [SerializeField] private GameObject debugText;

    [SerializeField] private GameObject zeitsandDrop;

    private GameManager gameManager;
    private AudioManager audioManager;
    private List<Bus> busses;
    private List<GameObject> busSelectionBtns = new List<GameObject>();
    private bool busInfoUpdated = false;
    private double distanceToStop = 0;

    private Bus selectedBus;

    // For loadout display
    private Dictionary<TurretType, Sprite> turretTypeToSprite;
    private List<Image> loadoutImages = new List<Image>();

    // For zeitsand shake animation
    private Coroutine shakeCoroutine;
    private bool isShaking = false;
    private Quaternion originalRotation;

    //private EpochChooser epochChooser;

    public List<TurretType> shownLoadout = new List<TurretType>();
    public List<List<TurretType>> temporaryLoadouts = new List<List<TurretType>>();
    private bool initializedTemporaryLoadouts = false;

    public Button Arsenal1;
    public int currentLoadoutIndex=1;

    private bool tutorialtoggleSet = false;

    public GameState stateBeforeSettingsVisit;

    private bool toggleLoadingBusstopIcon = false;

    public bool gameStartedFromPause = false;


    void Start()
    {
        this.gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        this.audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();

        ApplySavedSettingsToUI();
        ApplySettingsToSystem();

        initLoadout();

        if (this.gameManager.gameState == GameState.MAINMENU)
        {
            float highscore = SaveManager.LoadUhraniumHighscore();
            this.highscoreTextMainMenu.text = $"Highscore: {Mathf.FloorToInt(highscore)}";
        }
    }

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

        if (this.gameManager.gameState == GameState.LEVELPLAYING)
        {
            UpdateUhraniumText();
            UpdateZeitsandText();

            // Nur Shake, wenn NICHT pausiert
            if (Time.timeScale > 0)
            {
                if (this.gameManager.player.zeitsand >= this.gameManager.player.maxZeitsand)
                {
                    SetZeitsandShake(true);
                }
                else
                {
                    SetZeitsandShake(false);
                }
            }
        }

        if (this.gameManager.gameState == GameState.LEVELSELECTION)
        {
            ShowLoadingBusstopIcon();
            ShowLoadingStreetsIcon();
        }

        if (this.gameManager.player.zeitsand >= this.gameManager.player.maxZeitsand)
        {
            SetZeitsandShake(true);
        }
        else
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

    private void ShowLoadingBusstopIcon()
    {
        if (this.toggleLoadingBusstopIcon)
        {
            this.loadingBusstopIcon.SetActive(true);
            this.scrollerContent.SetActive(false);
        }
        else
        {
            this.loadingBusstopIcon.SetActive(false);
            this.scrollerContent.SetActive(true);
        }
    }

    private void ShowLoadingStreetsIcon()
    {
        if (this.gameManager.streetViewMapGetter.loading)
        {
            this.loadingStreetsIcon.SetActive(true);
            this.startLevelButton.SetActive(false);
        }
        else
        {
            this.loadingStreetsIcon.SetActive(false);
            this.startLevelButton.SetActive(true);
        }
    }

    /// <summary>
    /// SETTINGSMENU
    /// </summary>

    public void ToggleScreenShake()
    {
        SaveManager.SaveToggle(SettingOption.ScreenShake, screenShakeToggle.isOn);
        this.mainCamera.GetComponent<PlayerHitEffect>().SetToggleScreenshake(this.screenShakeToggle.isOn);
    }

    public void ToggleVignette()
    {
        SaveManager.SaveToggle(SettingOption.Vignette, vignetteToggle.isOn);
        this.mainCamera.GetComponent<PlayerHitEffect>().SetToggleVignette(this.vignetteToggle.isOn);
    }

    public void TogglePlaceableZonePulse()
    {
        SaveManager.SaveToggle(SettingOption.Pulse, pulseOnPlacableZoneToggle.isOn);
        this.gameManager.placeableZoneManager.GetComponent<PlaceableZone>().pulseOn = this.pulseOnPlacableZoneToggle.isOn;
    }

    public void ToggleAudioMute(bool isOn)
    {
        SaveManager.SaveToggle(SettingOption.AudioMute, isOn);
        ApplyAudioMuteState();
    }

    public void ToggleMusicMute(bool isOn)
    {
        SaveManager.SaveToggle(SettingOption.MusicMute, isOn);
        ApplyAudioMuteState();
    }

    public void ToggleSfxMute(bool isOn)
    {
        SaveManager.SaveToggle(SettingOption.SFXMute, isOn);
        ApplyAudioMuteState();
    }

    public void ApplyAudioMuteState()
    {
        /*bool isAudioOn = SaveManager.LoadToggle(SettingOption.AudioPlay, true);
        bool isMusicOn = SaveManager.LoadToggle(SettingOption.MusicPlay, true);
        bool isSfxOn = SaveManager.LoadToggle(SettingOption.SFXPlay, true);*/

        bool isAudioMute = !audioToggle.isOn;
        bool isMusicMute = !musicToggle.isOn;
        bool isSfxMute = !sfxToggle.isOn;

        bool shouldPlayMusic = isAudioMute && isMusicMute;
        bool shouldPlaySfx = isAudioMute && isSfxMute;

        audioManager.MusicMute(!shouldPlayMusic);
        audioManager.SfxMute(!shouldPlaySfx);

        // Stelle sicher, dass Musik läuft, wenn sie laufen soll
        if (shouldPlayMusic && !audioManager.musicSource.isPlaying)
        {
            audioManager.musicSource.Play(); // <-- erzwingt Musikstart
            Debug.Log("Force Play()");
        }

        Debug.Log("Music Muted? " + audioManager.musicSource.mute + " | Music Playing? " + audioManager.musicSource.isPlaying);
    }

    // volume slider for all audio, music and sfx. sfx and music volume is multiplied by state of overall audio slider
    public void SliderAudioVolume()
    {
        SaveManager.SaveVolume(SettingOption.AudioVolume, audioVolumeSlider.value);
        SliderMusicVolume();
        SliderSfxVolume();
    }

    public void SliderMusicVolume()
    {
        this.audioManager.MusicVolume(this.musicVolumeSlider.value * this.audioVolumeSlider.value);
        SaveManager.SaveVolume(SettingOption.MusicVolume, musicVolumeSlider.value);
    }

    public void SliderSfxVolume()
    {
        this.audioManager.SfxVolume(this.sfxVolumeSlider.value * this.audioVolumeSlider.value);
        SaveManager.SaveVolume(SettingOption.SfxVolume, sfxVolumeSlider.value);
    }

    public void ToggleTutorial()
    {
        //this.player.playTutorial = this.tutorialToggle.isOn;
        SaveManager.SaveToggleTutorialState(this.tutorialToggle.isOn);
    }

    public void ToggleVibrations()
    {
        if (SaveManager.LoadVibrationEnabled() != this.vibrationToggle)
        {
            SaveManager.SaveVibrationEnabled(this.vibrationToggle);
        }
    }

    public void ApplySavedSettingsToUI()
    {
        screenShakeToggle.isOn = SaveManager.LoadToggle(SettingOption.ScreenShake);
        vignetteToggle.isOn = SaveManager.LoadToggle(SettingOption.Vignette);
        pulseOnPlacableZoneToggle.isOn = SaveManager.LoadToggle(SettingOption.Pulse);
        tutorialToggle.isOn = SaveManager.LoadToggle(SettingOption.ShowTutorialEveryTime, true);
        vibrationToggle.isOn = SaveManager.LoadToggle(SettingOption.Vibration, true);

        audioToggle.isOn = SaveManager.LoadToggle(SettingOption.AudioMute);
        sfxToggle.isOn = SaveManager.LoadToggle(SettingOption.SFXMute);
        musicToggle.isOn = SaveManager.LoadToggle(SettingOption.MusicMute);

        audioVolumeSlider.value = SaveManager.LoadVolume(SettingOption.AudioVolume);
        sfxVolumeSlider.value = SaveManager.LoadVolume(SettingOption.SfxVolume);
        musicVolumeSlider.value = SaveManager.LoadVolume(SettingOption.MusicVolume);
    }

    public void ApplySettingsToSystem()
    {
        ToggleScreenShake();
        ToggleVignette();
        TogglePlaceableZonePulse();
        ToggleTutorial();
        ToggleVibrations();

        audioVolumeSlider.value = SaveManager.LoadVolume(SettingOption.AudioVolume, 1f);
        musicVolumeSlider.value = SaveManager.LoadVolume(SettingOption.MusicVolume, 0.3f);
        sfxVolumeSlider.value = SaveManager.LoadVolume(SettingOption.SfxVolume, 0.3f);

        SliderAudioVolume(); // also applies music/sfx slider internally
        ApplyAudioMuteState();
    }

    // check if bus information got updated for example to update bus selection buttons or countdown. use value later
    private void CheckForBusInfoUpdate()
    {
        if (this.gameManager.busses != this.busses)
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
        if (this.gameManager.selectedBus != this.selectedBus)
        {
            this.selectedBus = this.gameManager.selectedBus;

            // Button aktivieren oder deaktivieren
            startLevelButton.GetComponent<Button>().interactable = (this.selectedBus != null);
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
        if (timeSpan.Seconds < 0)
        {
            // if player is playing and bus departs, finish level, else dont allow levelstart
            if (this.gameManager.gameState == GameState.LEVELPLAYING)
            {
                text = "Bus angekommen! Level beendet!";

                // Check if updating the uhranium highscore is needed
                float finalUhraniumAcquired = SaveManager.LoadUhranium();
                if (SaveManager.LoadUhraniumHighscore() < finalUhraniumAcquired)
                {
                    SaveManager.SaveUhraniumHighscore(finalUhraniumAcquired);
                }

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
        this.uhraniumText.text = "Gesichert: "+((int)this.gameManager.player.uhraniumGain).ToString();
    }

    private void UpdateZeitsandText()
    {
        this.zeitsandText.text = "" + (int)this.gameManager.player.zeitsand;
    }

    private void UpdateLvlFinishedText()
    {
        string uhraniumGain = "<b><size=130%>+" + this.gameManager.player.uhraniumGain + " Uhranium!</size></b>";
        string totalUhranium = "Insgesamt: " + Mathf.FloorToInt(SaveManager.LoadUhranium()) + " Uhranium";

        this.lvlFinishedText.text = "— Spiel erfolgreich beendet —\nDein Bus ist angekommen!\n\n" + uhraniumGain + "\n" + totalUhranium + "\n";
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
                this.loadoutPanel.SetActive(false);
                this.pausePanel.SetActive(false);
                break;
            case GameState.LEVELPLAYING:
                this.lvlSelectionPanel.SetActive(false);
                this.lvlPlayingPanel.SetActive(true);
                this.startMenuPanel.SetActive(false);
                this.upgradingMenuPanel.SetActive(false);
                this.lvlEndPanel.SetActive(false);
                this.settingsPanel.SetActive(false);
                this.loadoutPanel.SetActive(false);
                this.pausePanel.SetActive(false);
                break;
            case GameState.UPGRADING:
                this.lvlSelectionPanel.SetActive(false);
                this.lvlPlayingPanel.SetActive(false);
                this.startMenuPanel.SetActive(false);
                this.upgradingMenuPanel.SetActive(true);
                this.lvlEndPanel.SetActive(false);
                this.settingsPanel.SetActive(false);
                this.loadoutPanel.SetActive(false);
                this.pausePanel.SetActive(false);
                break;
            case GameState.MAINMENU:
                this.lvlSelectionPanel.SetActive(false);
                this.lvlPlayingPanel.SetActive(false);
                this.startMenuPanel.SetActive(true);
                this.upgradingMenuPanel.SetActive(false);
                this.lvlEndPanel.SetActive(false);
                this.settingsPanel.SetActive(false);
                this.loadoutPanel.SetActive(false);
                this.pausePanel.SetActive(false);
                break;
            case GameState.LEVELEND:
                this.lvlSelectionPanel.SetActive(false);
                this.lvlPlayingPanel.SetActive(false);
                this.startMenuPanel.SetActive(false);
                this.upgradingMenuPanel.SetActive(false);
                this.lvlEndPanel.SetActive(true);
                this.settingsPanel.SetActive(false);
                this.loadoutPanel.SetActive(false);
                this.pausePanel.SetActive(false);
                break;
            case GameState.SETTINGS:
                this.lvlSelectionPanel.SetActive(false);
                this.lvlPlayingPanel.SetActive(false);
                this.startMenuPanel.SetActive(false);
                this.upgradingMenuPanel.SetActive(false);
                this.lvlEndPanel.SetActive(false);
                this.settingsPanel.SetActive(true);
                this.loadoutPanel.SetActive(false);
                this.pausePanel.SetActive(false);
                break;
            case GameState.LOADOUTCREATION:
                this.lvlSelectionPanel.SetActive(false);
                this.lvlPlayingPanel.SetActive(false);
                this.startMenuPanel.SetActive(false);
                this.upgradingMenuPanel.SetActive(false);
                this.lvlEndPanel.SetActive(false);
                this.settingsPanel.SetActive(false);
                this.loadoutPanel.SetActive(true);
                this.pausePanel.SetActive(false);
                break;
            case GameState.PAUSING:
                this.lvlSelectionPanel.SetActive(false);
                this.lvlPlayingPanel.SetActive(false);
                this.startMenuPanel.SetActive(false);
                this.upgradingMenuPanel.SetActive(false);
                this.lvlEndPanel.SetActive(false);
                this.settingsPanel.SetActive(false);
                this.loadoutPanel.SetActive(false);
                this.pausePanel.SetActive(true);
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
                //this.gameManager.highscoreTracker.resetTracker();

                if (!this.gameManager.tutorialManager.isActive && !gameStartedFromPause)
                {
                    switch (this.gameManager.currentEpoch)
                    {
                        case Epoch.MEDIEVAL:
                            this.gameManager.SpawnEpochText(new Vector3(0, 2.25f, -1), "Mittelalter", Color.white);
                            break;
                        case Epoch.PHARAOH:
                            this.gameManager.SpawnEpochText(new Vector3(0, 2.25f, -1), "Altes Ägypten", Color.white);

                            break;
                        case Epoch.PREHISTORIC:
                            this.gameManager.SpawnEpochText(new Vector3(0, 2.25f, -1), "Steinzeit", Color.white);
                            break;
                        default:
                            break;
                    }
                }
                break;
            case GameState.UPGRADING:
                break;
            case GameState.MAINMENU:
                this.selectedBus = null;
                this.gameManager.selectedBus = null;
                this.startLevelButton.GetComponent<Button>().interactable = false; // Standard state for the START btn

                float highscore = SaveManager.LoadUhraniumHighscore();
                this.highscoreTextMainMenu.text = $"Highscore: {Mathf.FloorToInt(highscore)}";
                break;
            case GameState.SETTINGS:
                // set toggle for tutorial the first time settingsmenu is opened
                if (!this.tutorialtoggleSet)
                {
                    this.tutorialToggle.isOn = this.gameManager.player.firstTimePlaying;
                    this.tutorialtoggleSet = true;
                }
                break;
            case GameState.LEVELEND:
                UpdateLvlFinishedText();
                break;
            case GameState.PAUSING:
                    this.uhraniumTextPausePanel.text = "Gesichert: " + ((int)this.gameManager.player.uhraniumGain).ToString();     
                FreezeTime();
                break;
            default:
                break;
        }
    }

    public void StartLevel()
    {
        // dont want to start level, if selected bus already departed or no bus is selected or player is too far away from busstop
        if (this.selectedBus == null)
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

        // Flag the upcoming game as a new game start
        gameStartedFromPause = false;

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
        switch (this.gameManager.gameState)
        {
            case GameState.MAINMENU:
                Debug.Log("Kam von Main Menu");
                this.stateBeforeSettingsVisit = GameState.MAINMENU;
                break;
            case GameState.PAUSING:
                Debug.Log("Kam von in-game");
                this.stateBeforeSettingsVisit = GameState.PAUSING;
                break;
            default:
                Debug.Log("Weder noch (Sollte nicht auftreten)");
                this.stateBeforeSettingsVisit = GameState.MAINMENU;
                break;
        }
        this.gameManager.ChangeGameState(GameState.SETTINGS);
    }

    public void NavigateToMainMenu()
    {
        if (this.gameManager.gameState == GameState.PAUSING)
        {
            // TODO?: Safe uhranium (& highscore) if game stopped while in-game
            Debug.Log("Ab ins Menü, davor aber Uhranium speichern");
            UnfreezeTime();
            this.gameManager.ClearScene();
            this.gameManager.player.ResetUhranium();
        }

        // Update UI
        this.gameManager.ChangeGameState(GameState.MAINMENU);
    }

    public void NavigateToEndLvl()
    {
        // when level is ended, regenerate bus selection: TODO: LATER CHANGE SO SHOWN UI DEPENDS ON GAMESTATE
        //GenerateBusSelection()
        // reset uhranium so only saved uhranium is added to account. prevents exploit of ending game before a big wave hits without being able to save current uhranium amount
        this.gameManager.player.ResetUhranium();

        // Update UI
        this.gameManager.ChangeGameState(GameState.LEVELEND);

    }

    public void NavigateToLoadout()
    {
        this.gameManager.ChangeGameState(GameState.LOADOUTCREATION);
    }

    // call, when busstop is searched in searchfield
    public void BusSearchInputFieldChanged()
    {
        this.locationTrackedIcon.SetActive(false);
        this.gameManager.SearchBusStop(this.busSearchInputField.text);
        this.toggleLoadingBusstopIcon = true;
    }

    // call, when busstop is searched in searchfield
    public void SearchClosestStopBtnPressed()
    {
        this.locationTrackedIcon.SetActive(true);
        this.gameManager.SearchClosestStopToPLayer();
        this.toggleLoadingBusstopIcon = true;
    }

    // Update search text when busstopsearch is updated externally for example with closest search
    public void UpdateInputFieldText(string text)
    {
        this.busSearchInputField.text = text;
    }

    private void GenerateBusSelection()
    {
        UpdateDistanceToStopText();

        this.toggleLoadingBusstopIcon = false;

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
            int playTimeInMinutes = (int)playTime.TotalMinutes;
            btn.transform.Find("TimeLabel").GetComponent<TMP_Text>().text += $"\n{playTimeInMinutes} Min Spielzeit";

            // change color of button of selected bus
            if (this.gameManager.selectedBus != null && this.gameManager.busses[i].line == this.gameManager.selectedBus.line && this.gameManager.busses[i].time == this.gameManager.selectedBus.time)
            {
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
            distanceToStopText.GetComponent<TMP_Text>().text = this.distanceToStop + "m, bitte näher an die Bushaltestelle heran.";
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
        TurretType[] types = (TurretType[])System.Enum.GetValues(typeof(TurretType));

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

    public void UpdateLoadoutVisualization()
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

    public void fillEmptySlots(List<TurretType> shownLoadout)
    {
        for (int emptySlotIndex = 0; emptySlotIndex < shownLoadout.Count; emptySlotIndex++)
        {
            List<TurretPrefabMapping> turretMappings = this.loadoutPanel.GetComponentInChildren<TurretGridFiller>().turretMappings;
            var result = turretMappings.FirstOrDefault(x => x.data.turretType == shownLoadout[emptySlotIndex]);
            var currentEmptySlot = this.gameManager.buildManager.emptySlots[emptySlotIndex];
            currentEmptySlot.GetComponent<Image>().sprite = result.buttonSprite;
            currentEmptySlot.GetComponent<EmptySlotHover>().texture = result.data.turretIconTexture;
            currentEmptySlot.GetComponent<EmptySlotHover>().Turret = result.prefab;
            currentEmptySlot.GetComponent<EmptySlotHover>().changedDragObject = result.DragObject;
            currentEmptySlot.GetComponent<EmptySlotHover>().turretType = result.data.turretType;

        }
    }

    public void initTemporaryLoadouts()
    {
        if (initializedTemporaryLoadouts)
        {

            for (int i = 0; i < 4; i++)
            {
                temporaryLoadouts[i] = SaveManager.LoadLoadout(i + 1);
            }
        }
        else
        {
            initializedTemporaryLoadouts = true;
            for (int i = 0; i < 4; i++)
            {
                temporaryLoadouts.Add(SaveManager.LoadLoadout(i + 1));
            }
        }
        //button hervorheben 1
        shownLoadout = temporaryLoadouts[currentLoadoutIndex-1];
        fillEmptySlots(shownLoadout);
    }

    public void UpdateUhraniumLoadoutDisplay()
    {
        this.uhraniumTextLoadout.text = Mathf.FloorToInt(SaveManager.LoadUhranium()).ToString();
    }

    public void ContinueGameAfterPause()
    {
        gameStartedFromPause = true;
        this.gameManager.ChangeGameState(GameState.LEVELPLAYING);
        UnfreezeTime();
    }

    public void NavigateToPause()
    {
        this.gameManager.ChangeGameState(GameState.PAUSING);
    }

    public void NavigateBackFromSettings()
    {
        switch (stateBeforeSettingsVisit)
        {
            case GameState.MAINMENU:
                NavigateToMainMenu();
                break;
            case GameState.PAUSING:
                NavigateToPause();
                break;
            default:
                Debug.Log("Shouldn't happen");
                break;
        }
    }

    public void FreezeTime()
    {
        Time.timeScale = 0;
        mainCamera.GetComponent<PlayerHitEffect>().PauseShake();
    }

    public void UnfreezeTime()
    {
        Time.timeScale = 1;
        mainCamera.GetComponent<PlayerHitEffect>().ResumeShake();
    }

    public void HitFreeze(float seconds)
    {
        StartCoroutine(HitFreezeCoroutine(seconds));
    }

    private IEnumerator HitFreezeCoroutine(float seconds)
    {
        FreezeTime();
        yield return new WaitForSecondsRealtime(seconds);
        UnfreezeTime();
    }

    public void TriggerMultikill(Vector3 whereToDisplay)
    {
        // Show a message
        this.gameManager.SpawnFloatingText(whereToDisplay, "MULTIKILL!", Color.yellow);

        HapticManager.Instance.PlayVibration(150, 200); // 150 ms, 200/255 Stärke

        // Hitfreeze the screen for more impact
        HitFreeze(0.7f);

        // Drop some extra zeitsand
        float dropDuration = 10;
        float dropAmount = 5;
        if (zeitsandDrop != null)
        {
            // Drop 3 zeitsand

            GameObject droppedItem = Instantiate(zeitsandDrop, whereToDisplay, transform.rotation);
            droppedItem.GetComponent<DropProperties>().dropAmount = dropAmount;
            droppedItem.GetComponent<DropProperties>().survivalDuration = dropDuration;
            droppedItem.GetComponent<DropProperties>().remainingDuration = dropDuration;
            this.audioManager.PlaySfx(this.audioManager.soundLibrary.sfxZeitsandDropped);

            GameObject droppedItem2 = Instantiate(zeitsandDrop, whereToDisplay + new Vector3(0.3f,0.3f,0), transform.rotation);
            droppedItem2.GetComponent<DropProperties>().dropAmount = dropAmount;
            droppedItem2.GetComponent<DropProperties>().survivalDuration = dropDuration;
            droppedItem2.GetComponent<DropProperties>().remainingDuration = dropDuration;
            this.audioManager.PlaySfx(this.audioManager.soundLibrary.sfxZeitsandDropped);

            GameObject droppedItem3 = Instantiate(zeitsandDrop, whereToDisplay + new Vector3(-0.2f, -0.1f, 0), transform.rotation);
            droppedItem3.GetComponent<DropProperties>().dropAmount = dropAmount;
            droppedItem3.GetComponent<DropProperties>().survivalDuration = dropDuration;
            droppedItem3.GetComponent<DropProperties>().remainingDuration = dropDuration;
            this.audioManager.PlaySfx(this.audioManager.soundLibrary.sfxZeitsandDropped);
        } else
        {
            Debug.Log("Drop Prefab not found");
        }
    }
}