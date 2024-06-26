using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Graph : MonoBehaviour, ISerializationCallbackReceiver
{
    public static Graph Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        // finally, we check the waypoint adjacencies to make sure they're correct
        foreach (Waypoint wp in Object.FindObjectsOfType<Waypoint>())
        {
            if (wp.isSubdivided || wp.isPedestrianOnly) continue;

            foreach (Waypoint adj in wp.adjacentWaypoints)
            {
                if (!adj.isSubdivided)
                {
                    Debug.LogWarning("Waypoint " + wp.name + " is adjacent to " + adj.name + " but they are not subdivided. Try rerunning sidewalk stuff.");
                }
            }
        }
    }

    public bool IsInitialised { get; private set; }

    // private waypointsize with getter
    [Range(0f, 2f)][SerializeField] private float waypointSize = 0.5f;

    // this contains a simple list of all edges
    [SerializeField] public List<Edge> allEdges;

    // this contains a list of all reduced edges
    [SerializeField] private List<ReducedEdge> reducedEdges;

    // this contains a dictionary of all edges, making them a lot faster to access
    // reduced edge is a simple struct containing two waypoints, so it can be used as a key
    // private Dictionary<ReducedEdge, Edge> edgesAsDict = new Dictionary<ReducedEdge, Edge>();

    public List<Building> allBuildings;
    public Dictionary<string, Building> buildings = new Dictionary<string, Building>();
    public Dictionary<BuildingType, List<Building>> buildingsByType = new Dictionary<BuildingType, List<Building>>();

    public List<Waypoint> waypoints;

    [SerializeField] private bool drawEdgeGizmos = true;

    [SerializeField] public bool inEditMode;

    void Start()
    {
        Random.InitState(42); // Set seed for random number generator
        Time.timeScale = 1; // Set time scale to 1
        IsInitialised = false;

        // Initialise buildings dictionary.
        foreach (BuildingType bType in BuildingProperties.buildingTypes)
        {
            buildingsByType.Add(bType, new List<Building>());
        }

        // Get list of waypoints and all buildings.
        waypoints = new List<Waypoint>(FindObjectsOfType<Waypoint>());
        allBuildings = new List<Building>(FindObjectsOfType<Building>());

        foreach (Building b in allBuildings)
        {
            buildingsByType[b.buildingType].Add(b);
            buildings.Add(b.GetComponent<UniqueID>().uniqueID, b);
        }
        
        if (!inEditMode && BarrierManager.Instance.loadBarriersFromSave)
        {
            BarrierManager.Instance.LoadBarriersFromSave();
        }

        IsInitialised = true;
    }

    // these methods are used to add, get, and reset edges.
    // we use these methods to ensure that the edgesAsDict dictionary is always up to date,
    // and that we don't have to worry about it getting out of sync with the allEdges list.
    public List<Edge> GetAllEdges()
    {
        return allEdges;
    }
    public void ResetEdges()
    {
        allEdges = new List<Edge>();
    }

    public void AddEdge(Edge edge)
    {
        ReducedEdge reducedEdge = edge.Reduce();
        allEdges.Add(edge);
        reducedEdges.Add(reducedEdge);
    }

    public Building getRandomBuildingByType(BuildingType buildingType)
    {
        return buildingsByType[buildingType][Random.Range(0, buildingsByType[buildingType].Count)];
    }

    public float WaypointSize
    {
        get { return waypointSize; }
    }

    public void OnBeforeSerialize()
    {
    }

    public void OnAfterDeserialize()
    {
        foreach (Edge edge in allEdges)
        {
            edge.BootstrapIntersectingEdges(this);
        }
    }

    private void OnDrawGizmos()
    {
        if (drawEdgeGizmos) // Check if drawing of edge gizmos is enabled
        {
            DrawEdgeGizmos();
        }
    }

    public float CalculateDistance(Waypoint a, Waypoint b)
    {
        return Vector3.Distance(a.transform.position, b.transform.position);
    }

    public float CalculateDistance(GameObject a, GameObject b)
    {
        return Vector3.Distance(a.transform.position, b.transform.position);
    }

    public Vector3 GetClosestPointToBuilding(GameObject building)
    {
        Vector3 buildingPosition = building.transform.position;

        float minDistance = float.MaxValue;
        Vector3 closestPoint = Vector3.zero;

        foreach (Edge edge in allEdges)
        {
            Vector3 point = edge.GetClosestPoint(buildingPosition);
            float distance = Vector3.Distance(point, buildingPosition);

            if (distance < minDistance)
            {
                minDistance = distance;
                closestPoint = point;
            }
        }

        return closestPoint;
    }

    private void DrawEdgeGizmos()
    {
        if (allEdges == null) return;
        foreach (Edge edge in allEdges)
        {
            edge.DrawGizmo();
        }
    }

    public Edge GetEdge(Waypoint startPoint, Waypoint endPoint)
    {
        foreach (Edge edge in allEdges)
        {
            if (edge.StartWaypoint == startPoint && edge.EndWaypoint == endPoint)
            {
                return edge;
            }
        }
        // Debug.LogWarning("Edge not found: " + a + " - " + b);
        return null;
    }
    public Edge GetEdge(ReducedEdge re)
    {
        return this.GetEdge(re.startWaypoint, re.endWaypoint);
    }

    public Edge getClosetRoadEdge(Vector3 position)
    {
        Edge closestEdge = null;
        float minDistance = float.MaxValue;

        foreach (Edge edge in allEdges)
        {
            if (edge.isPedestrianOnly) continue;
            if (edge.HasIntersecingEdges()) continue;

            float distance = edge.DistanceToEdge(position);

            if (distance < minDistance)
            {
                minDistance = distance;
                closestEdge = edge;
            }
        }

        return closestEdge;
    }

    public Edge getClosetPedestrianEdge(Vector3 position)
    {
        Edge closestEdge = null;
        float minDistance = float.MaxValue;

        foreach (Edge edge in allEdges)
        {
            if (edge.isPedestrianOnly)
            {
                float distance = edge.DistanceToEdge(position);

                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestEdge = edge;
                }
            }
        }

        return closestEdge;
    }

    public Building pickRandomBuilding()
    {
        return buildings.Values.ToList<Building>()[Random.Range(0, buildings.Count - 1)];
    }

    public string pickRandomBuildingID()
    {
        List<string> keys = new List<string>(buildings.Keys);
        int index = Random.Range(0, keys.Count);
        return keys[index];
    }
}
