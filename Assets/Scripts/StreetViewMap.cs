using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using SimpleJSON;
using System.IO;

public class StreetViewMap : MonoBehaviour
{
    [SerializeField] private double searchRadius = 100;
    [SerializeField] private double searchCenterLat = 52.5200;
    [SerializeField] private double searchCenterLon = 13.4040;

    [SerializeField] private GameObject busStopPrefab;
    [SerializeField] private GameObject roadContainer;

    public bool loading = false;


    private GameObject busStop;
    private List<GameObject> streets = new List<GameObject>();

    private GameManager gameManager;

    void Start()
    {
        this.gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        //StartCoroutine(DownloadOSMData());
    }

    public void SearchStreetsAroundCenter(double searchCenterLat, double searchCenterLon)
    {
        this.searchCenterLat = searchCenterLat;
        this.searchCenterLon = searchCenterLon;

        foreach (GameObject go in this.streets)
        {
            Destroy(go);
        }
        this.streets = new List<GameObject>();

        this.loading = true;

        StartCoroutine(DownloadOSMData());
    }

    IEnumerator DownloadOSMData()
    {
  
        // so toString method converts decimal point correctly (1.2 instead of 1,2)
        string searchCenterLatStr = searchCenterLat.ToString("G", System.Globalization.CultureInfo.CreateSpecificCulture("en-US"));
        string searchCenterLonStr = searchCenterLon.ToString("G", System.Globalization.CultureInfo.CreateSpecificCulture("en-US"));
        string searchRadiusStr = searchRadius.ToString("G", System.Globalization.CultureInfo.CreateSpecificCulture("en-US"));

        // WebRequest to Open Street View as JSON in specified area
        string url = "https://overpass-api.de/api/interpreter?data=[out:json];way['highway'](around:" + searchRadiusStr + "," + searchCenterLatStr + "," + searchCenterLonStr + ");(._;>;);out;";

        UnityWebRequest www = UnityWebRequest.Get(url);
        yield return www.SendWebRequest();
        JSONNode data = null;
        if (www.result == UnityWebRequest.Result.Success)
        {
            //File.WriteAllTextAsync(Application.dataPath+"/DebugMapData.json", www.downloadHandler.text);
            data = JSON.Parse(www.downloadHandler.text);
            
        }
        else
        {
            Debug.LogError("Fehler beim Abrufen der OSM-Daten: " + www.error);
            // in case of connection error read json with layout around technische hochschule
            TextAsset jsonFile = Resources.Load<TextAsset>("DebugMapData");
            string fileContents = jsonFile.text;
            data = JSON.Parse(fileContents);

        }

        Dictionary<long, Vector3> nodes = new Dictionary<long, Vector3>();

        // add busstop as element with index 1 in dictionary
        nodes[1] = Vector3.zero;

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

        // dictionary with locations and node ids
        this.gameManager.nodeLocationDictionary = nodes;

        foreach (JSONNode element in data["elements"].AsArray)
        {
            if (element["type"] == "way")
            {
                GameObject road = new GameObject("Road");
                Street streetComponent = road.AddComponent<Street>();
                road.transform.parent = roadContainer.transform;

                for (int i = 0; i < element["nodes"].Count; i++)
                {
                    long nodeId = element["nodes"][i].AsLong;
                    streetComponent.nodes.Add(nodes[nodeId]);
                    streetComponent.nodeIds.Add(nodeId);
                }
                //streetComponent.DrawNodes();

                this.streets.Add(road);
            }
        }

        //DEBUG
        //ScaleStreetGrid(0.5f);

        this.gameManager.SetStreets(this.streets);
        this.loading = false;

        DrawBusStop();
        //StartCoroutine(QueryBusStopLocation());
    }

    // GPS Data in Unity units
    Vector3 LatLonToUnity(double lat, double lon)
    {
        float scale = 10000f;
        float x = (float)((lon - this.searchCenterLon) * scale);
        float z = (float)((lat - this.searchCenterLat) * scale);
        
        return new Vector3(x, z, 0);
    }

    // TODO: reicht das schon oder m�ssen street komponenten auch angepasst werden?
    public void ScaleStreetGrid(float scale)
    {
        Dictionary<long, Vector3> nodes = new Dictionary<long, Vector3>();
        foreach (KeyValuePair<long, Vector3> node in this.gameManager.nodeLocationDictionary)
        {
            nodes[node.Key] = node.Value * scale;
        }

        this.gameManager.nodeLocationDictionary = nodes;
        Debug.Log("scaled");
    }

    // Draw rectangle at bus stop location
    private void DrawBusStop()
    {

        if (this.busStop != null)
        {
            Destroy(this.busStop);
        }
        this.busStop = GameObject.Instantiate(busStopPrefab);
        this.busStop.transform.position = new Vector3(0, 0, 0);
        this.gameManager.UpdateBusStopGameObject(this.busStop);
    }

}