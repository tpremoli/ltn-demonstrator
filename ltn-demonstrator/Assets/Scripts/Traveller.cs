using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Traveller : MonoBehaviour
{
    // Private attributes
    private float totalDistanceMoved;
    private float positionOnEdge;
    private Path currentPath;


    // Public attributes
    public Edge currentEdge; 
    public float currentVelocity; 
    public float maxVelocity; // will be assigned according to agent category enum (EdgeFunctionality)
    public int noOfPassengers; 
    

    // Public variable for timeStep
    public float timeStep = 1f / 60f; // Assuming 60 frames per second

    // H = the distance between points on the grid (kilometers)
    // set an arbitrary variable
    // set a loop that travels the traveller 
    // let the length of edge = e
    // e * H = length of edge in km

    public float H; 

    // Assuming rateOfEmission is defined somewhere
    private float rateOfEmission;

    public float CalculateEmissions()
    {
        // Calculate emissions using the rateOfEmission attribute and totalDistanceMoved
        return rateOfEmission * totalDistanceMoved;
    }

    // Method to move to the next edge in the path
    private void MoveToNextEdge()
    {
        // Your existing logic for moving to the next edge
        // traveller pointer to say what edge it's on
        // and what proportion of edge
        // but edge will calculate deltaD
    }


    // Update is called once per frame
    void Update()
    {

    }

    // Start is called before the first frame update
    void Start()
    {
        // *NOT IN THE CORRECT POSITION* - Visualize the agent on an edge
        currentEdge.Start()
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
