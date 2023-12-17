using UnityEngine;
using System.Collections.Generic;

public class WaypointPath
{
    private Queue<Waypoint> waypointQueue;

    public List<Waypoint> path;

    public List<Waypoint> BFS(Waypoint startpoint, Waypoint endpoint)
    {
        List<Waypoint> path = new List<Waypoint>();
        Queue<Waypoint> queue = new Queue<Waypoint>();
        Dictionary<Waypoint, Waypoint> cameFrom = new Dictionary<Waypoint, Waypoint>();
        Dictionary<Waypoint, int> costSoFar = new Dictionary<Waypoint, int>();

        queue.Enqueue(startpoint);
        cameFrom[startpoint] = null;
        costSoFar[startpoint] = 0;

        while (queue.Count > 0)
        {
            Waypoint current = queue.Dequeue();

            if (current == endpoint)
            {
                break;
            }

            foreach (Waypoint next in current.adjacentWaypoints)
            {
                int newCost = costSoFar[current] + 1;
                if (!costSoFar.ContainsKey(next) || newCost < costSoFar[next])
                {
                    costSoFar[next] = newCost;
                    queue.Enqueue(next);
                    cameFrom[next] = current;
                }
            }
        }

        Waypoint currentWaypoint = endpoint;
        while (currentWaypoint != startpoint)
        {
            path.Add(currentWaypoint);
            currentWaypoint = cameFrom[currentWaypoint];
        }

        path.Reverse();
        return path;
    }

    public WaypointPath(Waypoint startingPoint, Waypoint endPoint)
    {
        this.path = BFS(startingPoint, endPoint);
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
