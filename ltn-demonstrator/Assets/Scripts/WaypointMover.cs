using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointMover : MonoBehaviour
{
    [SerializeField] private Waypoint startingWaypoint;  // Initial waypoint to start the movement
    [SerializeField] private float speed = 5f;           // Movement speed
    [SerializeField] private float distanceThreshold = 0.1f;  // Distance threshold to consider reaching a waypoint

    private WaypointPath path;           // Instance of the pathfinding class
    private Waypoint currentWaypoint;    // Current waypoint the mover is heading towards

    void Start()
    {
        // Initialize the path with the starting waypoint
        path = new WaypointPath(startingWaypoint);
        
        // Get the first waypoint in the path and set the initial position
        currentWaypoint = path.GetNextWaypoint();
        transform.position = currentWaypoint.transform.position;
    }

    void Update()
    {
        if (currentWaypoint == null) return;  // Stop if there's no current waypoint

        // Move towards the current waypoint using linear interpolation
        transform.position = Vector3.MoveTowards(transform.position, currentWaypoint.transform.position, speed * Time.deltaTime);

        // Check if the distance to the current waypoint is below the threshold
        if (Vector3.Distance(transform.position, currentWaypoint.transform.position) < distanceThreshold)
        {
            // If the threshold is met, get the next waypoint in the path
            currentWaypoint = path.GetNextWaypoint();
        }
    }
}
