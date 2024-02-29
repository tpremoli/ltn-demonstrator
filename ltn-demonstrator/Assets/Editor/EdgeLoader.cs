using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class EdgeLoader
{
    // Find the Graph object in the scene
    Graph graph = Object.FindObjectOfType<Graph>();
    
    [MenuItem("Tools/Reload Edges")]
    [InitializeOnLoadMethod]
    [RuntimeInitializeOnLoadMethod]
    public static void LoadEdges()
    {

        // If no Graph object is found, log an error and return
        if (graph == null)
        {
            Debug.LogError("No graph found in scene.");
            return;
        }

        // Initialize the edges list in the Graph object
        graph.edges = new List<Edge>();

        // Find all Waypoint objects in the scene
        Waypoint[] waypoints = Object.FindObjectsOfType<Waypoint>();

        // For each Waypoint object
        foreach (Waypoint waypoint in waypoints)
        {
            // For each adjacent waypoint
            foreach (Waypoint adjacentWaypoint in waypoint.adjacentWaypoints)
            {
                // Calculate the distance between the waypoint and the adjacent waypoint
                float distance = graph.CalculateDistance(waypoint, adjacentWaypoint);

                // Create a new Edge object with the waypoint and the adjacent waypoint
                Edge edge = new Edge(waypoint, adjacentWaypoint);

                // Add the new Edge object to the edges list in the Graph object
                graph.edges.Add(edge);
            }
        }

        // Log the number of edges calculated
        Debug.Log("Calculated " + graph.edges.Count + " edges.");
    }
}
