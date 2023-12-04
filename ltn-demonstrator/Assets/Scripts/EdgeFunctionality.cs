using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Edge : MonoBehaviour
{
    private JunctionNode origin;
    private JunctionNode destination;
    private float length;
    private readonly float maxVelocity = float.MaxValue;
    private static float h = 1.0f; // Initial value for H
    private ModesOfTransportEnum modeOfTransport;
    private List<Traveller> vehiclesOnRoad = new List<Traveller>();

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

    // Other attributes and methods...

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
}
