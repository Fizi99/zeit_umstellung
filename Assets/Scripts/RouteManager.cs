using System.Collections.Generic;
using UnityEngine;

public class RouteManager : MonoBehaviour
{
    [HideInInspector]
    public Transform lastWaypoint;
    [HideInInspector]
    public Vector3 lastWaypointPos;
    private GameManager gameManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // create a waypoint close to the bus stop and add it to all streets as final destination
    public void AddLastWaypointToRoutes(List<GameObject> streets)
    {

        if(this.lastWaypoint != null)
        {
            Destroy(this.lastWaypoint.gameObject);
        }

        GameObject waypoint = new GameObject("waypoint");
        waypoint.transform.position = this.lastWaypointPos;
        this.lastWaypoint = waypoint.transform;

        foreach(GameObject street in streets)
        {
            street.GetComponent<Street>().waypoints.Add(this.lastWaypoint);
        }

    }

    // get closest point to a line, draw connection line and return the distance
    public Vector3 GetClosestPointOnLine(Vector3 p, Vector3 a, Vector3 b)
    {
        // Vektoren für die Projektionsformel
        Vector3 ab = b - a;
        Vector3 ap = p - a;
        float t = Vector3.Dot(ap, ab) / Vector3.Dot(ab, ab);
        t = Mathf.Clamp01(t);

        // closest point on line
        Vector3 closestPoint = a + t * ab;

        // distance Vec and distance to point
        Vector3 diff = p - closestPoint;
        double distance = diff.magnitude;

        //Debug.DrawLine(p, closestPoint, Color.red);
        return closestPoint;
    }

    // TODO: schauen, welche straßen miteinander verbunden sind um so ein zusammenhängendes straßennetz zu bauen?
    public void GetConnectedStreets()
    {

    }

}
