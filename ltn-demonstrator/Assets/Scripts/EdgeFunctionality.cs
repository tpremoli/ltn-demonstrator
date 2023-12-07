using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Edge : MonoBehaviour
{
    private JunctionNode origin;
    private JunctionNode destination;
    private float deltaD;
    private float length;
    private readonly float maxVelocity = float.MaxValue;
    private static float h = 1.0f; // Initial value for H
    public ModeOfTransport modeOfTransport;
    private List<Traveller> vehiclesOnRoad = new List<Traveller>();

    // New Edge Constant
    public static float H
    {
        get { return h; }
        set { h = value; }
    }

    // Origin & Destination Nodes of this edge
    /*
    public JunctionNode in the Edge class refers to the JunctionNode class type
    Unity will automatically link it to the JunctionNode class 
    defined in JunctionNodeFunctionality.cs
    */
    public JunctionNode Origin
    {
        get { return origin; }
        private set
        {
            if (origin == null)
            {
                origin = value;
            }
            else
            {
                // Throw an error since Origin can only be assigned once
                Debug.LogError("Origin can only be assigned once.");
            }
        }
    }

    public JunctionNode Destination
    {
        get { return destination; }
        private set
        {
            if (destination == null)
            {
                destination = value;
            }
            else
            {
                // Throw an error since Destination can only be assigned once
                Debug.LogError("Destination can only be assigned once.");
            }
        }
    }

    // Public method to calculate deltaD
    public void CalculateDeltaD()
    {
        // Ensure currentEdge is not null
        if (currentEdge != null)
        {
            // Calculate deltaD based on the given formula
            deltaD = (currentVelocity * (timeStep)) / (currentEdge.length * H);

            // Optional: You can limit deltaD to maxVelocity if needed
            deltaD = Mathf.Clamp(deltaD, 0f, maxVelocity);

            // Update the positionOnEdge based on the calculated deltaD
            positionOnEdge += deltaD;

            // If the traveller has moved beyond the current edge, update the current edge and reset positionOnEdge
            if (positionOnEdge > currentEdge.length)
            {
                // Move to the next edge
                MoveToNextEdge();
            }
        }
    }

    // Public method to calculate deltaD^(-1)
    public float TranslateFromTrueToDeltaD(float desiredSpeed, float edgeLength)
    {
        // Calculate deltaD^(-1) based on the given formula
        return (desiredSpeed * edgeLength * H) / timeStep; // TimeStep = 1 frame over frames per sec
    }

    // New public methods for edge space translation
    public float UnityToEdgeSpace(float d)
    {
        if (length != 0)
        {
            return d / length;
        }
        else
        {
            Debug.LogError("Edge length is zero. Cannot perform UnityToEdgeSpace conversion.");
            return 0;
        }
    }

    public float EdgeSpaceToUnity(float d)
    {
        if (length != 0)
        {
            return d * length;
        }
        else
        {
            Debug.LogError("Edge length is zero. Cannot perform EdgeSpaceToUnity conversion.");
            return 0;
        }
    }

    public float UnityToReal(float d)
    {
        return d * H;
    }

    public float RealToUnity(float d)
    {
        return d / H;
    }

    // Public behavior
    public void Subscribe(Traveller traveller)
    {
        vehiclesOnRoad.Add(traveller);
    }

    public void Unsubscribe(Traveller traveller)
    {
        vehiclesOnRoad.Remove(traveller);
    }

    // "Spawn child rectangle on simulation start" functionality
    void Start()
    {
        SpawnChildRectangle();
        CalculateRoadLength();
    }

    // "OnUpdate" functionality
    void Update()
    {
        // Your onUpdate logic here
    }

    // Spawn visualisation of the agent
    private void SpawnChildRectangle()
    {
        GameObject childRectangle = GameObject.CreatePrimitive(PrimitiveType.Cube);
        childRectangle.transform.SetParent(transform);
        // Additional positioning and scaling logic based on the junction nodes
    }

    // Road length calculation functionality
    private void CalculateRoadLength()
    {
        if (origin != null && destination != null)
        {
            length = Vector3.Distance(origin.transform.position, destination.transform.position);
        }
        else
        {
            // Handle the case where either origin or destination is not assigned
            Debug.LogError("Origin or Destination is not assigned.");
        }
    }

    public enum ModeOfTransport
    {
        Pedestrian,
        Bicycle,
        Car,
        Van,
        Truck,
        Bus
    }

    public class ModeOfTransportEnum
    {
        private ModeOfTransport mode;

        public ModeOfTransportEnum(ModeOfTransport mode)
        {
            this.mode = mode;
        }

        public float MaxVelocity()
        {
            switch (mode)
            {
                case ModeOfTransport.Pedestrian:
                    return 5.0f;
                case ModeOfTransport.Bicycle:
                    return 15.0f;
                case ModeOfTransport.Car:
                    return 120.0f;
                case ModeOfTransport.Van:
                    return 80.0f;
                case ModeOfTransport.Truck:
                    return 60.0f;
                case ModeOfTransport.Bus:
                    return 50.0f;
                default:
                    return 0.0f;
            }
        }

        public int MaxPassengers()
        {
            switch (mode)
            {
                case ModeOfTransport.Pedestrian:
                    return 1;
                case ModeOfTransport.Bicycle:
                    return 1;
                case ModeOfTransport.Car:
                    return 5;
                case ModeOfTransport.Van:
                    return 8;
                case ModeOfTransport.Truck:
                    return 2;
                case ModeOfTransport.Bus:
                    return 30;
                default:
                    return 0;
            }
        }

        public int WeightClass()
        {
            switch (mode)
            {
                case ModeOfTransport.Pedestrian:
                    return 1;
                case ModeOfTransport.Bicycle:
                    return 1;
                case ModeOfTransport.Car:
                    return 5;
                case ModeOfTransport.Van:
                    return 8;
                case ModeOfTransport.Truck:
                    return 2;
                case ModeOfTransport.Bus:
                    return 30;
                default:
                    return 0;
            }
        }
    }
}
