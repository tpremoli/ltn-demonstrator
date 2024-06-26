using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

// This is here for future expansion, but is not currently used
public enum BuildingType
{
    Generic,
    Residence,
    Office,
    Restaurant,
    Shop,
    ThroughTrafficDummy
}

public class Building : MonoBehaviour
{
    // DEBUG ATTRIBUUTES
    private static bool ALLOW_SPAWNING = false;

    // Private attributes
    [SerializeField] private int vehicleMax;
    [SerializeField] private int occupantMax;
    private Graph graph;
    public static Dictionary<BuildingType, float> destinationWeights; // Distribution for destination types

    // the spawn probability should be based on the building type and maximum number of occupants.
    // as it stands, it is a constant value, but it should be a function/enum of the building type
    [Range(0f, 1f)][SerializeField] private float spawnProbability = 0.1f;
    [Range(1, 600)][SerializeField] private float timeBetweenSpawns = 1; // The time between spawn attempts
    private float nextSpawnTime; // The time of the next spawn attempt

    public Edge closestRoadEdge;
    public Vector3 closestPointOnRoadEdge;

    public Edge closestPedestrianEdge { get; private set; } = null;
    public Vector3 closestPointOnPedestrianEdge { get; private set; } = Vector3.zero;

    // Some more attributes - not sure if needed, but seemed useful
    public readonly string buildingName;    // the name of the building (i.e "the X residence". Would be fun to have a random name generator?)
    [SerializeField] public BuildingType buildingType;// the type of the building (i.e "residence", "office", "restaurant", etc. would be an enum)

    // Start is called before the first frame update. We use these to initialize the building.
    void Start()
    {
        this.graph = Graph.Instance;

        //buildingType = BuildingProperties.buildingTypes[Random.Range(0, BuildingProperties.buildingTypes.Count)];

        // I don't want to hardcode these values, but I'm not sure how to do it otherwise.
        // if this is removed, the building will spam vehicles
        this.timeBetweenSpawns = 1;
        this.spawnProbability = 0.05f;
        this.nextSpawnTime = Time.time + timeBetweenSpawns;

        this.closestRoadEdge = graph.getClosetRoadEdge(this.transform.position);
        this.closestPointOnRoadEdge = closestRoadEdge.GetClosestPoint(this.transform.position);

        this.closestPedestrianEdge = graph.getClosetPedestrianEdge(this.transform.position);
        if (this.closestPedestrianEdge != null)
        {
            this.closestPointOnPedestrianEdge = closestPedestrianEdge.GetClosestPoint(this.transform.position);
        }
        else
        {
            Debug.Log("No Pedestrian Edge Found");
        }

        Debug.Log("Building Instantiated");
    }

    private void OnDrawGizmos()
    {
        if (Application.IsPlaying(this))
        {
            if (closestRoadEdge != null)
            {

                Gizmos.color = Color.green;
                Gizmos.DrawLine(this.transform.position, closestPointOnRoadEdge);
            }

            if (closestPedestrianEdge != null)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(this.transform.position, closestPointOnPedestrianEdge);
            }
        }

        if (buildingType == BuildingType.ThroughTrafficDummy)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(transform.position, new Vector3(2, 2, 2));
        }
    }

    public Vector3 CalcClosestPointOnEdge()
    {
        Graph tempGraph = GameObject.Find("Graph").GetComponent<Graph>();

        Edge closestEdge = tempGraph.getClosetRoadEdge(this.transform.position);
        Vector3 closestPointOnEdge = closestEdge.GetClosestPoint(this.transform.position);
        return closestPointOnEdge;
    }

    // Update is called once per frame
    void Update()
    {
        // Using time-based spawning- there is a spawnProbability chance of spawning a 
        // vehicle every timeBetweenSpawns seconds. Only spawn if the maximum number of
        // vehicles has not been reached.
        // && VehicleList.Count < vehicleMax  
        if (Time.time >= nextSpawnTime && !graph.inEditMode) // Check if it is time to spawn a vehicle
        {
            // Spawn a vehicle, if the random number is less than the spawn probability
            if (Random.value < spawnProbability)
            {
                //Debug.Log("Spawning traveller");
                SpawnTraveller();
            }
            // Set the next spawn time
            nextSpawnTime = Time.time + timeBetweenSpawns;
        }

    }

    // Spawn method
    public void SpawnTraveller()
    {
        if (!ALLOW_SPAWNING) return;
        Debug.Log("Spawning traveller");
        // We need a prefab for the traveller. This is a template from which we can create new travellers.
        //update the traveller count
        TravellerManager.Instance.noOfTravellers += 1;
        // The prefab should have a Traveller component attached to it.
        GameObject travellerPrefab = Resources.Load<GameObject>("Traveller");
        GameObject travellerManager = TravellerManager.Instance.gameObject;
        GameObject newTravellerObj = Instantiate(travellerPrefab, travellerManager.transform);
        newTravellerObj.GetComponent<WaypointMover>().setOriginBuilding(this);
        //stops spawning if max number of travellers reached
    }



    // public Vector3 getEdgeLocation()
    // {
    //     return edge.getPointOnEdge(positionOnEdge);
    // }

    // getters and setters
    // Get the maximum number of vehicles
    public int GetVehicleMax()
    {
        return vehicleMax;
    }

    // Get the maximum number of occupants
    public int GetOccupantMax()
    {
        return occupantMax;
    }

}