using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JunctionNode : MonoBehaviour
{
    private int vehiclesPassed = 0;

    // Getter method for the vehiclesPassed attribute
    public int VehiclesPassed
    {
        get { return vehiclesPassed; }
    }

    // Method to increment vehiclesPassed by 1
    public void VehiclePassed()
    {
        vehiclesPassed++;
    }

    // Method to get the location as a Vector3
    public Vector3 GetLocation()
    {
        return transform.position;
    }
}
