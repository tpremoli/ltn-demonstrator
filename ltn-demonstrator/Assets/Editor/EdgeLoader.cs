using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class EdgeLoader
{
    [MenuItem("Tools/Reload Edges")]
    [InitializeOnLoadMethod]
    [RuntimeInitializeOnLoadMethod]
    public static void LoadEdges()
    {
        Graph graph = Object.FindFirstObjectByType<Graph>();

        
        if (graph == null)
        {
            Debug.LogError("No graph found in scene.");
            return;
        }
        

        graph.edges = new List<Edge>();
        Waypoint[] waypoints = Object.FindObjectsOfType<Waypoint>();

        foreach (Waypoint waypoint in waypoints)
        {
            foreach (Waypoint adjacentWaypoint in waypoint.adjacentWaypoints)
            {
                float distance = graph.CalculateDistance(waypoint, adjacentWaypoint);
                // instantiate edge as empty game object
                Edge edge = new Edge(waypoint, adjacentWaypoint);
                //SHIFT EDGE???
                graph.edges.Add(edge);
            }
        }
        Debug.Log("Calculated " + graph.edges.Count + " edges.");
    }
}
