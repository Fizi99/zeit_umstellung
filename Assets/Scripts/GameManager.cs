using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public List<Bus> busses;
    [HideInInspector]
    public List<GameObject> streets;
    public Bus selectedBus;
    public Player player = new Player();
    public BusStop busStop;
    [HideInInspector]
    public GameObject busStopGO = null;

    public GameState gameState;

    [SerializeField] public BusTimeScraper busTimeScraper;
    [SerializeField] public StreetViewMap streetViewMapGetter;
    [SerializeField] public TMP_InputField busSearchInputField;
    [SerializeField] public UIManager uiManager;
    [SerializeField] public GPSTracker gpsTracker;
    [SerializeField] public WaveSpawner waveSpawner;
    [SerializeField] public RouteManager routeManager;


    void Start()
    {
        this.gameState = GameState.MAINMENU;
        InvokeRepeating("UpdateBusInformation", 1, 5);
        //LocationService location = new LocationService();
        //location.Start();
        //Debug.Log(location.status);
    }

    private void Update()
    {
        
    }

    public void ChangeGameState(GameState gameState)
    {
        this.gameState = gameState;
        this.uiManager.UpdateUI();
    }

    public void UpdateBusStopData(BusStop newData)
    {
        this.busStop = newData;
    }

    public void UpdateBusStopGameObject(GameObject newBusStop)
    {
        this.busStopGO = newBusStop;
    }

    // generate new streets. Set them so other scripts can access them, get the closest street to the bus stop and add that point as last destination to every street
    // TODO: only add last waypoint to eligable streets (maybe use OSRM?)
    public void SetStreets(List<GameObject> streets)
    {
        this.streets = streets;
        this.waveSpawner.setAmountOfRoutes(streets.Count);
        // draw line between closest point on street and bus stop
        foreach(GameObject street in streets)
        {
            street.GetComponent<Street>().GetClosestPointToBusStop();
        }

        DrawClosestStreet();
        this.routeManager.AddLastWaypointToRoutes(this.streets);
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

    public double CalcDistanceBetweenCordsInM(double lat1, double lon1, double lat2, double lon2)
    {
        return this.gpsTracker.CalcDistanceBetweenCordsInM(lat1, lon1, lat2, lon2);
    }

    public void SetPlayerCoords(double lat, double lon)
    {
        this.player.SetPlayerCoords(lat, lon);
    }

    public double GetPlayerLat()
    {
        return this.player.lat;
    }
    public double GetPlayerLon()
    {
        return this.player.lon;
    }

    public double GetDistanceThreshhold()
    {
        return this.gpsTracker.GetDistanceThreshhold();
    }

    //TODO: aufräumen
    public Vector3 GetClosestPointToBusStop(Vector3 p, Vector3 a, Vector3 b)
    {
        if(this.busStopGO == null)
        {
            return this.routeManager.GetClosestPointOnLine(Vector3.zero, a, b);
        }
        return this.routeManager.GetClosestPointOnLine(this.busStopGO.transform.position, a, b);

    }

    public void UpdateStreetsWithLastWaypoint()
    {

    }

    // Gets closest street to bus stop and draws sphere at position
    public void DrawClosestStreet()
    {
        GameObject closestStreet = null;
        double shortestDistance = 9999999;
        foreach(GameObject street in streets)
        {
            if(street.GetComponent<Street>().distance < shortestDistance)
            {
                closestStreet = street;
                shortestDistance = street.GetComponent<Street>().distance;
            }
                   
        }


        this.routeManager.lastWaypointPos = closestStreet.GetComponent<Street>().closestPointOnStreet;

        GameObject point = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        point.transform.position = closestStreet.GetComponent<Street>().closestPointOnStreet;
        point.transform.localScale = Vector3.one * 0.2f;

        // Optional: Farbe setzen
        var renderer = point.GetComponent<Renderer>();
        if (renderer != null)
        {
            Material mat = new Material(Shader.Find("Standard"));
            mat.color = Color.magenta;
            renderer.material = mat;
        }

    }
}
