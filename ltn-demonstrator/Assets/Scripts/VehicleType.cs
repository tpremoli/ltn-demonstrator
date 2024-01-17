using UnityEngine;
using System;

public enum VehicleTypes
{
    PersonalCar,
    SUV,
    Van,
    Taxi
}

public class VehicleType
{
    public VehicleTypes Type;
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

    public VehicleType(VehicleTypes type)
    {
        this.Type = type;
    }
    public VehicleType()
    {
        VehicleTypes type;
        Array values = Enum.GetValues(typeof(VehicleTypes));
        type = (VehicleTypes)values.GetValue(UnityEngine.Random.Range(0, values.Length));
        this.Type = type;
    }
    private float getBaseMaxVelocity()
    {
        switch (Type)
        {
            case VehicleTypes.PersonalCar:
                return 5f;
            case VehicleTypes.SUV:
                return 4.2f;
            case VehicleTypes.Van:
                return 4.5f;
            case VehicleTypes.Taxi:
                return 4.8f;
            default:
                return 0.0f;
        }
    }
    private float getVelocityVariance()
    {
        switch (Type)
        {
            case VehicleTypes.PersonalCar:
                return 1.0f;
            case VehicleTypes.SUV:
                return 0.6f;
            case VehicleTypes.Van:
                return 0.5f;
            case VehicleTypes.Taxi:
                return 0.2f;
            default:
                return 0.0f;
        }
    }
    private float getAcceleration()
    {
        switch (Type)
        {
            case VehicleTypes.PersonalCar:
                return 1.0f;
            case VehicleTypes.SUV:
                return 0.5f;
            case VehicleTypes.Van:
                return 0.25f;
            case VehicleTypes.Taxi:
                return 1.0f;
            default:
                return 0.0f;
        }
    }
    private float getDeacceleration()
    {
        switch (Type)
        {
            case VehicleTypes.PersonalCar:
                return 2.5f;
            case VehicleTypes.SUV:
                return 2.5f;
            case VehicleTypes.Van:
                return 2.0f;
            case VehicleTypes.Taxi:
                return 2.5f;
            default:
                return 0.0f;
        }
    }
    private float getRateOfEmmision()
    {
        switch (Type)
        {
            case VehicleTypes.PersonalCar:
                return 1.0f;
            case VehicleTypes.SUV:
                return 1.2f;
            case VehicleTypes.Van:
                return 1.4f;
            case VehicleTypes.Taxi:
                return 1.0f;
            default:
                return 0.0f;
        }
    }
    private int getMaxNumberOfPassangers()
    {
        switch (Type)
        {
            case VehicleTypes.PersonalCar:
                return 4;
            case VehicleTypes.SUV:
                return 6;
            case VehicleTypes.Van:
                return 2;
            case VehicleTypes.Taxi:
                return 3;
            default:
                return 0;
        }
    }
}
