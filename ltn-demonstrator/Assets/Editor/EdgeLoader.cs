using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class EdgeLoader
{
    [MenuItem("Tools/Reload Edges")]
    [InitializeOnLoadMethod]
    public static void LoadEdges()
    {
        Graph graph = GameObject.Find("Graph").GetComponent<Graph>();

        if (graph == null)
        {
            Debug.LogError("No graph found in scene.");
            return;
        }

        graph.edges = new List<Edge>();
        Waypoint[] waypoints = Object.FindObjectsOfType<Waypoint>();

        foreach (Waypoint waypoint in waypoints)
        {
            // Load bidirectional connections
            foreach (Waypoint adjacentWaypoint in waypoint.adjacentWaypoints)
            {
                float distance = graph.CalculateDistance(waypoint, adjacentWaypoint);
                Edge edge = new Edge(waypoint, adjacentWaypoint);
                graph.edges.Add(edge);
            }

            // Load single connections
            foreach (Waypoint singleConnectionWaypoint in waypoint.singleConnectionWaypoints)
            {
                float distance = graph.CalculateDistance(waypoint, singleConnectionWaypoint);
                Edge edge = new Edge(waypoint, singleConnectionWaypoint);
                graph.edges.Add(edge);
            }
        }

        Debug.Log("Calculated " + graph.edges.Count + " edges.");
    }
}
