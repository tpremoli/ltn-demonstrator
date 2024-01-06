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

    private List<Building> buildings;
    private Building destinationBuilding;


    // TODO: Need a way to notice if there is agents ahead of the current agent
    // this can be done using a box collider in front of the agent, and checking if there is a collision
    // if there is a collision, then the agent will slow down, and if there is no collision, then the agent will speed up.
    // This will be done in the update function, and the agent will have a max velocity, and a min velocity.
    // The agent will also have a current velocity, which will be updated in the update function.
    // Beyond that, the collider of the agent itself will be used to check if there is a crash with another agent

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

        // Choose a random destination building.
        chooseDestinationBuilding();

        // Initialize the path with the starting waypoint
        path = new WaypointPath(this.transform.position, destinationBuilding.GetClosestPointOnEdge());

        if (path.path == null)
        {
            // if no path is found, destroy the object.
            // Later on, we should change this so that the traveller changes their mode of transport
            Debug.LogWarning("Path doesn't exist for Traveller" + this.gameObject.name+". Destroying object.");
            Destroy(this.gameObject);
        }

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
    }

    // Choose a random destination from the possible buildings in the grid.
    public void chooseDestinationBuilding() {
        // Get list of buildings.
        // TODO: find a way to make this global so all movers can access this without having to call this function every time.
        buildings = new List<Building>(FindObjectsOfType<Building>());
        // Choose random destination building.
        destinationBuilding = buildings[Random.Range(0, buildings.Count)];

        // Edge case where the chosen building is the same as the building the traveller spawned at.
        while (Vector3.Distance(this.transform.position, destinationBuilding.GetClosestPointOnEdge()) < distanceThreshold) {
            // Choose new random destination building.
            destinationBuilding = buildings[Random.Range(0, buildings.Count)];
        }
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

    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            // Draw the destination sphere
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(path.destinationPos, 1f);
            if (currentWaypoint != null)
            {
                // Draw the current waypoint sphere
                Gizmos.DrawSphere(currentWaypoint.transform.position, 1f);
            }

            // Draw the path from the agent's current position
            Gizmos.color = Color.yellow;

            Vector3 startPosition = currentWaypoint != null ? currentWaypoint.transform.position : transform.position; // Use agent's position if currentWaypoint is null

            if (path.path.Count > 0)
            {
                // Iterate through the remaining waypoints
                foreach (var waypoint in path.path)
                {
                    // Draw sphere for each waypoint
                    Gizmos.DrawSphere(waypoint.transform.position, 1f);

                    // Draw arrow from the start position to the current waypoint
                    DrawArrow.ForGizmo(startPosition + Vector3.up, (waypoint.transform.position - startPosition) + Vector3.up, Color.yellow, 1f);

                    // Update start position
                    startPosition = waypoint.transform.position;
                }
            }

            // Always draw arrow to the destination from the current position or last waypoint
            DrawArrow.ForGizmo(startPosition + Vector3.up, (path.destinationPos - startPosition) + Vector3.up, Color.yellow, 1f);
        }
    }
}
