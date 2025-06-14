using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Controlled components")]

    [SerializeField] private TMP_InputField busSearchInputField;
    [SerializeField] private List<TMP_Text> countdown;
    [SerializeField] private GameObject gridLayout;
    [SerializeField] private GameObject busSelectorBtnPrefab;
    [SerializeField] private TMP_Text zeitsandText;
    [SerializeField] private TMP_Text uhraniumText;
    [SerializeField] private TMP_Text lvlFinishedText;
    [Space(10)]
    [Header("Panel for navigation")]
    [SerializeField] private GameObject lvlSelectionPanel;
    [SerializeField] private GameObject lvlPlayingPanel;
    [SerializeField] private GameObject startMenuPanel;
    [SerializeField] private GameObject upgradingMenuPanel;
    [SerializeField] private GameObject lvlEndPanel;

    [SerializeField] private GameObject distanceToStopText;

    [SerializeField] private GameObject debugText;

    private GameManager gameManager;
    private List<Bus> busses;
    private List<GameObject> busSelectionBtns = new List<GameObject>();
    private bool busInfoUpdated = false;
    private double distanceToStop = 0;

    private Bus selectedBus;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateSelectedBus();
        UpdateCountdown();
        CheckForBusInfoUpdate();
        if (busInfoUpdated)
        {
            GenerateBusSelection();
        }

        if(this.gameManager.gameState == GameState.LEVELPLAYING)
        {
            UpdateUhraniumText();
            UpdateZeitsandText();
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
            text = $"{timeSpan:hh':'mm':'ss}";
            //foreach (TMP_Text countdownDisplay in this.countdown)
            //    countdownDisplay.fontSize = 36;
        }

        // show countdown
        foreach (TMP_Text countdownDisplay in this.countdown)
            countdownDisplay.text = text;

    }

    private void UpdateUhraniumText()
    {
        this.uhraniumText.text = "Uhranium: "+(int)this.gameManager.player.uhranium;
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
                break;

            case GameState.LEVELPLAYING:
                this.lvlSelectionPanel.SetActive(false);
                this.lvlPlayingPanel.SetActive(true);
                this.startMenuPanel.SetActive(false);
                this.upgradingMenuPanel.SetActive(false);
                this.lvlEndPanel.SetActive(false);
                break;

            case GameState.UPGRADING:
                this.lvlSelectionPanel.SetActive(false);
                this.lvlPlayingPanel.SetActive(false);
                this.startMenuPanel.SetActive(false);
                this.upgradingMenuPanel.SetActive(true);
                this.lvlEndPanel.SetActive(false);
                break;
            case GameState.MAINMENU:
                this.lvlSelectionPanel.SetActive(false);
                this.lvlPlayingPanel.SetActive(false);
                this.startMenuPanel.SetActive(true);
                this.upgradingMenuPanel.SetActive(false);
                this.lvlEndPanel.SetActive(false);
                break;
            case GameState.LEVELEND:
                this.lvlSelectionPanel.SetActive(false);
                this.lvlPlayingPanel.SetActive(false);
                this.startMenuPanel.SetActive(false);
                this.upgradingMenuPanel.SetActive(false);
                this.lvlEndPanel.SetActive(true);

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

                break;
            case GameState.LEVELEND:
                UpdateLvlFinishedText();

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
            btn.transform.SetParent(this.gridLayout.transform);
            btn.GetComponent<BusSelectorBtn>().bus = newList[i];
            btn.GetComponentInChildren<TMP_Text>().text = newList[i].line + " Richtung: " + newList[i].headsign + " um " + $"{System.DateTimeOffset.FromUnixTimeSeconds(newList[i].time).LocalDateTime.TimeOfDay:hh\\:mm}" + "+" + System.DateTimeOffset.FromUnixTimeSeconds(newList[i].realtime - newList[i].time).Minute;

            // change color of button of selected bus
            if (this.gameManager.selectedBus != null && this.gameManager.busses[i].line == this.gameManager.selectedBus.line && this.gameManager.busses[i].time == this.gameManager.selectedBus.time)
            {
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


}
