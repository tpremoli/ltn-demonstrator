using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class EdgeLoader
{
    [InitializeOnLoadMethod]
    public static void LoadEdges()
    {
        Graph graph = Object.FindObjectOfType<Graph>();
        graph.edges = new List<Edge>();
        if (graph == null)
        {
            Debug.LogError("No graph found in scene.");
            return;
        }
        Waypoint[] waypoints = Object.FindObjectsOfType<Waypoint>();

        foreach (Waypoint waypoint in waypoints)
        {
            foreach (Waypoint adjacentWaypoint in waypoint.adjacentWaypoints)
            {
                float distance = graph.CalculateDistance(waypoint, adjacentWaypoint);
                // instantiate edge as empty game object
                Edge edge = new Edge(waypoint, adjacentWaypoint);
                graph.edges.Add(edge);
            }
        }
        Debug.Log("Calculated " + graph.edges.Count + " edges.");
    }
}
