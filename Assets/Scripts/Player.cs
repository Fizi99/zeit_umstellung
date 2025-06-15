using UnityEngine;

public class Player : MonoBehaviour
{

    public double lat;
    public double lon;
    public int lives;
    public int startLives = 20;

    public float zeitsand = 0;
    public float uhranium = 0;

    public float maxZeitsand = 100;
    public float zeitsandStartValue = 00;
    public float zeitsandRatePerSec = 1;

    public float savedUhranium = 0;
    public float uhrraniumRatePerSec = 1;
    // to display uhranium gain, after it has been reset
    public float uhraniumGain = 0;

    private GameManager gameManager;


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
        this.savedUhranium += (int)this.uhranium;
        this.uhraniumGain = (int)this.uhranium;
        this.uhranium = 0;
    }



    public void TakeDamage(int dmg)
    {
        this.uhranium -= dmg;
        this.uhranium = Mathf.Max(this.uhranium, 0);
        this.lives -= dmg;
    }

    public void setZeitsandRatePerSec(float zeitSandRate)
    {
        this.zeitsandRatePerSec = zeitSandRate;
    }
}
