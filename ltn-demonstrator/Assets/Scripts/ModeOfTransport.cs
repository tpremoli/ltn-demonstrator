public class ModeOfTransport
{
    private Mode mode;

    public ModeOfTransport(Mode mode)
    {
        this.mode = mode;
    }

    public enum Mode
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
        switch (mode)
        {
            case Mode.Pedestrian:
                return 5.0f;
            case Mode.Bicycle:
                return 15.0f;
            case Mode.Car:
                return 120.0f;
            case Mode.Van:
                return 80.0f;
            case Mode.Truck:
                return 60.0f;
            case Mode.Bus:
                return 50.0f;
            default:
                return 0.0f;
        }
    }

    public int MaxPassengers()
    {
        switch (mode)
        {
            case Mode.Pedestrian:
                return 1;
            case Mode.Bicycle:
                return 1;
            case Mode.Car:
                return 5;
            case Mode.Van:
                return 8;
            case Mode.Truck:
                return 2;
            case Mode.Bus:
                return 30;
            default:
                return 0;
        }
    }

    public int WeightClass()
    {
        switch (mode)
        {
            case Mode.Pedestrian:
                return 1;
            case Mode.Bicycle:
                return 1;
            case Mode.Car:
                return 5;
            case Mode.Van:
                return 8;
            case Mode.Truck:
                return 2;
            case Mode.Bus:
                return 30;
            default:
                return 0;
        }
    }
}
