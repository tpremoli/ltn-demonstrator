public class VehicleType
{
    private VehicleTypes type;

    public VehicleType(VehicleTypes type)
    {
        this.type = type;
    }

    public enum VehicleTypes
    {
        Pedestrian,
        Bicycle,
        Car,
        Van,
        Truck,
        Bus
    }

    public float MaxVelocity()
    {
        switch (type)
        {
            case VehicleTypes.Pedestrian:
                return 5.0f;
            case VehicleTypes.Bicycle:
                return 15.0f;
            case VehicleTypes.Car:
                return 120.0f;
            case VehicleTypes.Van:
                return 80.0f;
            case VehicleTypes.Truck:
                return 60.0f;
            case VehicleTypes.Bus:
                return 50.0f;
            default:
                return 0.0f;
        }
    }

    public int MaxPassengers()
    {
        switch (type)
        {
            case VehicleTypes.Pedestrian:
                return 1;
            case VehicleTypes.Bicycle:
                return 1;
            case VehicleTypes.Car:
                return 5;
            case VehicleTypes.Van:
                return 8;
            case VehicleTypes.Truck:
                return 2;
            case VehicleTypes.Bus:
                return 30;
            default:
                return 0;
        }
    }

    public int WeightClass()
    {
        switch (type)
        {
            case VehicleTypes.Pedestrian:
                return 1;
            case VehicleTypes.Bicycle:
                return 1;
            case VehicleTypes.Car:
                return 5;
            case VehicleTypes.Van:
                return 8;
            case VehicleTypes.Truck:
                return 2;
            case VehicleTypes.Bus:
                return 30;
            default:
                return 0;
        }
    }
}
