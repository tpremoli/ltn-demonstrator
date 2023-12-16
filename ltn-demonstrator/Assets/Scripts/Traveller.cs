using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Traveller : MonoBehaviour
{
    // Private attributes
    private float totalDistanceMoved;
    private Path currentPath;


    // Public attributes
    public Edge currentEdge; 
    public float positionOnEdge;
    private float deltaD;
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

    public float calculateEmissions()
    {
        // Calculate emissions using the rateOfEmission attribute and totalDistanceMoved
        return rateOfEmission * totalDistanceMoved;
    }

    public void calculateDeltaD(Traveller agent)
    {
        // Ensure currentEdge is not null
        if (this != null)
        {
            // getting agent parameters
            float agentVelocity = agent.currentVelocity;
            float timeStep = agent.timeStep;
            float H = agent.H;

            // Calculate deltaD based on the given formula
            deltaD = (agentVelocity * (timeStep)) / (this.currentEdge.length * H);

            // Optional: You can limit deltaD to maxVelocity if needed
            deltaD = Mathf.Clamp(deltaD, 0f, maxVelocity);

            // Update the positionOnEdge based on the calculated deltaD
            agent.positionOnEdge += deltaD;

            // If the traveller has moved beyond the current edge, update the current edge and reset positionOnEdge
            if (agent.positionOnEdge > this.currentEdge.length)
            {
                // Move to the next edge
                agent.MoveToNextEdge();
            }
        } else
        {
            // Returns error if null value given
            Debug.LogError("Current edge is null.");
        }
    }
        // Public method to calculate deltaD^(-1)
    public float translateFromTrueToDeltaD(float desiredSpeed, float edgeLength, float timeStep)
    {
        // Calculate deltaD^(-1) based on the given formula
        return (desiredSpeed * edgeLength * H) / timeStep; // TimeStep = 1 frame over frames per sec
    }

    // New public methods for edge space translation
    // to ensure meaningful behaviour for agents navigate 
    //the graph an arbitrary number greater than 0 should be returned
    public float unityToEdgeSpace(float d)
    {
        if (this.currentEdge.length > 0)
        {
            return d / this.currentEdge.length;
        }
        else
        {
            Debug.LogError("Edge length is zero. Cannot perform UnityToEdgeSpace conversion.");
            return 0;
        }
    }

    public float edgeSpaceToUnity(float d)
    {
        if (this.currentEdge.length > 0)
        {
            return d * this.currentEdge.length;
        }
        else
        {
            Debug.LogError("Edge length is zero. Cannot perform EdgeSpaceToUnity conversion.");
            return 0;
        }
    }

    public float unityToReal(float d)
    {
        return d * H;
    }

    public float realToUnity(float d)
    {
        return d / H;
    }
    // Method to move to the next edge in the path
    public void MoveToNextEdge()
    {
        int currentIndex = currentPath.path.IndexOf(currentEdge);
        if (currentIndex < currentPath.path.Count - 1)
        {
            Edge nextEdge = currentPath.path[currentIndex + 1];
            nextEdge.subscribe(this);
        }
        else
        {
            // Reached the end of the path, perform cleanup or handle as needed
            currentPath.endPath();
            despawnAtDestination();
        }
    }


    // Update is called once per frame
    void Update()
    {

    }

    // Start is called before the first frame update
    void Start()
    {
        // The lines related to spawning the traveller at the starting point are commented out
        // as we don't have a specific graph implementation. Beyond that, some parts of this
        // are currently handled in Building.cs, but we can move them here if needed.


        // 
        // *NOT IN THE CORRECT POSITION* - Visualize the agent on an edge
        // currentEdge.Start();
        // Assuming you have a Graph class to represent your graph structure

        // We don't have a graph class yet
        // Graph graph = GetYourGraph();

        // Assuming you have a method to get the starting edge and position
        // Edge startingEdge = graph.GetStartingEdge();
        // float startingPosition = graph.GetStartingPositionOnEdge();

        // Spawn the traveler at the starting point
        // spawnAtStartingPoint(startingEdge, startingPosition);

        // Call the Start method of the associated Edge
        // startingEdge.Start();

        // Save starting point to respawn-return later?
        // currentEdge = startingEdge;
        // positionOnEdge = Mathf.Clamp(startingPosition, 0f, startingEdge.length);
        // transform.position = startingEdge.getPointOnEdge(positionOnEdge);
        // then run update routing

        // Assuming you have a method to get the starting edge and position
        // Edge startingEdge = currentPath.path[0];
        // float startingPosition = 0f; // You may need to adjust this based on your requirements
        // startingEdge.subscribe(this);
        Debug.Log("Traveller Instantiated");

    }

    public void spawnAtStartingPoint(Edge startingEdge, float startingPosition){
        // Store the current path reference
        // currentPath = path;
    }

    // Public method to de-spawn the traveler at the destination
    public void despawnAtDestination()
    {
        // Perform any cleanup or de-spawning logic

        // Store destination for respawn to return?

        // For example, you might disable the GameObject or move it to an inactive zone
        gameObject.SetActive(false);
    }
}
