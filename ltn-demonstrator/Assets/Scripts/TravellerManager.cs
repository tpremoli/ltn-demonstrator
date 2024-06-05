using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.IO;

public class TravellerManager : MonoBehaviour
{
    [System.Serializable]
    public class VehiclePrefabTypePair
    {
        public GameObject vehiclePrefab;
        public VehicleType vehicleType;
    }

    public static TravellerManager Instance { get; private set; }
    private Graph graph;

    public EventManager eventManager;

    // records teh last time the Traveller Manager spawned a Traveller
    private float lastSpawn;

    // pick random model and material
    [SerializeField]
    public List<VehiclePrefabTypePair> vehiclePrefabTypePairs;
    public int noOfTravellers;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            noOfTravellers = 0;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Start()
    {
        graph = Graph.Instance;

        eventManager = GameObject.Find("EventManager").GetComponent<EventManager>();


        // saving and loading from json
        string filePath = "Saves/eventlist.json";
        if (File.Exists(filePath))
        {
            eventManager.LoadEventListFromJson(filePath);
        }
        else
        {
            // ideally we want to generate the eventlist here
            //eventManager.generateJourneys()
            eventManager.SaveEventListToJson(filePath);
        }
        this.lastSpawn = Time.time;
    }

    public void Update()
    {
        if (eventManager.eventList.Count > 0)
        {
            foreach (Journey j in eventManager.eventList)
            {
                if (j.status == JourneyStatus.NotStarted)
                {
                    Debug.Log("Spawning traveller from event list: " + j.origin + " to " + j.destination + " at time " + j.time + " (current time is " + Time.time + ")");
                    SpawnTraveller(j);
                    j.traveller.journeyStarted(j);
                    lastSpawn = Time.time;
                }
                //eventManager.eventList.Remove(j);
            }
        }
    }

    public bool FinishedFor(float period)
    {
        if (transform.childCount > 0) return false;
        if (lastSpawn + period < Time.time) return true;
        return false;
    }

    public void SpawnTraveller(Journey journey)
    {
        GameObject travellerPrefab = Resources.Load<GameObject>("Traveller");
        GameObject newTravellerObj = Instantiate(travellerPrefab, this.transform);
        Building originBuilding = Graph.Instance.buildings[journey.origin];
        Building destinationBuilding = Graph.Instance.buildings[journey.destination];
        ModeOfTransport mode;
        if (originBuilding.closestPedestrianEdge == null)
        {
            // we don't have a pedestrian edge, so we can't be a pedestrian (for now)
            mode = (ModeOfTransport)Random.Range(1, 3);
        }
        else
        {
            mode = (ModeOfTransport)Random.Range(0, 3);
        }

        newTravellerObj.GetComponent<WaypointMover>().Setup(originBuilding, destinationBuilding, mode, journey);
        SaveTravellerData(newTravellerObj);
    }

    public void SaveTravellerData(GameObject newTravellerObj)
    {
        //assign ID to traveller - although its actually a waypointPath, will need to be reconfigured
        WaypointMover waypointMover = newTravellerObj.GetComponent<WaypointMover>();
        waypointMover.ID = Instance.noOfTravellers; // Assign ID
        //make stats structures here 

        //create data struct for traveller information
        PathData pathData = new PathData
        {
            path = newTravellerObj.GetComponent<WaypointMover>().getEdgePath(),//getpath
            vType = newTravellerObj.GetComponent<WaypointMover>().vType,
            startTime = Time.time,
            ID = Instance.noOfTravellers,
            routeChange = false
        };
        Debug.Log("------------------Vehicle Type: " + pathData.vType);
        //store to list
        StatisticsManager.Instance.AddPathData(pathData);  //finish append
        noOfTravellers += 1;
    }

    public void SpawnRandomTraveller()
    {
        GameObject travellerPrefab = Resources.Load<GameObject>("Traveller");
        //GameObject travellerManager = Instance.gameObject;
        GameObject newTravellerObj = Instantiate(travellerPrefab, this.transform);
        newTravellerObj.GetComponent<WaypointMover>().Setup(graph.pickRandomBuilding(), graph.pickRandomBuilding(), ModeOfTransport.Car, null);
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
    public GameObject GetManagerObject()
    {
        return this.gameObject;
    }
}
