using UnityEngine;

public class BusSelectorBtn : MonoBehaviour
{

    private GameManager gameManager;
    public Bus bus;


    void Start()
    {
        this.gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClick()
    {
        this.gameManager.selectedBus = bus;
        Debug.Log(this.bus.line + " Richtung: " + this.bus.headsign + " um " + System.DateTimeOffset.FromUnixTimeSeconds(this.bus.realtime).LocalDateTime.TimeOfDay);
    }

}
