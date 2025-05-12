using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public List<Bus> busses;
    public List<GameObject> streets;
    public Bus selectedBus;

    [SerializeField] public BusTimeScraper busTimeScraper;
    [SerializeField] public StreetViewMap streetViewMapGetter;
    [SerializeField] public TMP_InputField busSearchInputField;
    [SerializeField] public UIManager uiManager;


    void Start()
    {
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
    }
}
