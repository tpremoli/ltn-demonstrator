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
        // The building spawns the Mover, so Mover is at the Building position
        Waypoint startingPoint = FindClosestWaypoint(closestPointOnEdge.transform.position);
        Waypoint endPoint = RandEndNode(startingPoint);

        // Initialize the path with the starting waypoint
        path = new WaypointPath(startingPoint, endPoint);

        // Get the first waypoint in the path and set the initial position
        currentWaypoint = path.GetNextWaypoint();

        // Set the initial position based on the closestPointOnEdge
        transform.position = closestPointOnEdge.transform.position;
    }


    Waypoint FindClosestWaypoint(Vector3 position)
    {
        Waypoint closestWaypoint = null;
        float closestDistance = float.MaxValue;

        // Iterate through all waypoints
        foreach (Waypoint waypoint in path.path)
        {
            float distance = Vector3.Distance(position, waypoint.transform.position);
            
            // Update the closest waypoint if a closer one is found
            if (distance < closestDistance)
            {
                closestWaypoint = waypoint;
                closestDistance = distance;
            }
        }

        return closestWaypoint;
    }

    Waypoint RandEndNode(Waypoint startingPoint)
    {
        // Access the list of waypoints from the path
        List<Waypoint> waypoints = path.path;

        if (waypoints == null || waypoints.Count <= 1)
        {
            Debug.LogError("Not enough waypoints for random selection.");
            return null;
        }

        // Generate a list of waypoints excluding the starting point
        List<Waypoint> availableWaypoints = new List<Waypoint>(waypoints);
        availableWaypoints.Remove(startingPoint);

        // Select a random waypoint from the available list
        Waypoint randomWaypoint = availableWaypoints[Random.Range(0, availableWaypoints.Count)];

        return randomWaypoint;
    }



}
