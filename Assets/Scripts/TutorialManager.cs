using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] public GameObject tutorialPanel;
    [SerializeField] public List<TutorialDataSObj> tutorialParts;
    [SerializeField] public TMP_Text tutorialText;

    [SerializeField] public GameObject highlightShop;
    [SerializeField] public GameObject highlightUhranium;
    [SerializeField] public GameObject highlightBusstop;


    [HideInInspector] public bool playTutorial = true;
    public bool isActive = false;

    private int currentPart;

    // which aspekts should be controlled by tutorial?:
    // - enemyspawning
    // - zeitsand
    // - turretplacing
    // - uhraniumrate

    private float defaultZeitsandRate;
    private float defaultUhraniumRate;
    private float defaultzeitsandStartValue;


    private GameManager gameManager;

    private void Start()
    {
        this.gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }


    private void Update()
    {
        if (this.isActive)
        {
            // if tapped, go to next part of tutorial
            if (Input.GetMouseButtonDown(0))
            {
                NextPart();
            }

            if(this.gameManager.gameState != GameState.LEVELPLAYING)
            {
                FinishTutorial();
            }
        }
    }

    // start the tutorial if player is playing for first time or explicitly activated tutorial
    public void PlayTutorial()
    {
        // Check whether the tutorial can be played immediately
        bool firstTime = SaveManager.LoadFirstTimePlaying();
        bool showAlways = SaveManager.LoadToggleTutorialState();
        this.gameManager.player.playTutorial = firstTime || showAlways;

        if (this.gameManager.player.playTutorial)
        {
            if (this.gameManager.gameState == GameState.LEVELPLAYING)
            {
                ControlVariablesForTutorial();
                this.tutorialPanel.SetActive(true);
                this.isActive = true;
                this.currentPart = 0;
                this.tutorialText.text = this.tutorialParts[this.currentPart].text;
                Highlight();
            }
        }
        
    }

    // show the next part of the tutorial
    public void NextPart()
    {
        this.currentPart++;
        if(this.currentPart < this.tutorialParts.Count)
        {
            this.tutorialText.text = this.tutorialParts[this.currentPart].text;
            Highlight();
        }
        else
        {
            FinishTutorial();
        }
        
    }

    // choose which ui part to highlight
    private void Highlight()
    {
        DeactivateHighlights();
        switch (this.tutorialParts[this.currentPart].part)
        {
            case TutorialPart.BUSSTOP:
                this.highlightBusstop.SetActive(true);
                break;
            case TutorialPart.TURRETS:
                this.highlightShop.SetActive(true);
                break;
            case TutorialPart.UHRANIUM:
                this.highlightUhranium.SetActive(true);
                break;
            default:
                break;
        }
    }
    private void DeactivateHighlights()
    {
        this.highlightShop.SetActive(false);
        this.highlightUhranium.SetActive(false);
        this.highlightBusstop.SetActive(false);
    }

    // finish the tutorial and save, that player played the game before
    public void FinishTutorial()
    {
        this.tutorialPanel.SetActive(false);
        this.currentPart = 0;
        this.isActive = false;
        SaveManager.SaveFirstTimePlaying(false);
        ResetControlledVariables();

    }

    private void ControlVariablesForTutorial()
    {
        // contorl zeitsand
        this.defaultZeitsandRate = this.gameManager.player.zeitsandRatePerSec;
        this.gameManager.player.zeitsandRatePerSec = 0;
        this.defaultzeitsandStartValue = this.gameManager.player.zeitsandStartValue;

        // stop enemy spawning
        this.gameManager.waveSpawner.SetStopWaveSpawning(true);

        // control uhranium
        this.defaultUhraniumRate = this.gameManager.player.uhrraniumRatePerSec;
        this.gameManager.player.uhrraniumRatePerSec = 1;
    }

    private void ResetControlledVariables()
    {
        this.gameManager.player.zeitsand = this.defaultzeitsandStartValue;
        this.gameManager.player.zeitsandRatePerSec = this.defaultZeitsandRate;
        this.gameManager.player.uhrraniumRatePerSec = this.defaultUhraniumRate;
        this.gameManager.waveSpawner.SetStopWaveSpawning(false);
    }
}
