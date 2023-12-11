using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Edge : MonoBehaviour
{
    private JunctionNode origin;
    private JunctionNode destination;
    
    public float length;
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





    // Public behavior
    public void subscribe(Traveller traveller)
    {
        vehiclesOnRoad.Add(traveller);
        traveller.currentEdge = this;
        traveller.positionOnEdge = 0f; // Start at the beginning of the edge
    }

    public void unsubscribe(Traveller traveller)
    {
        vehiclesOnRoad.Remove(traveller);
    }

    public Vector3 getPointOnEdge(float positionOnEdge)
    {
        // Calculate the point on the edge based on the given formula
        // return (1 - positionOnEdge) * origin + positionOnEdge * destination;
        return (1 - positionOnEdge) * origin.transform.position + positionOnEdge * destination.transform.position;
    }

    // "Spawn child rectangle on simulation start" functionality
    void Start()
    {
        spawnChildRectangle();
        calculateRoadLength();
    }

    // "OnUpdate" functionality
    void Update()
    {
        // MOVE CALLS TO EDGE CLASS ***
        // Update code here
        // Calculate deltaD and update the position

        // Needs to be called for each agent. Pass the agents as parameters
        // calculateDeltaD();

        // Update the total distance moved
        // Again, needs to be called for each agent. Pass the agents as parameters
        // totalDistanceMoved += Mathf.Abs(deltaD);
    }

    // Spawn visualisation of the agent
    private void spawnChildRectangle()
    {
        GameObject childRectangle = GameObject.CreatePrimitive(PrimitiveType.Cube);
        childRectangle.transform.SetParent(transform);
        // Additional positioning and scaling logic based on the junction nodes
    }

    // Road length calculation functionality
    private void calculateRoadLength()
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
