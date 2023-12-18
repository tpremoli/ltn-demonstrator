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
        // Initialize distance dictionary and previous waypoint dictionary
        Dictionary<Waypoint, float> dist = new Dictionary<Waypoint, float>();
        Dictionary<Waypoint, Waypoint> prev = new Dictionary<Waypoint, Waypoint>();

        // Initialize all distances as infinity and prev as null
        foreach (Waypoint waypoint in graph.waypoints)
        {
            dist[waypoint] = float.MaxValue;
            prev[waypoint] = null;
        }

        // Setup start distances based on the edge proximity
        dist[startEdge.StartWaypoint] = Vector3.Distance(startEdge.StartWaypoint.transform.position, beginningPos);
        dist[startEdge.EndWaypoint] = Vector3.Distance(startEdge.EndWaypoint.transform.position, beginningPos);

        // Queue to select the waypoint with the smallest distance
        Queue<Waypoint> queue = new Queue<Waypoint>();
        queue.Enqueue(startEdge.StartWaypoint);
        queue.Enqueue(startEdge.EndWaypoint);

        while (queue.Count > 0)
        {
            Waypoint current = queue.Dequeue();

            if (current == endEdge.StartWaypoint || current == endEdge.EndWaypoint)
            {
                break;
            }

            // Explore adjacent waypoints
            foreach (Waypoint neighbor in current.adjacentWaypoints)
            {
                float alt = dist[current] + Vector3.Distance(current.transform.position, neighbor.transform.position);
                if (alt < dist[neighbor])
                {
                    dist[neighbor] = alt;
                    prev[neighbor] = current;
                    queue.Enqueue(neighbor);  // Re-enqueue with updated distance
                }
            }
        }

        // Construct the shortest path
        return ConstructPath(prev, endEdge);
    }

    private List<Waypoint> ConstructPath(Dictionary<Waypoint, Waypoint> prev, Edge endEdge)
    {
        List<Waypoint> path = new List<Waypoint>();
        Waypoint current = endEdge.StartWaypoint;

        // Choose the end waypoint based on which is closer to the destination position
        if (Vector3.Distance(endEdge.EndWaypoint.transform.position, destinationPos) < Vector3.Distance(endEdge.StartWaypoint.transform.position, destinationPos))
        {
            current = endEdge.EndWaypoint;
        }

        // Trace back the path using the prev dictionary
        while (current != null)
        {
            path.Add(current);
            current = prev[current];
        }

        path.Reverse();
        return path;
    }

    // Get the next waypoint in the BFS traversal
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
