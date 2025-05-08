using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public List<Bus> busses;
    public List<GameObject> streets;

    [SerializeField] public BusTimeScraper busTimeScraper;
    [SerializeField] public StreetViewMap streetViewMapGetter;
    [SerializeField] public TMP_InputField busSearchInputField;


    void Start()
    {
    }

    // call, when busstop is searched in searchfield
    public void BusSearchInputFieldChanged()
    {
        busTimeScraper.SearchBusStop(busSearchInputField.text);
    }
}
