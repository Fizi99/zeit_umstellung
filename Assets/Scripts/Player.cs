using UnityEngine;
using System.Collections.Generic;

public class Player : MonoBehaviour
{
    public double lat;
    public double lon;
    public int lives;
    public int startLives = 20;

    // Related to zeitsand
    public float zeitsand = 0;
    public float maxZeitsand = 100;
    public float zeitsandStartValue = 00;
    public float zeitsandRatePerSec = 1;

    // related to uhranium
    public float uhranium = 0;
    public float savedUhranium = 0;
    public float uhrraniumRatePerSec = 1;
    public int savableThreshhold = 10;
    public float uhraniumGain = 0; // to display uhranium gain, after it has been reset

    private GameManager gameManager;

    public List<TurretType> chosenLoadout = new List<TurretType>()
    {
        TurretType.STANDARD,
        TurretType.MISSILE,
        TurretType.DRONEBASE,
        TurretType.LASER
    };

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
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

    public void SaveUhranium()
    {
        this.savedUhranium += (int) this.uhranium;
        this.uhraniumGain += (int) this.uhranium;
        this.uhranium = 0;

        // Check if updating the uhranium highscore is needed (while being in-game)
        float lastUhraniumLevelSaved = this.savedUhranium;
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

    public void SetLoadout(List<TurretType> newLoadout)
    {
        this.chosenLoadout = newLoadout;
    }
}
