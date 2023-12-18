using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointMover : MonoBehaviour
{
    // attributes from the Traveller class
    private float totalDistanceMoved;
    private float positionOnEdge;
    private float currentVelocity; 
    private float maxVelocity; // will be assigned according to agent category enum (EdgeFunctionality)
    private int noOfPassengers; 
    private float rateOfEmission;

    // attributes from the WaypointMover class
    [SerializeField] private Waypoint startingWaypoint;
    [SerializeField] private float speed = 5f;
    [SerializeField] private float distanceThreshold = 0.1f;

    private WaypointPath path;           // Instance of the pathfinding class
    private Waypoint currentWaypoint;    // Current waypoint the mover is heading towards
    private Graph graph;                 // Instance of the graph class


    void Start()
    {
        this.graph = GameObject.Find("Graph").GetComponent<Graph>();

        // The building spawns the Mover, so Mover is at the Building position
        Edge endEdge = graph.edges[Random.Range(0, graph.edges.Count)];

        // Initialize the path with the starting waypoint
        path = new WaypointPath(this.transform.position, endEdge.GetRandomPointOnEdge());

        // Get the first waypoint in the path and set the initial position
        currentWaypoint = path.GetNextWaypoint();
        Debug.Log("Traveller Instantiated");
    }

    void Update()
    {
        // if no waypoint to move to, go to the position along the edge
        if (currentWaypoint == null){
            // Move towards the current waypoint using linear interpolation
            transform.position = Vector3.MoveTowards(transform.position, path.destinationPos, speed * Time.deltaTime); 
            if (Vector3.Distance(transform.position, path.destinationPos) < distanceThreshold)
            {
                // if we're close enough to the destination, destroy the object
                arriveToDestination();
            }

        } else {
            transform.position = Vector3.MoveTowards(transform.position, currentWaypoint.transform.position, speed * Time.deltaTime);
            if (Vector3.Distance(transform.position, currentWaypoint.transform.position) < distanceThreshold)
            {
                // If the threshold is met, get the next waypoint in the path
                currentWaypoint = path.GetNextWaypoint();
            }
        }

        // Move towards the current waypoint using linear interpolation

        // Check if the distance to the current waypoint is below the threshold

    }

    public void arriveToDestination()
    {
        Destroy(this.gameObject);
    }

    public float calculateEmissions()
    {
        // Calculate emissions using the rateOfEmission attribute and totalDistanceMoved
        return rateOfEmission * totalDistanceMoved;
    }

}
