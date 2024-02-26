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

    }

    public void Update() {
        if (Time.time >= nextSpawnTime) {
            Debug.Log("Time to spawn new traveller!");
            if (Random.value < spawnProbability) {
                Debug.Log("Spawning traveller from Traveller Manager");
                SpawnTraveller();
            }

            nextSpawnTime = Time.time + timeBetweenSpawns;
        }
    }

    public void SpawnTraveller() {
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
