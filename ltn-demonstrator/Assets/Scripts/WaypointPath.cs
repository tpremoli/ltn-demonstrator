using UnityEngine;
using System.Collections.Generic;

public class WaypointPath
{
    private Waypoint currentWaypoint;

    public WaypointPath(Waypoint startingPoint)
    {
        currentWaypoint = startingPoint;
    }

    public Waypoint GetNextWaypoint()
    {
        if (currentWaypoint == null || currentWaypoint.adjacentWaypoints.Count == 0)
        {
            Debug.LogWarning("Current waypoint is null or has no adjacent waypoints.");
            return null;
        }

        // For simplicity, just get the next waypoint in the list.
        // This logic can be expanded to more complex pathfinding algorithms.
        currentWaypoint = currentWaypoint.adjacentWaypoints[0]; // Simple example, replace with actual pathfinding logic
        return currentWaypoint;
    }
}
