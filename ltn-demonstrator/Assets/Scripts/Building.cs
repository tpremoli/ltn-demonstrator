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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
