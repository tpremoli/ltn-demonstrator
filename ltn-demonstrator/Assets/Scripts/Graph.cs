using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Graph : MonoBehaviour
{
    public static Graph Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    // private waypointsize with getter
    [Range(0f, 2f)][SerializeField] private float waypointSize = 0.5f;
    [SerializeField] public List<Edge> edges;

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

        foreach (KeyValuePair<BuildingType, List<Building>> t in buildingsByType)
        {
            Debug.Log("Building Type: " + t.Key + ", Total: " + t.Value.Count);
        }

        if (!inEditMode && BarrierManager.Instance.loadBarriersFromSave){
            BarrierManager.Instance.RecalcBarriersOnEdges();
        }

    }

    public Building getRandomBuildingByType(BuildingType buildingType)
    {
        return buildingsByType[buildingType][Random.Range(0, buildingsByType[buildingType].Count)];
    }

    public float WaypointSize
    {
        get { return waypointSize; }
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

        foreach (Edge edge in edges)
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
        foreach (Edge edge in edges)
        {
            edge.DrawGizmo();
        }
    }

    public Edge getEdge(Waypoint a, Waypoint b)
    {
        foreach (Edge edge in edges)
        {
            if (edge.StartWaypoint == a && edge.EndWaypoint == b)
            {
                return edge;
            }
        }

        return null;
    }

    public Edge getClosetRoadEdge(Vector3 position)
    {
        Edge closestEdge = null;
        float minDistance = float.MaxValue;

        foreach (Edge edge in edges)
        {
            if (edge.isPedestrianOnly) continue;
            
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

        foreach (Edge edge in edges)
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

    public Edge GetEdge(Waypoint startPoint, Waypoint endPoint)
    {
        foreach (Edge edge in edges)
        {
            if (edge.StartWaypoint == startPoint && edge.EndWaypoint == endPoint)
            {
                return edge;
            }
        }

        return null;
    }

    public Building pickRandomBuilding() {
        return buildings.Values.ToList<Building>()[Random.Range(0, buildings.Count)];
    }
}
