using UnityEngine;
using System;

public enum VehicleType
{
    PersonalCar,
    SUV,
    Van,
    Taxi
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
    public int MaxNumberOfPassangers
    {
        get
        {
            return getMaxNumberOfPassangers();
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
        type = (VehicleType)values.GetValue(UnityEngine.Random.Range(0, values.Length));
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
            default:
                return 0.0f;
        }
    }
    private float getRateOfEmmision()
    {
        switch (Type)
        {
            case VehicleType.PersonalCar:
                return 1.0f;
            case VehicleType.SUV:
                return 1.2f;
            case VehicleType.Van:
                return 1.4f;
            case VehicleType.Taxi:
                return 1.0f;
            default:
                return 0.0f;
        }
    }
    private int getMaxNumberOfPassangers()
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
            default:
                return 0;
        }
    }
}
