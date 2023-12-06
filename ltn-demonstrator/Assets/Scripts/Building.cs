using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    // Private attributes
    private readonly int vehicleMax;  // Maximum number of vehicles
    private readonly int occupantMax; // Maximum number of occupants
    private Dictionary<string, float> destinationWeights; // Distribution for destination types

    // Public attributes
    public List<Vehicle> VehicleList; // List of vehicles at the building
    public int OccupantCount;         // Number of occupants in the building
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
        
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Spawn method
    public void Spawn(Vehicle vehicle)
    {
        // Generate a path and set up so that vehicle can complete journey
        // Insert vehicle to the edge queue
        // ...
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
