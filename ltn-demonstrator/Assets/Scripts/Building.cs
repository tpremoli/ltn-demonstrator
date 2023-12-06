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
    private readonly int vehicleMax;
    private readonly int occupantMax;
    private Dictionary<BuildingType, float> destinationWeights; // Distribution for destination types

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
    public readonly BuildingType buildingType;// the type of the building (i.e "residence", "office", "restaurant", etc. would be an enum)

    // Constructor 
    public Building(int vehicleMax, int occupantMax, Edge edgeLocation)
    {
        this.vehicleMax = vehicleMax;
        this.occupantMax = occupantMax;
        this.edgeLocation = edgeLocation;
        this.occupantCount = 0;
        this.VehicleList = new List<Vehicle>();
        this.destinationWeights = new Dictionary<string, float>();
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

        // Load the vehicle prefab
        GameObject vehicle = Resources.Load("Vehicles/Car_3_Blue");

        // Traveller doesn't have a constructor yet
        Traveller newTraveller = new Traveller()
        newTraveller.currentEdge = this.edgeLocation;
        newTraveller.modeOfTransport = ModeOfTransport.Car;

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
