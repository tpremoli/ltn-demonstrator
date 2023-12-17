using UnityEngine;
using System.Collections.Generic;

public class WaypointPath
{
    private Queue<Waypoint> waypointQueue;

    public WaypointPath(Waypoint startingPoint)
    {
        waypointQueue = new Queue<Waypoint>();
        EnqueueAdjacentWaypoints(startingPoint);
    }

    // Enqueue all adjacent waypoints of the given waypoint
    private void EnqueueAdjacentWaypoints(Waypoint waypoint)
    /*
    adjacentWaypoints list from the Waypoint class used
    BFS traversal in the WaypointPath class follows
    connections specified in the Waypoint class
    */
    {
        if (waypoint == null || waypoint.adjacentWaypoints.Count == 0)
        {
            Debug.LogWarning("Waypoint is null or has no adjacent waypoints.");
            return;
        }

        foreach (Waypoint adjacent in waypoint.adjacentWaypoints)
        {
            waypointQueue.Enqueue(adjacent);
        }
    }

    // Get the next waypoint in the BFS traversal
    public Waypoint GetNextWaypoint()
    {
        if (waypointQueue.Count == 0)
        {
            Debug.LogWarning("No more waypoints in the path.");
            return null;
        }

        return waypointQueue.Dequeue();
    }
}
