using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ModeOfTransport
{
    Walker,
    Bicycle,
    Car,
    Van,
    Truck,
    Bus
}

public class Traveller : MonoBehaviour
{
    // Private attributes
    private float deltaD;
    private float totalDistanceMoved;
    private float positionOnEdge;
    private Path currentPath;
    private List<Path> previousPaths = new List<Path>();

    // Public attributes
    public Edge currentEdge; // Add semicolon here
    public float currentVelocity; // Add semicolon here
    public float maxVelocity; // Add semicolon here
    public int noOfPassengers; // Add semicolon here
    public ModeOfTransport modeOfTransport; // Add semicolon here

    // Public variable for timeStep
    public float timeStep = 1f / 60f; // Assuming 60 frames per second

    // H = the distance between points on the grid (kilometers)
    // set an arbitrary variable
    // set a loop that travels the traveller 
    // let the length of edge = e
    // e * H = length of edge in km
    public float H; // Add semicolon here

    // Assuming rateOfEmission is defined somewhere
    private float rateOfEmission;

    public float CalculateEmissions()
    {
        // Calculate emissions using the rateOfEmission attribute and totalDistanceMoved
        return rateOfEmission * totalDistanceMoved;
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

    // Method to move to the next edge in the path
    private void MoveToNextEdge()
    {
        // Your existing logic for moving to the next edge
        // traveller pointer to say what edge it's on
        // and what proportion of edge
        // but edge will calculate deltaD
    }

    // Public method to calculate deltaD^(-1)
    public float TranslateFromTrueToDeltaD(float desiredSpeed, float edgeLength)
    {
        // Calculate deltaD^(-1) based on the given formula
        return (desiredSpeed * edgeLength * H) / timeStep; // TimeStep = 1 frame over frames per sec
    }

    // Update is called once per frame
    void Update()
    {
        // MOVE CALLS TO EDGE CLASS ***
        // Update code here
        // Calculate deltaD and update the position
        CalculateDeltaD();

        // Update the total distance moved
        totalDistanceMoved += Mathf.Abs(deltaD);
    }

    // Start is called before the first frame update
    void Start()
    {
        // *NOT IN THE CORRECT POSITION* - Visualize the agent on an edge

        // Assuming you have a Graph class to represent your graph structure
        Graph graph = GetYourGraph();

        // Assuming you have a method to get the starting edge and position
        Edge startingEdge = graph.GetStartingEdge();
        float startingPosition = graph.GetStartingPositionOnEdge();

        // Spawn the traveler at the starting point
        SpawnAtStartingPoint(startingEdge, startingPosition);

        // Call the Start method of the associated Edge
        startingEdge.Start();

        // Save starting point to respawn-return later?
        currentEdge = startingEdge;
        positionOnEdge = Mathf.Clamp(startingPosition, 0f, startingEdge.length);
        transform.position = startingEdge.GetPointOnEdge(positionOnEdge);
        // then run update routing
    }

    // Public method to de-spawn the traveler at the destination
    public void DeSpawnAtDestination()
    {
        // Perform any cleanup or de-spawning logic

        // Store destination for respawn to return?

        // For example, you might disable the GameObject or move it to an inactive zone
        gameObject.SetActive(false);
    }
}
