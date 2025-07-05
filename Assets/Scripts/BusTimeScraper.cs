using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.Text.RegularExpressions;
using SimpleJSON;
using System.Collections.Generic;
using System.Net.Http;
using System.Net;
using System.Threading.Tasks;
using System;

public class BusTimeScraper : MonoBehaviour
{
    [SerializeField] private string busstop = "Technische-Hochschule";
    [SerializeField] private double searchRadiusForClosestBusstop = 100;
    private int hafas = 0;
    private string busstopActual = "";
    // URL to fetch hafas of specific bus stop
    private string busStopSearchURL = "https://netzplan.swhl.de/api/v1/stationboards/hafas/9057765";
    // URL to fetch specific busstop with hafas
    private string busStopURL = "";
    // representation of bus stop with data aboput location etc.
    private BusStop busStopData;

    private List<Bus> busses = new List<Bus>();

    private GameManager gameManager;


    void Start()
    {
        this.gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        this.busStopSearchURL = "https://nah.sh.hafas.de/bin/ajax-getstop.exe/eny?start=1&start=1&tpl=suggest2json&getstop=1&getstop=1&noSession=yes&=&encoding=utf-8&S=l�beck" + this.busstop + "?&REQ0JourneyStopsS0A=255&REQ0JourneyStopsB=12";
        StartCoroutine(FetchBusStopHafas());

    }

    private void Update()
    {
        UpdateGameManagerBusList();
    }

    // Updates bus list of gamemanager, to grant access to other components
    private void UpdateGameManagerBusList()
    {
        // update data of selected bus
        if (this.gameManager.selectedBus != null)
        {
            for (int i = 0; i < this.busses.Count; i++)
            {
                if (this.gameManager.selectedBus.line == this.busses[i].line && this.gameManager.selectedBus.time == this.busses[i].time)
                {
                    this.gameManager.selectedBus = this.busses[i];
                }
            }
        }


        this.gameManager.busses = this.busses;
    }

    // Find the closest Busstop to the player
    public void FindClosestBusStop()
    {

        if (this.gameManager.GetPlayerLat() != 0 && this.gameManager.GetPlayerLon() != 0)
        {

            StartCoroutine(QueryBusStopAroundLocation());

        }
    }

    IEnumerator QueryBusStopAroundLocation()
    {

        // Query for busstops around player
        string searchCenterLatStr = this.gameManager.GetPlayerLat().ToString("G", System.Globalization.CultureInfo.CreateSpecificCulture("en-US"));
        string searchCenterLonStr = this.gameManager.GetPlayerLon().ToString("G", System.Globalization.CultureInfo.CreateSpecificCulture("en-US"));
        string searchRadiusStr = this.searchRadiusForClosestBusstop.ToString("G", System.Globalization.CultureInfo.CreateSpecificCulture("en-US"));
        string url = "https://overpass-api.de/api/interpreter?data=[out:json];node[%22highway%22=%22bus_stop%22](around:"+searchRadiusStr + "," + searchCenterLatStr + ","+ searchCenterLonStr + ");out%20body;";
        UnityWebRequest request = UnityWebRequest.Get(url);

        yield return request.SendWebRequest();
        {
            JSONNode data = null;
            if (request.result == UnityWebRequest.Result.Success)
            {
                List<BusStop> stopsInRange = new List<BusStop>();
                data = JSON.Parse(request.downloadHandler.text);
                foreach (JSONNode element in data["elements"].AsArray)
                {
                    // get information of busstops close to player
                    if (element["type"] == "node")
                    {
                        string name = element["tags"]["name"];
                        double lat = element["lat"].AsDouble;
                        double lon = element["lon"].AsDouble;
                        stopsInRange.Add(new BusStop(lat, lon, name));
                    }
                }

                // calculate closest busstop to player
                double closest = 999999;
                BusStop closestStop = null;
                foreach(BusStop busstop in stopsInRange)
                {
                    double dist = this.gameManager.CalcDistanceBetweenCordsInM(busstop.lat, busstop.lon, this.gameManager.GetPlayerLat(), this.gameManager.GetPlayerLon());
                    if(dist < closest)
                    {
                        closest = dist;
                        closestStop = busstop;
                    }
                }

                // search for busdata of closest stop
                if(closestStop != null)
                {
                    SearchBusStop(closestStop.name);
                }
            }
            else
            {
                Debug.LogError("Fehler beim Abrufen der OSM-Daten: " + request.error);

            }
        }
    }

    // Load for specific busstop
    public void SearchBusStop(string busstop)
    {
        this.busstop = busstop;

        this.busStopSearchURL = "https://nah.sh.hafas.de/bin/ajax-getstop.exe/eny?start=1&start=1&tpl=suggest2json&getstop=1&getstop=1&noSession=yes&=&encoding=utf-8&S=lübeck " + this.busstop + "?&REQ0JourneyStopsS0A=255&REQ0JourneyStopsB=12";
        StartCoroutine(FetchBusStopHafas());
    }

    // fetch new bus information and update list accordingly
    public void UpdateBusInformation()
    {
        StartCoroutine(UpdateBusStopInformation());
    }

    // Get hafas number for searched busstop
    IEnumerator FetchBusStopHafas()
    {

        UnityWebRequest request = UnityWebRequest.Get(this.busStopSearchURL);

        yield return request.SendWebRequest();
        {

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Fehler beim Laden: " + request.error);

                // hard coded technische hochschule in cas of error
                double lon = 10.700337;
                double lat = 53.837951;
                this.busStopData = new BusStop(lat, lon, "Technische Hochschule");
                this.hafas = 9057765;
                this.busstopActual = "Technische Hochschule";
            }
            else
            {

                string json = request.downloadHandler.text.Substring(8);
                json.Remove(json.Length - 1);
                // Parse json return
                JSONNode data = JSON.Parse(json);
                for (int i = 0; i < data["suggestions"].AsArray.Count; i++)
                {
                    if (i == 0)
                    {
                        // get hafas of first element in search
                        double lon = (double)data["suggestions"][i]["xcoord"] / (double)1000000;
                        double lat = (double)data["suggestions"][i]["ycoord"] / (double)1000000;
                        this.busStopData = new BusStop(lat, lon, data["suggestions"][i]["value"]);
                        this.hafas = data["suggestions"][i]["extId"];
                        this.busstopActual = data["suggestions"][i]["value"];

                    }
                }

                // DEBUG 
                /*this.busStopData = new BusStop(53.837944, 10.700381);
                this.hafas = 9057765;
                this.busstopActual = "Technische Hochschule";
                this.gameManager.UpdateBusStopData(busStopData);*/
                // DEBUG 

                if (this.hafas == 0)
                {
                    // hard coded technische hochschule in case of error
                    double lon = 10.700337;
                    double lat = 53.837951;
                    this.busStopData = new BusStop(lat, lon, "Technische Hochschule");
                    this.hafas = 9057765;
                    this.busstopActual = "Technische Hochschule";
                }


            }

            this.gameManager.UpdateBusStopData(this.busStopData);
            this.gameManager.UpdateBusStopSearchInputFieldInUI(this.busstopActual);
            // Load Bus Information
            StartCoroutine(FetchBusStopInformation());
            // Update map so bus stop is centered
            this.gameManager.SearchStreetsAroundCenter(this.busStopData.lat, this.busStopData.lon);
        }


    }


    // Fetch Busstop information like departure of each bus and update current list
    IEnumerator UpdateBusStopInformation()
    {
        this.busStopURL = "https://netzplan.swhl.de/api/v1/stationboards/hafas/" + this.hafas;

        // fetch result for busstop
        UnityWebRequest request = UnityWebRequest.Get(this.busStopURL);
        yield return request.SendWebRequest();
        {
            List<Bus> fetchedBusses = new List<Bus>();

            if (request != null && request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Fehler beim Laden: " + request.error);

            }
            else
            {
                // Parse json return
                JSONNode data = JSON.Parse(request.downloadHandler.text);
                for (int i = 0; i < data["data"].AsArray.Count; i++)
                {
                    // add busses to List
                    fetchedBusses.Add(new Bus(data["data"][i]["line"]["name"], data["data"][i]["headsign"], data["data"][i]["time"], data["data"][i]["realtime"]));
                    //Debug.Log(this.busses[i].line + ": Richtung: " + this.busses[i].headsign + " kommt um: " + System.DateTimeOffset.FromUnixTimeSeconds(this.busses[i].realtime).LocalDateTime.TimeOfDay);


                }


            }
            // loop through new bus information and update bus list.
            List<Bus> bussesTemp = new List<Bus>();
            for (int i = 0; i < fetchedBusses.Count; i++)
            {
                if((System.DateTimeOffset.FromUnixTimeSeconds(fetchedBusses[i].realtime).LocalDateTime - System.DateTime.Now).Seconds > 0)
                {
                    bussesTemp.Add(fetchedBusses[i]);
                }
                /*for (int j = 0; j < this.busses.Count; j++)
                {
                    if (bussesTemp[i].line == this.busses[j].line && bussesTemp[i].time == this.busses[j].time)
                    {
                        this.busses[j] = bussesTemp[i];
                    }
                }*/
            }

            this.busses = bussesTemp;
        }

    }

    // Fetch Busstop information like departure of each bus
    IEnumerator FetchBusStopInformation()
    {
        this.busStopURL = "https://netzplan.swhl.de/api/v1/stationboards/hafas/" + this.hafas;

        // fetch result for busstop
        UnityWebRequest request = UnityWebRequest.Get(this.busStopURL);
        yield return request.SendWebRequest();
        {
            this.busses = new List<Bus>();
            // debug bus that is 5 min late after bus is loaded
            this.busses.Add(new Bus("0", "DEBUG BUS", System.DateTimeOffset.UtcNow.ToUnixTimeSeconds(), System.DateTimeOffset.UtcNow.ToUnixTimeSeconds() + 300));

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Fehler beim Laden: " + request.error);
            }
            else
            {
                // Parse json return
                JSONNode data = JSON.Parse(request.downloadHandler.text);
                for (int i = 0; i < data["data"].AsArray.Count; i++)
                {
                    // add busses to List
                    this.busses.Add(new Bus(data["data"][i]["line"]["name"], data["data"][i]["headsign"], data["data"][i]["time"], data["data"][i]["realtime"]));
                    //Debug.Log(this.busses[i].line + ": Richtung: " + this.busses[i].headsign + " kommt um: " + System.DateTimeOffset.FromUnixTimeSeconds(this.busses[i].realtime).LocalDateTime.TimeOfDay);


                }
            }

            
        }

    }


}
