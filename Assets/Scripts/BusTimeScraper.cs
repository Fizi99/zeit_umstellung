using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.Text.RegularExpressions;
using SimpleJSON;
using System.Collections.Generic;
using System.Net.Http;
using System.Net;
using System.Threading.Tasks;

public class BusTimeScraper : MonoBehaviour
{
    [SerializeField] private string busstop = "Technische-Hochschule";
    private int hafas = 9057765;
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
        this.busStopSearchURL = "https://nah.sh.hafas.de/bin/ajax-getstop.exe/eny?start=1&start=1&tpl=suggest2json&getstop=1&getstop=1&noSession=yes&=&encoding=utf-8&S=lübeck" + this.busstop + "?&REQ0JourneyStopsS0A=255&REQ0JourneyStopsB=12";
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

    // Load for specific busstop
    public void SearchBusStop(string busstop)
    {
        this.busstop = busstop;
        
        this.busStopSearchURL = "https://nah.sh.hafas.de/bin/ajax-getstop.exe/eny?start=1&start=1&tpl=suggest2json&getstop=1&getstop=1&noSession=yes&=&encoding=utf-8&S=lübeck" + this.busstop + "?&REQ0JourneyStopsS0A=255&REQ0JourneyStopsB=12";
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
        // fetch search result for busstop
        /*UnityWebRequest request = UnityWebRequest.Get(this.busStopSearchURL);
        yield return request.SendWebRequest();
        {

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Fehler beim Laden: " + request.error);
            }
            else
            {
                // Parse json return
                JSONNode data = JSON.Parse(request.downloadHandler.text);
                for(int i = 0; i < data["data"].AsArray.Count; i++)
                {
                    if(i== 0)
                    {
                        // get hafas of first element in search
                        this.busStopData = new BusStop(data["data"][i]["lat"], data["data"][i]["lon"]);
                        this.hafas = data["data"][i]["extId"];
                        this.busstopActual = data["data"][i]["name"];
                        this.gameManager.UpdateBusStopData(busStopData);
                    }
                }
                // DEBUG 
                this.busStopData = new BusStop(53.837944, 10.700381);
                this.hafas = 9057765;
                this.busstopActual = "Technische Hochschule";
                this.gameManager.UpdateBusStopData(busStopData);
                // DEBUG 
                Debug.Log(this.busstopActual+ ": " +this.hafas);

                // Load Bus Information
                StartCoroutine(FetchBusStopInformation());
                // Update map so bus stop is centered
                this.gameManager.SearchStreetsAroundCenter(this.busStopData.lat, this.busStopData.lon);
            }
        }*/


        UnityWebRequest request = UnityWebRequest.Get(this.busStopSearchURL);

        yield return request.SendWebRequest();
        {

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Fehler beim Laden: " + request.error);
            }
            else
            {

                string json = request.downloadHandler.text.Substring(8);
                json.Remove(json.Length - 1);
                // Parse json return
                JSONNode data = JSON.Parse(json);
                for(int i = 0; i < data["suggestions"].AsArray.Count; i++)
                {
                    if(i== 0)
                    {
                        // get hafas of first element in search
                        double lon = (double)data["suggestions"][i]["xcoord"]/ (double)1000000;
                        double lat = (double)data["suggestions"][i]["ycoord"]/ (double)1000000;
                        this.busStopData = new BusStop(lat, lon);
                        this.hafas = data["suggestions"][i]["extId"];
                        this.busstopActual = data["suggestions"][i]["value"];
                        this.gameManager.UpdateBusStopData(busStopData);
                        Debug.Log(busstopActual);
                    }
                }

                // DEBUG 
                /*this.busStopData = new BusStop(53.837944, 10.700381);
                this.hafas = 9057765;
                this.busstopActual = "Technische Hochschule";
                this.gameManager.UpdateBusStopData(busStopData);*/
                // DEBUG 

                // Load Bus Information
                StartCoroutine(FetchBusStopInformation());
                // Update map so bus stop is centered
                this.gameManager.SearchStreetsAroundCenter(this.busStopData.lat, this.busStopData.lon);
            }
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

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Fehler beim Laden: " + request.error);
            }
            else
            {
                // Parse json return
                JSONNode data = JSON.Parse(request.downloadHandler.text);
                List<Bus> bussesTemp = new List<Bus>();
                for (int i = 0; i < data["data"].AsArray.Count; i++)
                {
                    // add busses to List
                    bussesTemp.Add(new Bus(data["data"][i]["line"]["name"], data["data"][i]["headsign"], data["data"][i]["time"], data["data"][i]["realtime"]));
                    //Debug.Log(this.busses[i].line + ": Richtung: " + this.busses[i].headsign + " kommt um: " + System.DateTimeOffset.FromUnixTimeSeconds(this.busses[i].realtime).LocalDateTime.TimeOfDay);
                    
                    
                }

                // loop through new bus information and update bus list.
                for (int i=0; i <bussesTemp.Count; i++){
                    for (int j = 0; j < this.busses.Count; j++)
                    {
                        if (bussesTemp[i].line == this.busses[j].line && bussesTemp[i].time == this.busses[j].time)
                        {
                            this.busses[j] = bussesTemp[i];
                        }
                    }
                }
            }
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

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Fehler beim Laden: " + request.error);
            }
            else
            {
                // Parse json return
                JSONNode data = JSON.Parse(request.downloadHandler.text);
                this.busses = new List<Bus>();
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
