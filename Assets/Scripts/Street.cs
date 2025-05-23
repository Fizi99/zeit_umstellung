using System.Collections.Generic;
using UnityEngine;

public class Street : MonoBehaviour
{
    [HideInInspector]
    public List<Vector3> nodes = new List<Vector3>();
    [HideInInspector]
    public List<Transform> waypoints = new List<Transform>();
    private GameManager gameManager;

    [HideInInspector]
    public Vector3 closestPointOnStreet;
    [HideInInspector]
    public double distance;

    private void Start()
    {
         this.gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    // draw nodes belonging to street and place waypoints, to be used for enemy pathfinding
    public void DrawNodes()
    {
        foreach(Vector3 node in nodes)
        {

            GameObject waypoint = new GameObject("waypoint");
            waypoint.transform.position = node;
            this.waypoints.Add(waypoint.transform);
            /*GameObject point = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            point.transform.position = node;
            point.transform.localScale = Vector3.one * 5;

            // Optional: Farbe setzen
            var renderer = point.GetComponent<Renderer>();
            if (renderer != null)
            {
                Material mat = new Material(Shader.Find("Standard"));
                mat.color = Color.magenta;
                renderer.material = mat;
            }
            point.name = $"Point_{node.x}_{node.y}_{node.z}";
            //Instantiate(point);*/
        }
    }

    // draw closest point on street to bus stop, to check, if bus stop is at street
    public void GetClosestPointToBusStop()
    {
        // for some reason have to set gamemanager here??
        this.gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        double shortestDistance = 99999;
        for(int i = 0; i < nodes.Count-1; i++)
        {
       
            Vector3 point = this.gameManager.GetClosestPointToBusStop(Vector3.zero, nodes[i], nodes[i + 1]);
            double distance = (point - Vector3.zero).magnitude;
            if (distance < shortestDistance)
            {
                shortestDistance = distance;
                this.distance = distance;
                this.closestPointOnStreet = point;
            }
        }

       

        //Debug.DrawLine(Vector3.zero, this.closestPointOnStreet, Color.red);
    }



}
