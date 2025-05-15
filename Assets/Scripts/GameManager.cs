using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public List<Bus> busses;
    public List<GameObject> streets;
    public Bus selectedBus;

    public GameState gameState;

    [SerializeField] public BusTimeScraper busTimeScraper;
    [SerializeField] public StreetViewMap streetViewMapGetter;
    [SerializeField] public TMP_InputField busSearchInputField;
    [SerializeField] public UIManager uiManager;


    void Start()
    {
        this.gameState = GameState.LEVELSELECTION;
        InvokeRepeating("UpdateBusInformation", 1, 5);
    }

    private void Update()
    {
        
    }

    public void ChangeGameState(GameState gameState)
    {
        this.gameState = gameState;
    }


    public void SearchBusStop(string busStop)
    {
        this.busTimeScraper.SearchBusStop(busStop);
    }

    public void SearchStreetsAroundCenter(double searchCenterLat, double searchCenterLon)
    {
        this.streetViewMapGetter.SearchStreetsAroundCenter(searchCenterLat, searchCenterLon);
    }

    public void UpdateBusInformation()
    {
        this.busTimeScraper.UpdateBusInformation();
        Debug.Log("update");
    }
}
