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
    /*public void AddLastWaypointToRoutes(List<GameObject> streets)
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
            street.GetComponent<Street>().AddWaypoint(this.lastWaypointPos);
        }

    }*/

    // calculate closest point to bus stop and draw that point as sphere
    public void CalcClosestStreetToBusStop()
    {
        // claculate closest point to bus stop for every street
        foreach (GameObject street in this.gameManager.streets)
        {
            street.GetComponent<Street>().GetClosestPointToBusStop();
        }

        // choose closest street
        GameObject closestStreet = null;
        double shortestDistance = 9999999;
        foreach (GameObject street in this.gameManager.streets)
        {
            if (street.GetComponent<Street>().distance < shortestDistance)
            {
                closestStreet = street;
                shortestDistance = street.GetComponent<Street>().distance;
            }

        }


        this.lastWaypointPos = closestStreet.GetComponent<Street>().closestPointOnStreet;

        // draw closest point on closest street as sphere
        GameObject point = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        point.transform.position = closestStreet.GetComponent<Street>().closestPointOnStreet;
        point.transform.localScale = Vector3.one * 0.2f;

        var renderer = point.GetComponent<Renderer>();
        if (renderer != null)
        {
            Material mat = new Material(Shader.Find("Standard"));
            mat.color = Color.magenta;
            renderer.material = mat;
        }

        GetConnectedStreets();

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
        for (int i = 0; i < this.gameManager.streets.Count; i++)
        {
            for (int j = i+1; j < this.gameManager.streets.Count; j++)
            {
                if(i != j)
                {
                    Street street1 = this.gameManager.streets[i].GetComponent<Street>();
                    Street street2 = this.gameManager.streets[j].GetComponent<Street>();

                    if (ConnectedViaNode(street1, street2, out Vector3 connection1)){
                        // draw connection point as green sphere
                        GameObject point = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                        point.transform.position = connection1;
                        point.transform.localScale = Vector3.one * 0.2f;

                        var renderer = point.GetComponent<Renderer>();
                        if (renderer != null)
                        {
                            Material mat = new Material(Shader.Find("Standard"));
                            mat.color = Color.green;
                            renderer.material = mat;
                        }
                    }
                    // TODO: can be deletet since connection is always via node
                    /*else if(ConnectedViaIntersection(street1, street2, out Vector3 connection2))
                    {
                        // draw connection point as yellow sphere
                        GameObject point = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                        point.transform.position = connection2;
                        point.transform.localScale = Vector3.one * 0.2f;

                        var renderer = point.GetComponent<Renderer>();
                        if (renderer != null)
                        {
                            Material mat = new Material(Shader.Find("Standard"));
                            mat.color = Color.green;
                            renderer.material = mat;
                        }
                    }*/
                 
                }

            }
        }
    }

    // check if 2 streets are connected via node, or 2 nodes are really close
    public bool ConnectedViaNode(Street street1, Street street2, out Vector3 connection)
    {
        connection = Vector3.zero;
        foreach (Vector3 node1 in street1.nodes)
        {
            foreach (Vector3 node2 in street2.nodes)
            {
                if ((node1 - node2).magnitude < 0.2)
                {
                    connection = node1;
                    Debug.Log("connected via nodes");
                    return true;
                }
            }
        }

        return false;

    }

    // check if 2 streets intersect
    public bool ConnectedViaIntersection(Street street1, Street street2, out Vector3 connection)
    {
        connection = Vector3.zero;
        for (int i = 0; i < street1.nodes.Count - 1; i++)
        {
            for (int j = 0; j < street2.nodes.Count - 1; j++)
            {
                // Ausgabe initialisieren
                Vector3 intersection = Vector3.zero;

                Vector3 A = street1.nodes[i];
                Vector3 B = street1.nodes[i+1];
                Vector3 C = street2.nodes[j];
                Vector3 D = street2.nodes[j + 1];

                // Richtungsvektoren und Hilfsgrößen
                Vector3 u = B - A;           // Richtung Segment 1
                Vector3 v = D - C;           // Richtung Segment 2
                Vector3 w = A - C;           // Verbindung A->C

                float a = Vector3.Dot(u, u); // |u|²
                float b = Vector3.Dot(u, v); // u·v
                float c = Vector3.Dot(v, v); // |v|²
                float d = Vector3.Dot(u, w); // u·w
                float e = Vector3.Dot(v, w); // v·w

                float denom = a * c - b * b; // Nenner des Cramersystems
                const float EPS = 1e-8f;

                float s, t;                  // Parameter auf den Geraden (0..1 -> im Segment)

                if (denom < EPS)             // (Fast) parallele Geraden
                {
                    s = 0.0f;
                    t = (b > c ? d / b : e / c);
                }
                else
                {
                    s = (b * e - c * d) / denom;
                    t = (a * e - b * d) / denom;
                }

                // Auf Segmentgrenzen beschränken
                s = Mathf.Clamp01(s);
                t = Mathf.Clamp01(t);

                // Nächste Punkte auf den beiden Segmenten bestimmen
                Vector3 pAB = A + s * u;
                Vector3 pCD = C + t * v;

                // Mittelpunkt der kürzesten Verbindung als "Schnittpunkt" zurückgeben
                intersection = 0.5f * (pAB + pCD);

                // Tatsächlicher Abstand
                float distance = Vector3.Distance(pAB, pCD);

                if(distance <= 0.01)
                {
                    connection = intersection;
                    Debug.Log("cennected via intersection");
                    return true;
                }

            }
        }
        return false;

    }
}
