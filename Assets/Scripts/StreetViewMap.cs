using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using SimpleJSON;

public class StreetViewMap : MonoBehaviour
{
    [SerializeField] private double searchRadius = 0.001;
    [SerializeField] private double searchCenterLat = 52.5200;
    [SerializeField] private double searchCenterLon = 13.4040;

    private List<GameObject> streets = new List<GameObject>();

    // Beispielkoordinaten (Berlin)
    private double minLat = 0;
    private double minLon = 0;
    private double maxLat = 0;
    private double maxLon = 0;

    void Start()
    {
        StartCoroutine(DownloadOSMData());
    }

    public void SearchStreetsAroundCenter(double searchCenterLat, double searchCenterLon)
    {
        this.searchCenterLat = searchCenterLat;
        this.searchCenterLon = searchCenterLon;

        StartCoroutine(DownloadOSMData());
    }

    IEnumerator DownloadOSMData()
    {
        this.minLat = searchCenterLat - searchRadius;
        this.minLon = searchCenterLon - searchRadius;
        this.maxLat = searchCenterLat + searchRadius;
        this.maxLon = searchCenterLon + searchRadius;
        // so toString method converts decimal point correctly (1.2 instead of 1,2)
        string minLatString = minLat.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("en-US"));
        string minLonString = minLon.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("en-US"));
        string maxLatString = maxLat.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("en-US"));
        string maxLonString = maxLon.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("en-US"));

        // WebRequest to Open Street View as JSON in specified area
        string url = "https://overpass-api.de/api/interpreter?data=[out:json];way['highway'](" + minLatString +","+ minLonString + ","+ maxLatString + ","+ maxLonString + ");(._;>;);out;";

        UnityWebRequest www = UnityWebRequest.Get(url);
        yield return www.SendWebRequest();
        if (www.result == UnityWebRequest.Result.Success)
        {
            JSONNode data = JSON.Parse(www.downloadHandler.text);
            Dictionary<long, Vector3> nodes = new Dictionary<long, Vector3>();

            // loop through nodes
            foreach (JSONNode element in data["elements"].AsArray)
            {
                if (element["type"] == "node")
                {
                    long id = element["id"].AsLong;
                    double lat = element["lat"].AsDouble;
                    double lon = element["lon"].AsDouble;
                    Vector3 pos = LatLonToUnity(lat, lon);
                    nodes[id] = pos;
                }
            }

            foreach (GameObject go in this.streets)
            {
                Destroy(go);
            }

            // Streets as lines
            this.streets = new List<GameObject>();
            foreach (JSONNode element in data["elements"].AsArray)
            {
                if (element["type"] == "way")
                {
                    GameObject road = new GameObject("Road");
                    LineRenderer lr = road.AddComponent<LineRenderer>();
                    lr.material = new Material(Shader.Find("Legacy Shaders/Particles/Alpha Blended Premultiply"));
                    lr.widthMultiplier = 0.2f;
                    lr.positionCount = element["nodes"].Count;
                    lr.startColor = Color.blue;
                    lr.endColor = Color.blue;

                    for (int i = 0; i < element["nodes"].Count; i++)
                    {
                        long nodeId = element["nodes"][i].AsLong;
                        lr.SetPosition(i, nodes[nodeId]);
                    }

                    this.streets.Add(road);
                }
            }
        }
        else
        {
            Debug.LogError("Fehler beim Abrufen der OSM-Daten: " + www.error);
        }
    }

    // GPS Data in Unity units
    Vector3 LatLonToUnity(double lat, double lon)
    {
        float scale = 10000f;
        float x = (float)((lon - minLon) * scale);
        float z = (float)((lat - minLat) * scale);
        return new Vector3(x, 0, z);
    }
}
