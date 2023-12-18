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

        this.path = Dijkstra();
    }

    public List<Waypoint> Dijkstra()
    {
        // Special case: start and end are on the same edge
        if (startEdge.isSameEdge(endEdge))
        {
            return new List<Waypoint>();
        }

        Dictionary<Waypoint, float> dist = new Dictionary<Waypoint, float>();
        Dictionary<Waypoint, Waypoint> prev = new Dictionary<Waypoint, Waypoint>();
        Dictionary<Waypoint, float> mostRecentDistances = new Dictionary<Waypoint, float>();

        foreach (Waypoint waypoint in graph.waypoints)
        {
            dist[waypoint] = float.MaxValue;
            prev[waypoint] = null;
        }

        dist[startEdge.StartWaypoint] = Vector3.Distance(startEdge.StartWaypoint.transform.position, beginningPos);
        dist[startEdge.EndWaypoint] = Vector3.Distance(startEdge.EndWaypoint.transform.position, beginningPos);

        PriorityQueue<Waypoint, float> queue = new PriorityQueue<Waypoint, float>();

        queue.Enqueue(startEdge.StartWaypoint, dist[startEdge.StartWaypoint]);
        mostRecentDistances[startEdge.StartWaypoint] = dist[startEdge.StartWaypoint];

        queue.Enqueue(startEdge.EndWaypoint, dist[startEdge.EndWaypoint]);
        mostRecentDistances[startEdge.EndWaypoint] = dist[startEdge.EndWaypoint];

        while (queue.Count > 0)
        {
            Waypoint current = queue.Dequeue();

            if (mostRecentDistances[current] != dist[current])
            {
                continue;
            }

            // Explore adjacent waypoints
            foreach (Waypoint neighbor in current.adjacentWaypoints)
            {
                float alt = dist[current] + Vector3.Distance(current.transform.position, neighbor.transform.position);
                if (alt < dist[neighbor])
                {
                    dist[neighbor] = alt;
                    prev[neighbor] = current;
                    queue.Enqueue(neighbor, alt);
                    mostRecentDistances[neighbor] = alt;
                }
            }
        }

        return ConstructPath(prev, endEdge);
    }

    private List<Waypoint> ConstructPath(Dictionary<Waypoint, Waypoint> prev, Edge endEdge)
    {
        List<Waypoint> path = new List<Waypoint>();
        Waypoint current = null;

        // Determine the endpoint that leads most directly to the destination
        Waypoint closerEndpoint = Vector3.Distance(endEdge.EndWaypoint.transform.position, destinationPos) < 
                                Vector3.Distance(endEdge.StartWaypoint.transform.position, destinationPos) 
                                ? endEdge.EndWaypoint : endEdge.StartWaypoint;

        // Construct the path from the closer endpoint
        current = closerEndpoint;

        while (current != null && prev.ContainsKey(current))
        {
            path.Add(current);
            current = prev[current];
        }

        path.Reverse();

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
            Debug.LogWarning("Path is null or empty.");
            return null;
        }
        Waypoint nextWaypoint = path[0];
        path.RemoveAt(0);
        return nextWaypoint;
    }
}
