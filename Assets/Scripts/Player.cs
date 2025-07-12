using UnityEngine;
using System.Collections.Generic;

public class Player : MonoBehaviour
{
    public double lat;
    public double lon;
    public int lives;
    public int startLives = 20;

    // Related to zeitsand
    [HideInInspector] public float zeitsand = 0;
    public float maxZeitsand = 100;
    public float zeitsandStartValue = 00;
    public float zeitsandRatePerSec = 1;
    
    // Related to uhranium
    [HideInInspector] public float uhranium = 0f;

    public float uhraniumRatePerSec = 1;
    public int savableThreshhold = 10;
    [HideInInspector] public float uhraniumGain = 0; // to display uhranium gain, after it has been reset

    private GameManager gameManager;

    public bool firstTimePlaying;
    public bool playTutorial;
    public bool showTutorialEveryTime;

    public List<TurretType> chosenLoadout = new List<TurretType>();

    //public List<TurretType> changedLoadoutSth = new List<TurretType>()
    //{
    //    TurretType.MISSILE,
    //    TurretType.MISSILE,
    //    TurretType.MISSILE,
    //    TurretType.MISSILE
    //};
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (SaveManager.LoadFirstTimePlaying())
        {
            SaveManager.SavePurchasedTurrets(new List<TurretType>()
            {
                TurretType.STANDARD,
                TurretType.MISSILE,
                TurretType.DRONEBASE,
                TurretType.LASER,
            });
        }


        chosenLoadout = SaveManager.LoadLoadout(1);
        //SaveManager.SaveLoadout(1, changedLoadoutSth);
        //SaveManager.SaveLoadout(2, changedLoadoutSth);
        //SaveManager.SaveLoadout(3, changedLoadoutSth);
        //SaveManager.SaveLoadout(4, changedLoadoutSth);
        this.gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        this.firstTimePlaying = SaveManager.LoadFirstTimePlaying();
        this.showTutorialEveryTime = SaveManager.LoadToggle(SettingOption.ShowTutorialEveryTime);

        this.playTutorial = this.firstTimePlaying || this.showTutorialEveryTime;

        lives = startLives;
        this.zeitsand = this.zeitsandStartValue;
    }

    // Update is called once per frame
    void Update()
    {
        if(this.gameManager.gameState == GameState.LEVELPLAYING)
        {
            UpdateUhranium();
            UpdateZeitsand();
        }
    }

    public void SetPlayerCoords(double lat, double lon)
    {
        this.lat = lat;
        this.lon = lon;
    }

    private void UpdateZeitsand()
    {
        zeitsand += Time.deltaTime * zeitsandRatePerSec;
        zeitsand = Mathf.Clamp(zeitsand, 0, maxZeitsand);
    }

    public void SetZeitsand(float adjustedZeitsand)
    {
        zeitsand = adjustedZeitsand;
    }

    private void UpdateUhranium()
    {
        uhranium += Time.deltaTime * uhraniumRatePerSec;
    }

    // reset uhranium for example if lvl is finished over button press. this way, not saved uhranium is not saved
    public void ResetUhranium()
    {
        this.uhranium = 0;
    }

    public void SaveUhranium()
    {

        SaveManager.SaveUhranium(SaveManager.LoadUhranium()+this.uhranium);
        HapticManager.Instance.PlayVibration(75, 200);
        Debug.Log("current saved uhranium: " + Mathf.FloorToInt(SaveManager.LoadUhranium()).ToString());
        this.uhraniumGain += (int) this.uhranium;
        this.uhranium = 0;

        // Check if displaying highscore-reached-text (while being in-game)
        //float lastUhraniumLevelSaved = SaveManager.LoadUhranium(); // IMPORTANT: BUGGY, LOADUHRANIUM GIVES ALL UHRANIUM COMBINED, WE WANT THE UHRANIUM OF JUST THIS LEVEL (ONE SESSION)
        float uhraniumThisSession = uhraniumGain;
        if (SaveManager.LoadUhraniumHighscore() < uhraniumThisSession)
        {
            SaveManager.SaveUhraniumHighscore(uhraniumThisSession);
            this.gameManager.highscoreTracker.GetComponent<HighscoreTracker>().SetHighscoreDisplayVisibility(true);
            HapticManager.Instance.PlayVibration(150, 200); // 150 ms, 200/255 Stärke
        }
    }

    public void ResetUhraniumGain()
    {
        this.uhraniumGain = 0;
    }

    public void TakeDamage(int dmg)
    {
        this.uhranium -= dmg;
        this.uhranium = Mathf.Max(this.uhranium, 0);
        this.lives -= dmg;
        this.gameManager.SpawnFloatingText(new Vector3(0, 1, -1), "-" + dmg, Color.red);
        this.gameManager.TriggerScreenEffect();
    }

    public void setZeitsandRatePerSec(float zeitSandRate)
    {
        this.zeitsandRatePerSec = zeitSandRate;
    }

    public void addZeitsand(float zeitSandAmount)
    {
        if (this.zeitsand + zeitSandAmount > maxZeitsand)
        {
            this.zeitsand = maxZeitsand;
        }
        else
        {
            this.zeitsand += zeitSandAmount;
        }
    }

    public void SetLoadout(List<TurretType> newLoadout)
    {
        this.chosenLoadout = newLoadout;
    }
}
