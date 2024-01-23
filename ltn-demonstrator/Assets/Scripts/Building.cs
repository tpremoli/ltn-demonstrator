using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This is here for future expansion, but is not currently used
public enum BuildingType
{
    Generic,
    Residence,
    Office,
    Restaurant,
    Shop,
}

public static class BuildingProperties {
        public static Dictionary<BuildingType, float> destinationWeights = new Dictionary<BuildingType, float>(){
            {BuildingType.Generic, 0.1f},
            {BuildingType.Residence, 1.0f},
            {BuildingType.Office, 2.0f},
            {BuildingType.Restaurant, 2.5f},
            {BuildingType.Shop, 5.0f},
        };

        public static List<BuildingType> buildingTypes = new List<BuildingType>((BuildingType[]) System.Enum.GetValues(typeof(BuildingType)));

        // Choose a random building type.
        public static BuildingType getRandomWeightedDestinationType() {
            float totalDestinationWeight = 0.0f;
            List<float> cumulativeWeights = new List<float>();

            // Get totals for all destination weights and populate cumulative weights list.
            foreach (KeyValuePair<BuildingType, float> destinationWeight in destinationWeights) {
                totalDestinationWeight += destinationWeight.Value;

                cumulativeWeights.Add(totalDestinationWeight);
            }

            // Select random float value.
            float r = UnityEngine.Random.value * totalDestinationWeight;

            // Iterate through cumulative weights to find index to select.
            int index = -1;
            for (int i=0; i < cumulativeWeights.Count; i++) {
                float weight = cumulativeWeights[i];
                if (r <= weight) {
                    index = i;
                    break;
                }
            }

            return buildingTypes[index];
        }
}


public class Building : MonoBehaviour
{
    // DEBUG ATTRIBUUTES
    private static bool ALLOW_SPAWNING = true;

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

    public Edge closestEdge;
    public Vector3 closestPointOnEdge;

    // Some more attributes - not sure if needed, but seemed useful
    public readonly string buildingName;    // the name of the building (i.e "the X residence". Would be fun to have a random name generator?)
    [SerializeField] public BuildingType buildingType;// the type of the building (i.e "residence", "office", "restaurant", etc. would be an enum)

    // Start is called before the first frame update. We use these to initialize the building.
    void Start()
    {
        this.graph = GameObject.Find("Graph").GetComponent<Graph>();

        //buildingType = BuildingProperties.buildingTypes[Random.Range(0, BuildingProperties.buildingTypes.Count)];

        // I don't want to hardcode these values, but I'm not sure how to do it otherwise.
        // if this is removed, the building will spam vehicles
        this.timeBetweenSpawns = 1;
        this.spawnProbability = 0.1f;
        this.nextSpawnTime = Time.time + timeBetweenSpawns;

        this.closestEdge = graph.getClosetEdge(this.transform.position);
        this.closestPointOnEdge = closestEdge.GetClosestPoint(this.transform.position);

        Debug.Log("Building Instantiated");
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        if (Application.IsPlaying(this))
        {
            Gizmos.DrawLine(this.transform.position, closestPointOnEdge);
        }
    }

    public Vector3 CalcClosestPointOnEdge()
    {
        Graph tempGraph = GameObject.Find("Graph").GetComponent<Graph>();

        Edge closestEdge = tempGraph.getClosetEdge(this.transform.position);
        Vector3 closestPointOnEdge = closestEdge.GetClosestPoint(this.transform.position);
        return closestPointOnEdge;
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
        if (!ALLOW_SPAWNING) return;
        // We need a prefab for the traveller. This is a template from which we can create new travellers.
        // The prefab should have a Traveller component attached to it.
        GameObject travellerPrefab = Resources.Load<GameObject>("Traveller");
        GameObject travellerManager = TravellerManager.Instance.gameObject;
        GameObject newTravellerObj = Instantiate(travellerPrefab, this.closestPointOnEdge, Quaternion.identity, travellerManager.transform);
        newTravellerObj.GetComponent<WaypointMover>().setOriginBuilding(this);
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