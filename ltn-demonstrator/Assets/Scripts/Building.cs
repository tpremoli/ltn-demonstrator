using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This is here for future expansion, but is not currently used
public enum BuildingType
{
    Residence,
    Office,
    Restaurant,
    Shop
}

public class Building : MonoBehaviour
{
    // Private attributes
    [SerializeField] private int vehicleMax;
    [SerializeField] private int occupantMax;
    [SerializeField] private Graph graph;
    private Dictionary<BuildingType, float> destinationWeights; // Distribution for destination types

    // the spawn probability should be based on the building type and maximum number of occupants.
    // as it stands, it is a constant value, but it should be a function/enum of the building type
    [Range(0f, 1f)] [SerializeField] private float spawnProbability;
    [Range(1, 600)] [SerializeField] private float timeBetweenSpawns; // The time between spawn attempts
    private float nextSpawnTime; // The time of the next spawn attempt

    // Public attributes
    // public List<Vehicle> VehicleList; // List of vehicles at the building.
    public int occupantCount;
    public Edge edge;               // Edge where the building is located
    public float positionOnEdge;    // how far down the edge our building is located

    // Some more attributes - not sure if needed, but seemed useful
    public readonly string buildingName;    // the name of the building (i.e "the X residence". Would be fun to have a random name generator?)
    public readonly BuildingType buildingType;// the type of the building (i.e "residence", "office", "restaurant", etc. would be an enum)

    // Start is called before the first frame update. We use these to initialize the building.
    void Start(){
        Random.InitState(42); // Set seed for random number generator
        this.vehicleMax = 2;
        this.occupantMax = 5;

        this.spawnProbability = 0.1f;
        this.timeBetweenSpawns = 1f;
        this.nextSpawnTime = 0f;

        // Assign these in the editor
        // this.edge = edge;
        // this.positionOnEdge = edgeLocation;

        // this.VehicleList = new List<Vehicle>();
        this.occupantCount = 0;
        this.destinationWeights = new Dictionary<BuildingType, float>();
        this.nextSpawnTime = Time.time + timeBetweenSpawns;

        Debug.Log("Building Instantiated");
    }

    private void OnDrawGizmos()
    {
        Gizmos.color  = Color.green;
        Vector3 buildingPosition = this.transform.position;
        Vector3 closestPoint = graph.GetClosestPointToBuilding(this.gameObject);
        Gizmos.DrawLine(buildingPosition, closestPoint);
    }

    // Update is called once per frame
    void Update()
    {
        // Using time-based spawning- there is a spawnProbability chance of spawning a 
        // vehicle every timeBetweenSpawns seconds. Only spawn if the maximum number of
        // vehicles has not been reached.
        if (Time.time >= nextSpawnTime) // && VehicleList.Count < vehicleMax  
        {
            // Spawn a vehicle, if the random number is less than the spawn probability
            if (Random.value < spawnProbability)
            {
                Debug.Log("Spawning traveller");
                SpawnTraveller();
            }
            // Set the next spawn time
            nextSpawnTime = Time.time + timeBetweenSpawns;
        }

    }

    // Spawn method
    public void SpawnTraveller()
    {
        Edge startingEdge = this.edge;

        // We need a prefab for the traveller. This is a template from which we can create new travellers.
        // The prefab should have a Traveller component attached to it.
        GameObject travellerPrefab = Resources.Load<GameObject>("Traveller");
        GameObject newTravellerObj = Instantiate(travellerPrefab, new Vector3(0, 0, 0), Quaternion.identity);

        // Get the Traveller component from the instantiated object
        Traveller newTraveller = newTravellerObj.GetComponent<Traveller>();

        // Initialize the traveller's properties
        InitializeTravellerFromThisBuilding(newTraveller, startingEdge);

        // Subscribe the new traveller to the starting edge
        startingEdge.subscribe(newTraveller);
    }

    private void InitializeTravellerFromThisBuilding(Traveller traveller, Edge startingEdge)
    {
        // Set the starting edge and position
        traveller.currentEdge = this.edge;
        traveller.positionOnEdge = this.positionOnEdge;

        // Set other initial properties as needed, for example:
        traveller.currentVelocity = 0;
        // traveller.modeOfTransport = ModeOfTransport.Car; // TODO: this is a placeholder
        traveller.H = Edge.H; // Set H from Edge class

        // Position the traveller GameObject on the edge
        traveller.transform.position = startingEdge.getPointOnEdge(traveller.positionOnEdge);
    }

    public Vector3 getEdgeLocation()
    {
        return edge.getPointOnEdge(positionOnEdge);
    }

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

    // Get the number of occupants
    public int GetOccupantCount()
    {
        return occupantCount;
    }
}
