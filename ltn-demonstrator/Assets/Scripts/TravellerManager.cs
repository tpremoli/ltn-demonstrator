using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TravellerManager : MonoBehaviour
{
    [System.Serializable]
    public class VehiclePrefabTypePair
    {
        public GameObject vehiclePrefab;
        public VehicleType vehicleType;
    }

    public static TravellerManager Instance;
    public float timeBetweenSpawns;
    public float spawnProbability;
    public float nextSpawnTime;
    private Graph graph;

    EventManager eventManager;

    // pick random model and material
    [SerializeField]
    public List<VehiclePrefabTypePair> vehiclePrefabTypePairs;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Start() {
        this.timeBetweenSpawns = 0.5f;
        this.spawnProbability = 1f;
        this.nextSpawnTime = Time.time + timeBetweenSpawns;
        this.graph = Graph.Instance;
        
        eventManager = GameObject.Find("EventManager").GetComponent<EventManager>();

    }

    public void Update() {
        if (Time.time >= nextSpawnTime) {
            if (Random.value < spawnProbability) {
                //Debug.Log("Spawning random traveller...");
                //SpawnRandomTraveller();
            }

            nextSpawnTime = Time.time + timeBetweenSpawns;
        }

        if (eventManager.eventList.Count > 0) {
            foreach (Journey j in eventManager.eventList) {
                if (j.status == JourneyStatus.NotStarted) {
                    Debug.Log("Spawning traveller from event list: " + j.origin + " to " + j.destination + " at time " + j.time + " (current time is " + Time.time + ")");
                    SpawnTraveller(j.origin, j.destination);
                    j.traveller.journeyStarted(j);
                }
                //eventManager.eventList.Remove(j);
            }
        }
    }
    
    public void SpawnTraveller(string origin, string destination) {
        GameObject travellerPrefab = Resources.Load<GameObject>("Traveller");
        GameObject newTravellerObj = Instantiate(travellerPrefab, this.transform);
        Building originBuilding = Graph.Instance.buildings[origin];
        Building destinationBuilding = Graph.Instance.buildings[destination];
        newTravellerObj.GetComponent<WaypointMover>().Setup(originBuilding, destinationBuilding, ModeOfTransport.Car);
    }

    public void SpawnRandomTraveller() {
        GameObject travellerPrefab = Resources.Load<GameObject>("Traveller");
        //GameObject travellerManager = Instance.gameObject;
        GameObject newTravellerObj = Instantiate(travellerPrefab, this.transform);
        newTravellerObj.GetComponent<WaypointMover>().Setup(graph.pickRandomBuilding(), graph.pickRandomBuilding(), ModeOfTransport.Car);
    }

    public GameObject pickRandomModelAndMaterial(VehicleType type)
    {
        var filteredList = vehiclePrefabTypePairs.Where(pair => pair.vehicleType == type).ToList();
        if (filteredList.Count > 0)
        {
            int randomIndex = UnityEngine.Random.Range(0, filteredList.Count);
            return filteredList[randomIndex].vehiclePrefab;
        }

        return null; // Or handle this case as needed
    }
}
