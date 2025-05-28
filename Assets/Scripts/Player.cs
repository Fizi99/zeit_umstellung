using UnityEngine;

public class Player : MonoBehaviour
{

    public double lat;
    public double lon;
    public int lives;
    public int startLives = 20;

    public void SetPlayerCoords(double lat, double lon)
    {
        this.lat = lat;
        this.lon = lon;
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        lives = startLives;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void TakeDamage(int dmg)
    {
        this.lives -= dmg;
    }

}
