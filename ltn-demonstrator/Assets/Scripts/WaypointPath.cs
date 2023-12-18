using UnityEngine;
using System.Collections.Generic;
using Utils;

public class WaypointPath
{
    public List<Waypoint> path;
    private Graph graph;

    public Vector3 beginningPos;
    public Vector3 destinationPos;

    public Edge startEdge;
    public Edge endEdge;

    public WaypointPath(Vector3 beginningPos, Vector3 destinationPos)
    {
        this.graph = GameObject.Find("Graph").GetComponent<Graph>();
        this.beginningPos = beginningPos;
        this.destinationPos = destinationPos;

        this.startEdge = graph.getClosetEdge(beginningPos);
        this.endEdge = graph.getClosetEdge(destinationPos);


        if (PathExists()){
            this.path = Dijkstra();
        }else{
            Debug.LogWarning("No path exists");
        }
    }

    public List<Waypoint> Dijkstra()
    {
        // Check if start and end are on the same edge to handle this special case
        if (startEdge.isSameEdge(endEdge))
        {
            // Return an empty path if starting and ending on the same edge
            return new List<Waypoint>();
        }

        // Initialize dictionaries to store distances, previous waypoints, and the most recent distances
        Dictionary<Waypoint, float> dist = new Dictionary<Waypoint, float>();
        Dictionary<Waypoint, Waypoint> prev = new Dictionary<Waypoint, Waypoint>();
        Dictionary<Waypoint, float> mostRecentDistances = new Dictionary<Waypoint, float>();

        // Set initial distances to all waypoints as infinite and previous waypoints as null
        foreach (Waypoint waypoint in graph.waypoints)
        {
            dist[waypoint] = float.MaxValue;
            prev[waypoint] = null;
        }

        // Initialize the distances for the start waypoints on the start edge
        dist[startEdge.StartWaypoint] = Vector3.Distance(startEdge.StartWaypoint.transform.position, beginningPos);
        dist[startEdge.EndWaypoint] = Vector3.Distance(startEdge.EndWaypoint.transform.position, beginningPos);

        // Priority queue to manage waypoints based on their current shortest distance
        PriorityQueue<Waypoint, float> queue = new PriorityQueue<Waypoint, float>();

        // Enqueue the start waypoints and update their most recent distances
        queue.Enqueue(startEdge.StartWaypoint, dist[startEdge.StartWaypoint]);
        mostRecentDistances[startEdge.StartWaypoint] = dist[startEdge.StartWaypoint];

        queue.Enqueue(startEdge.EndWaypoint, dist[startEdge.EndWaypoint]);
        mostRecentDistances[startEdge.EndWaypoint] = dist[startEdge.EndWaypoint];

        // Process each waypoint in the queue
        while (queue.Count > 0)
        {
            Waypoint current = queue.Dequeue();

            // Skip processing if the current distance is not the most recent
            if (mostRecentDistances[current] != dist[current])
            {
                continue;
            }

            // Explore all adjacent waypoints of the current waypoint
            foreach (Waypoint neighbor in current.adjacentWaypoints)
            {
                // Calculate the alternative distance to this neighbor
                float alt = dist[current] + Vector3.Distance(current.transform.position, neighbor.transform.position);

                // If the alternative distance is shorter, update the distance and previous waypoint
                if (alt < dist[neighbor])
                {
                    dist[neighbor] = alt;
                    prev[neighbor] = current;

                    // Enqueue the neighbor with the updated distance
                    queue.Enqueue(neighbor, alt);
                    mostRecentDistances[neighbor] = alt; // Update the most recent distance
                }
            }
        }

        // If a path exists, construct the path
        return ConstructPath(prev, endEdge);
    }

    private List<Waypoint> ConstructPath(Dictionary<Waypoint, Waypoint> prev, Edge endEdge)
    {
        List<Waypoint> path = new List<Waypoint>();

        // Determine the endpoint that leads most directly to the destination
        Waypoint closerEndpoint = ClosestWaypointOnEdge(endEdge, destinationPos);

        // Check if a path exists to the closer endpoint
        if (!prev.ContainsKey(closerEndpoint))
        {   
            Debug.LogWarning("No path exists to the closer endpoint");
            return null; // or any other appropriate response
        }

        // If a path exists, construct the path from the closer endpoint
        Waypoint current = closerEndpoint;
        while (current != null)
        {
            path.Add(current);
            current = prev.ContainsKey(current) ? prev[current] : null;
        }

        path.Reverse(); // Reverse the path to start from the beginning

        // Refine the path to avoid overshooting
        if (path.Count >= 2)
        {
            // Check if the destination is between the last two waypoints in the path
            Waypoint lastWaypoint = path[path.Count - 1];
            Waypoint secondLastWaypoint = path[path.Count - 2];

            if (IsDestinationBetween(destinationPos, lastWaypoint.transform.position, secondLastWaypoint.transform.position))
            {
                path.RemoveAt(path.Count - 1); // Remove the last waypoint if it overshoots the destination
            }
        }

        return path;
    }

    // Helper method to determine if the destination is between two waypoints
    private bool IsDestinationBetween(Vector3 destination, Vector3 last, Vector3 secondLast)
    {
        float totalDistance = Vector3.Distance(last, secondLast);
        float distanceToLast = Vector3.Distance(destination, last);
        float distanceToSecondLast = Vector3.Distance(destination, secondLast);

        return distanceToLast + distanceToSecondLast <= totalDistance + 0.1f; // Add a small tolerance
    }

    private Waypoint ClosestWaypointOnEdge(Edge edge, Vector3 position)
    {
        if (!edge.isPointOnEdge(position))
        {
            Debug.Log("Position is not on edge");
            return null;
        }
        // Determine the closest waypoint on the given edge
        float startDist = Vector3.Distance(position, edge.StartWaypoint.transform.position);
        float endDist = Vector3.Distance(position, edge.EndWaypoint.transform.position);
        return startDist < endDist ? edge.StartWaypoint : edge.EndWaypoint;
    }

    // Get the next waypoint in the traversal
    public Waypoint GetNextWaypoint()
    {
        if (path == null || path.Count == 0)
        {
            Debug.Log("Finished path, moving to destination point now.");
            return null;
        }
        Waypoint nextWaypoint = path[0];
        path.RemoveAt(0);
        return nextWaypoint;
    }

    /// <summary>
    /// This method checks if a path exists between the start and end positions.
    /// </summary>
    /// <returns></returns>
    public bool PathExists()
    {
        Waypoint nearestStartWaypoint = ClosestWaypointOnEdge(startEdge, beginningPos);
        Waypoint nearestEndWaypoint = ClosestWaypointOnEdge(endEdge, destinationPos);

        // Initialize dictionaries for distances and previous waypoints
        Dictionary<Waypoint, float> dist = new Dictionary<Waypoint, float>();
        Dictionary<Waypoint, Waypoint> prev = new Dictionary<Waypoint, Waypoint>();

        // Set initial distances to all waypoints as infinite and previous waypoints as null
        foreach (Waypoint waypoint in graph.waypoints)
        {
            dist[waypoint] = float.MaxValue;
            prev[waypoint] = null;
        }

        // Set the distance for the start waypoint to zero
        dist[nearestStartWaypoint] = 0;

        // Priority queue to manage waypoints based on their current shortest distance
        PriorityQueue<Waypoint, float> queue = new PriorityQueue<Waypoint, float>();

        // Enqueue the start waypoint
        queue.Enqueue(nearestStartWaypoint, dist[nearestStartWaypoint]);

        // Process each waypoint in the queue
        while (queue.Count > 0)
        {
            Waypoint current = queue.Dequeue();

            // If the current waypoint is the end waypoint, return true indicating a path exists
            if (current == nearestEndWaypoint)
            {
                return true;
            }

            // Explore all adjacent waypoints of the current waypoint
            foreach ( Waypoint neighbor in current.adjacentWaypoints)
            {
                // Calculate the alternative distance to this neighbor
                float alt = dist[current] + Vector3.Distance(current.transform.position, neighbor.transform.position);

                // If the alternative distance is shorter, update the distance and previous waypoint
                if (alt < dist[neighbor])
                {
                    dist[neighbor] = alt;
                    prev[neighbor] = current;

                    // Enqueue the neighbor with the updated distance
                    queue.Enqueue(neighbor, alt);
                }
            }
        }

        // If the loop completes without finding the end waypoint, return false indicating no path exists
        return false;
    }
}
