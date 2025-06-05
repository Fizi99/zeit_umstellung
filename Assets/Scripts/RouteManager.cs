using System.Collections.Generic;
using UnityEngine;

public class RouteManager : MonoBehaviour
{
    [HideInInspector]
    public Transform lastWaypoint;
    [HideInInspector]
    public Vector3 lastWaypointPos;
    private GameManager gameManager;

    [SerializeField]
    private int spawnRadius = 10;

    [SerializeField]
    private GameObject routeContainer;
    [SerializeField]
    private GameObject routeVisualizerContainer;

    [HideInInspector]
    public List<List<long>> enemyRouteIds = new List<List<long>>();
    [HideInInspector]
    public List<List<GameObject>> enemyRouteWaypoints = new List<List<GameObject>>();
    [HideInInspector]
    public List<GameObject> enemyRouteVisualizer = new List<GameObject>();
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


    public void InitiateRouteManagement()
    {
        CalcClosestStreetToBusStop();
        CalculateRoutes();
        SpawnWaypoints();
    }

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
        if( closestStreet != null)
        {
            this.lastWaypointPos = closestStreet.GetComponent<Street>().closestPointOnStreet;
            // add target node to closest street
            closestStreet.GetComponent<Street>().SetAsClosestStreet();
        }


        // draw closest point on closest street as sphere
        //GameObject point = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        //point.transform.position = closestStreet.GetComponent<Street>().closestPointOnStreet;
        //point.transform.localScale = Vector3.one * 0.2f;

        //var renderer = point.GetComponent<Renderer>();
        //if (renderer != null)
        //{
        //    Material mat = new Material(Shader.Find("Standard"));
        //    mat.color = Color.magenta;
        //    renderer.material = mat;
        //}

        
        //GetConnectedStreets();

    }

    // spawn waypoints for enemy ai. takes enemyspawnid list: is already culled for spawnradius
    public void SpawnWaypoints()
    {
        // reset waypoints
        foreach(List<GameObject> oldRoute in this.enemyRouteWaypoints)
        {
            foreach(GameObject oldWaypoint in oldRoute)
            {
                Destroy(oldWaypoint);
            }
        }

        this.enemyRouteWaypoints = new List<List<GameObject>>();
        foreach(List<long> route in this.enemyRouteIds)
        {

            List<GameObject> newRoute = new List<GameObject>();
            foreach(long node in route)
            {
       
                GameObject waypoint = new GameObject("Waypoint");
                waypoint.transform.position = this.gameManager.nodeLocationDictionary[node];
                waypoint.transform.parent = this.routeContainer.transform;
                Instantiate(waypoint);
                newRoute.Add(waypoint);
            }

            this.enemyRouteWaypoints.Add(newRoute);
        }

        this.gameManager.routes = this.enemyRouteWaypoints;
    }


    private void CalculateRoutes()
    {
        Dictionary<long, List<Edge>> graph = new Dictionary<long, List<Edge>>();
        FillDictionaryWithStreets(graph);

        // traverse graph to find routes
        Dictionary<long, List<long>> routes = FindAllPaths(graph, 1);

        // reset id list
        this.enemyRouteIds = new List<List<long>>();

        foreach(GameObject routeVisualizer in this.enemyRouteVisualizer)
        {
            Destroy(routeVisualizer);
        }

        this.enemyRouteVisualizer = new List<GameObject>();

        foreach (KeyValuePair<long, List<long>> enemyroute in routes)
        {
            // reverse List and add them as routes for use in enemy scripts
            List<long> singleRoute = new List<long>();
            for (int i = enemyroute.Value.Count-1; i >= 0; i--)
            {
                singleRoute.Add(enemyroute.Value[i]);
            }

            // only add routes for enemies, if they are inside spawn radius! TODO: cull not needed nodes in beginning, before traversing graph.
            //float distanceFromCenter = (this.gameManager.nodeLocationDictionary[singleRoute[0]] - Vector3.zero).magnitude;
            //if(distanceFromCenter < spawnRadius)
            //{
                this.enemyRouteIds.Add(singleRoute);
            //}

            // draw route
            GameObject route = new GameObject("Route");
            LineRenderer lr = route.AddComponent<LineRenderer>();
            lr.material = new Material(Shader.Find("Legacy Shaders/Particles/Alpha Blended Premultiply"));
            lr.widthMultiplier = 0.2f;
            lr.positionCount = enemyroute.Value.Count;
            lr.startColor = Color.red;
            lr.endColor = Color.blue;
            for (int i = 0; i < enemyroute.Value.Count; i++)
            {
                lr.SetPosition(i, this.gameManager.nodeLocationDictionary[enemyroute.Value[i]]);
            }
            route.transform.parent = this.routeVisualizerContainer.transform;
            this.enemyRouteVisualizer.Add(route);
            
        }

    }

    // fill the dictionary for all nodes with neighbour nodes of that node
    public void FillDictionaryWithStreets(Dictionary<long, List<Edge>> graph)
    {
        for (int i = 0; i < this.gameManager.streets.Count; i++)
        {
            Street street = this.gameManager.streets[i].GetComponent<Street>();
            for (int j = 0; j < street.nodeIds.Count; j++)
            {
                // cull nodes that are too far away, so they are not traversed and enemies dont spawn
                float distanceFromCenter = (this.gameManager.nodeLocationDictionary[street.nodeIds[j]] - Vector3.zero).magnitude;
                if(distanceFromCenter < spawnRadius)
                {
                    // create a new list of connected nodes to currently watched list, if no entry exists
                    if (!graph.ContainsKey(street.nodeIds[j]))
                    {
                        graph[street.nodeIds[j]] = new List<Edge>();
                    }

                    // add neighbour nodes in street to dictionary at key of currently watched node
                    if (j != 0)
                    {
                        // check first, if node is cutoff point and node should be handled as leaf, when node before is out of bounds
                        float distanceFromCenterForPrev = (this.gameManager.nodeLocationDictionary[street.nodeIds[j-1]] - Vector3.zero).magnitude;
                        if (distanceFromCenterForPrev < spawnRadius)
                        {
                            float cost = (this.gameManager.nodeLocationDictionary[street.nodeIds[j]] - this.gameManager.nodeLocationDictionary[street.nodeIds[j - 1]]).magnitude;
                            graph[street.nodeIds[j]].Add(new Edge(street.nodeIds[j], street.nodeIds[j - 1], cost));
                        }
                            
                    }
                    if (j != street.nodeIds.Count - 1)
                    {
                        // check first, if node is cutoff point and node should be handled as leaf, when node after is out of bounds
                        float distanceFromCenterForNext = (this.gameManager.nodeLocationDictionary[street.nodeIds[j + 1]] - Vector3.zero).magnitude;
                        if (distanceFromCenterForNext < spawnRadius)
                        {
                            float cost = (this.gameManager.nodeLocationDictionary[street.nodeIds[j]] - this.gameManager.nodeLocationDictionary[street.nodeIds[j + 1]]).magnitude;
                            graph[street.nodeIds[j]].Add(new Edge(street.nodeIds[j], street.nodeIds[j + 1], cost));
                        }
                               
                    }
                }
                
                    
            }
        }
    }

    public Dictionary<long, List<long>> FindAllPaths(Dictionary<long, List<Edge>> graph, long start)
    {

        long goalId = start;
        DijkstraFromGoal(graph, goalId,
                                    out var dist,
                                    out var prev);

        List<long> leafNodes = new List<long>();
        foreach (KeyValuePair<long, List<Edge>> pair in graph)
        {
            
            if (pair.Value.Count <= 1)
            {
                leafNodes.Add(pair.Key);
            }
        }

        Dictionary<long, List<long>> shortestPaths = new Dictionary<long, List<long>>();

        foreach (long leaf in leafNodes)
        {
            List<long> path = ReconstructPath(leaf, goalId, prev);
            if (path != null)
                shortestPaths[leaf] = path;   // LeafID → Pfad
        }

        return shortestPaths;
    }



    // TODO: schauen, welche straßen miteinander verbunden sind um so ein zusammenhängendes straßennetz zu bauen?
    /*public void GetConnectedStreets()
    {
        for (int i = 0; i < this.gameManager.streets.Count; i++)
        {
            for (int j = i + 1; j < this.gameManager.streets.Count; j++)
            {
                if (i != j)
                {
                    Street street1 = this.gameManager.streets[i].GetComponent<Street>();
                    Street street2 = this.gameManager.streets[j].GetComponent<Street>();

                    if (ConnectedViaNodeId(street1, street2, out Vector3 connection1))
                    {
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
                    /*else if (ConnectedViaNode(street1, street2, out Vector3 connection2))
                    {
                        // draw connection point as green sphere
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
                    }

                }

            }
        }
    }*/
    /*
     * struktur: dictionary: node a: node b, node c...
                             node b: node a, node d...
     */
    /*public Dictionary<long, List<long>> ConnectedNode(Dictionary<long, List<long>> graph, long start)
    {
        if()
    }*/




    // check if nodes are connected via node id given by OSM API
    //public bool ConnectedViaNodeId(Street street1, Street street2, out Vector3 connection)
    //{
    //    connection = Vector3.zero;
    //    for (int i = 0; i < street1.nodeIds.Count; i++)
    //    {
    //        foreach (long node2 in street2.nodeIds)
    //        {
    //            if (street1.nodeIds[i] == node2)
    //            {
    //                connection = street1.nodes[i];
    //                Debug.Log("connected via node Id");
    //                return true;
    //            }
    //        }
    //    }

    //    return false;
    //}

    //// check if 2 streets are connected via node, or 2 nodes are really close
    //public bool ConnectedViaNode(Street street1, Street street2, out Vector3 connection)
    //{
    //    connection = Vector3.zero;
    //    foreach (Vector3 node1 in street1.nodes)
    //    {
    //        foreach (Vector3 node2 in street2.nodes)
    //        {
    //            if ((node1 - node2).magnitude < 0.2)
    //            {
    //                connection = node1;
    //                Debug.Log("connected via nodes");
    //                return true;
    //            }
    //        }
    //    }

    //    return false;

    //}

    //// check if 2 streets intersect
    //public bool ConnectedViaIntersection(Street street1, Street street2, out Vector3 connection)
    //{
    //    connection = Vector3.zero;
    //    for (int i = 0; i < street1.nodes.Count - 1; i++)
    //    {
    //        for (int j = 0; j < street2.nodes.Count - 1; j++)
    //        {
    //            // Ausgabe initialisieren
    //            Vector3 intersection = Vector3.zero;

    //            Vector3 A = street1.nodes[i];
    //            Vector3 B = street1.nodes[i + 1];
    //            Vector3 C = street2.nodes[j];
    //            Vector3 D = street2.nodes[j + 1];

    //            // Richtungsvektoren und Hilfsgrößen
    //            Vector3 u = B - A;           // Richtung Segment 1
    //            Vector3 v = D - C;           // Richtung Segment 2
    //            Vector3 w = A - C;           // Verbindung A->C

    //            float a = Vector3.Dot(u, u); // |u|²
    //            float b = Vector3.Dot(u, v); // u·v
    //            float c = Vector3.Dot(v, v); // |v|²
    //            float d = Vector3.Dot(u, w); // u·w
    //            float e = Vector3.Dot(v, w); // v·w

    //            float denom = a * c - b * b; // Nenner des Cramersystems
    //            const float EPS = 1e-8f;

    //            float s, t;                  // Parameter auf den Geraden (0..1 -> im Segment)

    //            if (denom < EPS)             // (Fast) parallele Geraden
    //            {
    //                s = 0.0f;
    //                t = (b > c ? d / b : e / c);
    //            }
    //            else
    //            {
    //                s = (b * e - c * d) / denom;
    //                t = (a * e - b * d) / denom;
    //            }

    //            // Auf Segmentgrenzen beschränken
    //            s = Mathf.Clamp01(s);
    //            t = Mathf.Clamp01(t);

    //            // Nächste Punkte auf den beiden Segmenten bestimmen
    //            Vector3 pAB = A + s * u;
    //            Vector3 pCD = C + t * v;

    //            // Mittelpunkt der kürzesten Verbindung als "Schnittpunkt" zurückgeben
    //            intersection = 0.5f * (pAB + pCD);

    //            // Tatsächlicher Abstand
    //            float distance = Vector3.Distance(pAB, pCD);

    //            if (distance <= 0.01)
    //            {
    //                connection = intersection;
    //                Debug.Log("cennected via intersection");
    //                return true;
    //            }

    //        }
    //    }
    //    return false;

    //}

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










    /// <summary>
    /// ////////////////DIJKSTRA
    /// </summary>



    private class MinHeap
    {
        private readonly List<(long node, double dist)> _data = new();

        public int Count => _data.Count;
        public void Enqueue(long node, double dist)
        {
            _data.Add((node, dist));
            HeapifyUp(_data.Count - 1);
        }

        public (long node, double dist) Dequeue()
        {
            var root = _data[0];
            var last = _data[^1];
            _data.RemoveAt(_data.Count - 1);
            if (_data.Count > 0)
            {
                _data[0] = last;
                HeapifyDown(0);
            }
            return root;
        }

        private void HeapifyUp(int i)
        {
            while (i > 0)
            {
                int p = (i - 1) >> 1;
                if (_data[i].dist >= _data[p].dist) break;
                (_data[i], _data[p]) = (_data[p], _data[i]);
                i = p;
            }
        }

        private void HeapifyDown(int i)
        {
            int n = _data.Count;
            while (true)
            {
                int l = (i << 1) + 1, r = l + 1, s = i;
                if (l < n && _data[l].dist < _data[s].dist) s = l;
                if (r < n && _data[r].dist < _data[s].dist) s = r;
                if (s == i) break;
                (_data[i], _data[s]) = (_data[s], _data[i]);
                i = s;
            }
        }
    }

    //------------------------------------------------------------------
    // Kürzeste Wege vom Ziel zu allen Knoten (Dijkstra)
    //------------------------------------------------------------------
    public void DijkstraFromGoal(
        Dictionary<long, List<Edge>> graph,
        long goalId,
        out Dictionary<long, double> dist,
        out Dictionary<long, long> prev)
    {
        dist = new Dictionary<long, double>();
        prev = new Dictionary<long, long>();

        var pq = new MinHeap();
        pq.Enqueue(goalId, 0);
        dist[goalId] = 0;

        while (pq.Count > 0)
        {
            var (u, d) = pq.Dequeue();
            if (d > dist[u]) continue; // veralteter Eintrag

            if (!graph.TryGetValue(u, out var edges)) continue;

            foreach (var e in edges)
            {
                long v = e.targetId;        // gerichtete Kante u→v
                double nd = d + e.cost;
                if (!dist.ContainsKey(v) || nd < dist[v])
                {
                    dist[v] = nd;
                    prev[v] = u;      // Vorgänger auf kürzestem Weg
                    pq.Enqueue(v, nd);
                }
            }
        }
    }

    //------------------------------------------------------------------
    // Pfad-Rekonstruktion von leaf → goal mittels prev-Dictionary
    //------------------------------------------------------------------
    public List<long> ReconstructPath(long leaf, long goal,
                                             Dictionary<long, long> prev)
    {
        var path = new List<long>();
        long cur = leaf;
        while (true)
        {
            path.Add(cur);
            if (cur == goal) break;
            if (!prev.TryGetValue(cur, out cur))
                return null; // kein Weg – sollte bei verbundenem Graph nie passieren
        }
        path.Reverse();
        return path;
    }
}

public class Edge
{
    public long startId;
    public long targetId;
    public float cost;
    public Edge(long startId, long targetId, float cost)
    {
        this.targetId = targetId;
        this.startId = startId;
        this.cost = cost;

    }
}