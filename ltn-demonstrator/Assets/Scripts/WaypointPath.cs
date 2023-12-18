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

        // Choose the endpoint based on proximity to destinationPos
        if (Vector3.Distance(endEdge.EndWaypoint.transform.position, destinationPos) < 
            Vector3.Distance(endEdge.StartWaypoint.transform.position, destinationPos))
        {
            current = endEdge.EndWaypoint;
        }
        else
        {
            current = endEdge.StartWaypoint;
        }

        // Trace back the path using the prev dictionary
        while (current != null && prev.ContainsKey(current))
        {
            path.Add(current);
            current = prev[current];
        }

        path.Reverse();
        return path;
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
