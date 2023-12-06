using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    // Private attributes
    private readonly int vehicleMax;
    private readonly int occupantMax;
    private Dictionary<string, float> destinationWeights; // Distribution for destination types

    // the spawn probability should be based on the building type and maximum number of occupants.
    // as it stands, it is a constant value, but it should be a function/enum of the building type
    private readonly float spawnProbability = 0.1f;

    private readonly float timeBetweenSpawns = 1f; // The time between spawn attempts

    // Public attributes
    public List<Vehicle> VehicleList; // List of vehicles at the building.
    public int occupantCount;
    public Edge edgeLocation;         // Edge where the building is located

    // Some more attributes - not sure if needed, but seemed useful
    public readonly string buildingName;    // the name of the building (i.e "the X residence". Would be fun to have a random name generator?)
    public readonly string buildingType;    // the type of the building (i.e "residence", "office", "restaurant", etc. would be an enum)

    // Constructor to initialize immutable values 
    public Building(int vehicleMax, int occupantMax)
    {
        this.vehicleMax = vehicleMax;
        this.occupantMax = occupantMax;
        VehicleList = new List<Vehicle>();
        destinationWeights = new Dictionary<string, float>();
    }

    // Update is called once per frame
    void Update()
    {
        // Using time-based spawning- there is a spawnProbability chance of spawning a 
        // vehicle every timeBetweenSpawns seconds. Only spawn if the maximum number of
        // vehicles has not been reached.
        if (VehicleList.Count < vehicleMax && Time.time >= nextSpawnTime)
        {
            // Spawn a vehicle, if the random number is less than the spawn probability
            if (Random.value < spawnProbability)
            {
                SpawnTraveller();
            }
            // Set the next spawn time
            nextSpawnTime = Time.time + timeBetweenSpawns;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Random.seed = 42; // Set seed for random number generator
    }

    // Spawn method
    public void SpawnTraveller()
    {
        // first, pick the vehicle randomly from the list
        //var rngIndex = Random.Range(0, TravellerList.Count);
        //Traveller traveller = TravellerList[rngIndex];
        




        GameObject vehicle = Instantiate(Resources.Load("Prefabs/Vehicle")) as GameObject;
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
        return OccupantCount;
    }
}
