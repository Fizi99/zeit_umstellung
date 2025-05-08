using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.Text.RegularExpressions;
using SimpleJSON;
using System.Collections.Generic;

public class BusTimeScraper : MonoBehaviour
{
    [SerializeField] private string busstop = "Technische-Hochschule";
    private int hafas = 9057765;
    private string busstopActual = "";
    // URL to fetch hafas of specific bus stop
    private string busStopSearchURL = "https://netzplan.swhl.de/api/v1/stationboards/hafas/9057765";
    // URL to fetch specific busstop with hafas
    private string busStopURL = "";

    private List<Bus> busses;


    void Start()
    {
        this.busStopSearchURL = "https://swhl.vercel.app/api/stops?search="+ this.busstop +"&luebeckOnly=true";
        StartCoroutine(FetchBusStopHafas());
   
    }

    // Load for specific busstop
    public void SearchBusStop(string busstop)
    {
        this.busstop = busstop;
        this.busStopSearchURL = "https://swhl.vercel.app/api/stops?search=" + this.busstop + "&luebeckOnly=true";
        StartCoroutine(FetchBusStopHafas());
    }

    // Get hafas number for searched busstop
    IEnumerator FetchBusStopHafas()
    {
        // fetch search result for busstop
        UnityWebRequest request = UnityWebRequest.Get(this.busStopSearchURL);
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
                        this.hafas = data["data"][i]["extId"];
                        this.busstopActual = data["data"][i]["name"];
                    }
                }
                Debug.Log(this.busstopActual+ ": " +this.hafas);

                StartCoroutine(FetchBusStopInformation());
            }
        }
    }

    // Fetch Busstop information like departure of each bus
    IEnumerator FetchBusStopInformation()
    {
        this.busStopURL = "https://netzplan.swhl.de/api/v1/stationboards/hafas/" + this.hafas;

        Debug.Log(this.hafas);

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
                    Debug.Log(this.busses[i].line + ": Richtung: " + this.busses[i].headsign + " kommt um: " + System.DateTimeOffset.FromUnixTimeSeconds(this.busses[i].realtime).LocalDateTime.TimeOfDay);
                    
                    
                }
            }
        }
    }

}
