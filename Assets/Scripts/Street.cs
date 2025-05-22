using System.Collections.Generic;
using UnityEngine;

public class Street : MonoBehaviour
{
    public List<Vector3> nodes = new List<Vector3>();
    public List<Transform> waypoints = new List<Transform>();

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

}
