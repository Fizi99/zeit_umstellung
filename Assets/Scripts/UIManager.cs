using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{

    [SerializeField] private TMP_InputField busSearchInputField;
    [SerializeField] private TMP_Text countdown;
    [SerializeField] private GameObject gridLayout;
    [SerializeField] private GameObject busSelectorBtnPrefab;

    [SerializeField] private GameObject startLevelBtn;
    [SerializeField] private GameObject endLevelBtn;

    private GameManager gameManager;
    private List<Bus> busses;
    private List<GameObject> busSelectionBtns = new List<GameObject>();
    private bool busInfoUpdated = false;

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

            }
            else
            {
                text = "Bus already departed. pick another";
            }
            this.countdown.fontSize = 12;
        }
        else
        {
            text = $"{timeSpan:hh':'mm':'ss}";
            this.countdown.fontSize = 36;
        }

        // show countdown
        this.countdown.text = text;

    }

    public void StartLevel()
    {
        // dont want to start level, if selected bus already departed
        TimeSpan timeSpan = (System.DateTimeOffset.FromUnixTimeSeconds(this.selectedBus.realtime).LocalDateTime - System.DateTime.Now);
        if (timeSpan.Seconds < 0)
        {
            return;
        }
        // empty bus selection list
        foreach (GameObject busSelectionBtn in this.busSelectionBtns)
        {
            Destroy(busSelectionBtn);
        }

        this.busSearchInputField.gameObject.SetActive(false);
        this.endLevelBtn.SetActive(true);
        this.startLevelBtn.SetActive(false);
        this.gameManager.StartLevel();
    }

    public void EndLevel()
    {
        // when level is ended, regenerate bus selection: TODO: LATER CHANGE SO SHOWN UI DEPENDS ON GAMESTATE
        GenerateBusSelection();

        this.busSearchInputField.gameObject.SetActive(true);
        this.endLevelBtn.SetActive(false);
        this.startLevelBtn.SetActive(true);
        this.gameManager.EndLevel();
    }

    // call, when busstop is searched in searchfield
    public void BusSearchInputFieldChanged()
    {
        this.gameManager.SearchBusStop(this.busSearchInputField.text);
    }

    private void GenerateBusSelection()
    {
        // empty bus selection list
        foreach (GameObject busSelectionBtn in this.busSelectionBtns)
        {
            Destroy(busSelectionBtn);
        }
        // add new bus selection buttons
        for (int i = 0; i < this.gameManager.busses.Count; i++)
        {
            // create new selection button and set grid as parent
            GameObject btn = GameObject.Instantiate(this.busSelectorBtnPrefab);
            btn.transform.SetParent(this.gridLayout.transform);
            btn.GetComponent<BusSelectorBtn>().bus = this.gameManager.busses[i];
            btn.GetComponentInChildren<TMP_Text>().text = this.gameManager.busses[i].line + " Richtung: " + this.gameManager.busses[i].headsign + " um " + System.DateTimeOffset.FromUnixTimeSeconds(this.busses[i].realtime).LocalDateTime.TimeOfDay;
            this.busSelectionBtns.Add(btn);

        } 
    }


}
