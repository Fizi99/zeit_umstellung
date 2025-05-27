using UnityEngine;

public class playerStats : MonoBehaviour
{

    public static int lives;
    public int startLives = 20;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        lives=startLives;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
