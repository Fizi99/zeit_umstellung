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
    private GameObject routeContainer;
    [SerializeField]
    private GameObject routeVisualizerContainer;
    [SerializeField]
    private Material streetMat;

    [SerializeField]
    private GameObject boundingBoxForNodeLoading;
    private Vector3 boxCenter = Vector3.zero;
    private Vector3 boxSize = new Vector3(25, 15, 1);
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
        this.boxCenter = boundingBoxForNodeLoading.transform.position;
        this.boxSize = boundingBoxForNodeLoading.transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {

    }


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
        if (closestStreet != null)
        {
            this.lastWaypointPos = closestStreet.GetComponent<Street>().closestPointOnStreet;
            // add target node to closest street
            closestStreet.GetComponent<Street>().SetAsClosestStreet();
        }

    }

    // spawn waypoints for enemy ai. takes enemyspawnid list: is already culled for spawnradius
    public void SpawnWaypoints()
    {
        // reset waypoints
        foreach (List<GameObject> oldRoute in this.enemyRouteWaypoints)
        {
            foreach (GameObject oldWaypoint in oldRoute)
            {
                Destroy(oldWaypoint);
            }
        }

        this.enemyRouteWaypoints = new List<List<GameObject>>();
        foreach (List<long> route in this.enemyRouteIds)
        {

            List<GameObject> newRoute = new List<GameObject>();
            foreach (long node in route)
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

        foreach (GameObject routeVisualizer in this.enemyRouteVisualizer)
        {
            Destroy(routeVisualizer);
        }

        this.enemyRouteVisualizer = new List<GameObject>();

        foreach (KeyValuePair<long, List<long>> enemyroute in routes)
        {
            // reverse List and add them as routes for use in enemy scripts
            List<long> singleRoute = new List<long>();
            for (int i = enemyroute.Value.Count - 1; i >= 0; i--)
            {
                singleRoute.Add(enemyroute.Value[i]);
            }

            this.enemyRouteIds.Add(singleRoute);

            // draw route
            GameObject route = new GameObject("Route");
            LineRenderer lr = route.AddComponent<LineRenderer>();
            lr.material = this.streetMat;
            lr.widthMultiplier = 0.2f;
            lr.positionCount = enemyroute.Value.Count;
            lr.textureMode = LineTextureMode.Tile;
            //lr.startColor = Color.red;
            //lr.endColor = Color.blue;
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
                if (IsNodeInBounds(this.gameManager.nodeLocationDictionary[street.nodeIds[j]]))
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
                        if (IsNodeInBounds(this.gameManager.nodeLocationDictionary[street.nodeIds[j - 1]]))
                        {
                            float cost = (this.gameManager.nodeLocationDictionary[street.nodeIds[j]] - this.gameManager.nodeLocationDictionary[street.nodeIds[j - 1]]).magnitude;
                            graph[street.nodeIds[j]].Add(new Edge(street.nodeIds[j], street.nodeIds[j - 1], cost));
                        }

                    }
                    if (j != street.nodeIds.Count - 1)
                    {
                        // check first, if node is cutoff point and node should be handled as leaf, when node after is out of bounds
                        if (IsNodeInBounds(this.gameManager.nodeLocationDictionary[street.nodeIds[j + 1]]))
                        {
                            float cost = (this.gameManager.nodeLocationDictionary[street.nodeIds[j]] - this.gameManager.nodeLocationDictionary[street.nodeIds[j + 1]]).magnitude;
                            graph[street.nodeIds[j]].Add(new Edge(street.nodeIds[j], street.nodeIds[j + 1], cost));
                        }

                    }
                }


            }
        }
    }

    // check if node is in bounds of box.
    private bool IsNodeInBounds(Vector3 node)
    {
        //return ((node - Vector3.zero).magnitude) < this.spawnRadius;

        Vector3 halfSize = this.boxSize * 0.5f;
        Vector3 min = this.boxCenter - halfSize;
        Vector3 max = this.boxCenter + halfSize;

        return (node.x >= min.x && node.x <= max.x) &&
               (node.y >= min.y && node.y <= max.y) &&
               (node.z >= min.z && node.z <= max.z);

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