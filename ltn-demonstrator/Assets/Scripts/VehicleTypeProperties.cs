using UnityEngine;
using System;

// NOTE: Similar info is found in ModeOfTransport.cs
// we need better separation betweeen the classes,
// as this doesn't really make sense atm.
// We should get rid of ModeOfTransport.cs
public enum VehicleType
{
    PersonalCar,
    SUV,
    Van,
    Taxi,
    Pedestrian,
    Bicycle
}

public class VehicleProperties
{
    public VehicleType Type;
    private float maxVelocity;
    public float MaxVelocity
    {
        get
        {
            if (maxVelocity == 0)
            {
                maxVelocity = this.getBaseMaxVelocity();
                // vary up to the variance value
                maxVelocity -= this.getVelocityVariance();
                maxVelocity += this.getVelocityVariance() * 2 * UnityEngine.Random.value;

            }
            return maxVelocity;
        }
    }
    public float Acceleration
    {
        get
        {
            return getAcceleration();
        }
    }
    public float Deacceleration
    {
        get
        {
            return getDeacceleration();
        }
    }
    public float RateOfEmission
    {
        get
        {
            return getRateOfEmmision();
        }
    }
    public int MaxNumberOfPassengers
    {
        get
        {
            return getMaxNumberOfPassengers();
        }
    }

    public VehicleProperties(VehicleType type)
    {
        this.Type = type;
    }
    public VehicleProperties()
    {
        VehicleType type;
        Array values = Enum.GetValues(typeof(VehicleType));
        type = (VehicleType)values.GetValue(UnityEngine.Random.Range(0, values.Length-2)); // -2 to exclude Pedestrian & bike. TODO: make this more elegant
        this.Type = type;
    }
    private float getBaseMaxVelocity()
    {
        switch (Type)
        {
            case VehicleType.PersonalCar:
                return 5f;
            case VehicleType.SUV:
                return 4.2f;
            case VehicleType.Van:
                return 4.5f;
            case VehicleType.Taxi:
                return 4.8f;
            case VehicleType.Pedestrian:
                return 1.5f;
            case VehicleType.Bicycle:
                return 3.0f;
            default:
                return 0.0f;
        }
    }
    private float getVelocityVariance()
    {
        switch (Type)
        {
            case VehicleType.PersonalCar:
                return 1.0f;
            case VehicleType.SUV:
                return 0.6f;
            case VehicleType.Van:
                return 0.5f;
            case VehicleType.Taxi:
                return 0.2f;
            case VehicleType.Pedestrian:
                return 0.2f;
            case VehicleType.Bicycle:
                return 0.5f;
            default:
                return 0.0f;
        }
    }
    private float getAcceleration()
    {
        switch (Type)
        {
            case VehicleType.PersonalCar:
                return 2.0f;
            case VehicleType.SUV:
                return 1.5f;
            case VehicleType.Van:
                return 1.0f;
            case VehicleType.Taxi:
                return 2.0f;
            case VehicleType.Pedestrian:
                return 1000.0f;
            case VehicleType.Bicycle:
                return 2.5f;
            default:
                return 0.0f;
        }
    }
    private float getDeacceleration()
    {
        switch (Type)
        {
            case VehicleType.PersonalCar:
                return 4.0f;
            case VehicleType.SUV:
                return 3.5f;
            case VehicleType.Van:
                return 2.0f;
            case VehicleType.Taxi:
                return 4.0f;
            case VehicleType.Pedestrian:
                return 1000.0f;
            case VehicleType.Bicycle:
                return 5f;
            default:
                return 0.0f;
        }
    }
    private float getRateOfEmmision()
    {
        switch (Type)
        {
            // values in g/m (grams per meter)
            case VehicleType.PersonalCar:
                return 0.143f;
            case VehicleType.SUV:
                return 0.202f;
            case VehicleType.Van:
                return 0.176f;
            case VehicleType.Taxi:
                return 0.100f;
            default:
                return 0.0f;
        }
    }
    private int getMaxNumberOfPassengers()
    {
        switch (Type)
        {
            case VehicleType.PersonalCar:
                return 4;
            case VehicleType.SUV:
                return 6;
            case VehicleType.Van:
                return 2;
            case VehicleType.Taxi:
                return 3;
            case VehicleType.Pedestrian:
                return 1; // lol
            case VehicleType.Bicycle:
                return 1;
            default:
                return 0;
        }
    }
}
