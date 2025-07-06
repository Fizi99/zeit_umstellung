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
    
    // related to uhranium
    [HideInInspector] public float uhranium = 0f;

    public float uhrraniumRatePerSec = 1;
    public int savableThreshhold = 10;
    [HideInInspector] public float uhraniumGain = 0; // to display uhranium gain, after it has been reset

    private GameManager gameManager;

    public bool firstTimePlaying;
    public bool playTutorial = true;

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
        
        
        
        chosenLoadout = SaveManager.LoadLoadout(1);
        //SaveManager.SaveLoadout(1, changedLoadoutSth);
        //SaveManager.SaveLoadout(2, changedLoadoutSth);
        //SaveManager.SaveLoadout(3, changedLoadoutSth);
        //SaveManager.SaveLoadout(4, changedLoadoutSth);
        this.gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        this.firstTimePlaying = SaveManager.LoadFirstTimePlaying();
        //Debug.Log("first time?" + this.firstTimePlaying);
        this.playTutorial = firstTimePlaying;

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
        uhranium += Time.deltaTime * uhrraniumRatePerSec;
    }

    // reset uhranium for example if lvl is finished over button press. this way, not saved uhranium is not saved
    public void ResetUhranium()
    {
        this.uhranium = 0;
    }

    public void SaveUhranium()
    {

        SaveManager.SaveUhranium(SaveManager.LoadUhranium()+this.uhranium);
        Debug.Log("current saved uhranium: " + Mathf.FloorToInt(SaveManager.LoadUhranium()).ToString());
        this.uhraniumGain += (int) this.uhranium;
        this.uhranium = 0;

        // Check if updating the uhranium highscore is needed (while being in-game)
        float lastUhraniumLevelSaved = SaveManager.LoadUhranium();
        if (SaveManager.LoadUhraniumHighscore() < lastUhraniumLevelSaved)
        {
            SaveManager.SaveUhraniumHighscore(lastUhraniumLevelSaved);
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
