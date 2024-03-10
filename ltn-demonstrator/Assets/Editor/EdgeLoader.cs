using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class EdgeLoader
{
    // [InitializeOnLoadMethod]
    // [RuntimeInitializeOnLoadMethod]
    [MenuItem("Tools/Reload Edges")]
    public static void LoadEdgesOnStart()
    {
        // This method will be called by Unity and should not have parameters.
        // You can have it call another method with default parameters if needed.
        LoadEdges();
    }

    public static void LoadEdges(Dictionary<Edge, List<Edge>> intersectingEdgesOverride = null)
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
                graph.edges.Add(edge);
            }
        }
        Debug.Log("Calculated " + graph.edges.Count + " edges.");

        if (intersectingEdgesOverride != null)
        {
            Debug.Log("Overriding intersecting edges.");

            // these edges will be stale, however, we can get the updated edges from the graph
            foreach (KeyValuePair<Edge, List<Edge>> kvp in intersectingEdgesOverride)
            {
                Edge keyEdge = graph.getEdge(kvp.Key.startWaypoint, kvp.Key.endWaypoint);
                foreach (Edge edge in kvp.Value)
                {
                    keyEdge.RegisterIntersectingEdge(graph.getEdge(edge.startWaypoint, edge.endWaypoint));
                }
            }

            Debug.Log("Overridden intersecting edges.");
        }
    }

    // Constructor static method will be called in both editor and play mode
    static EdgeLoader()
    {
        // For Editor: Listen to scene opened event
        EditorSceneManager.sceneOpened += SceneOpenedCallback;
    }

    // This method will be called whenever a scene is opened in the Editor
    private static void SceneOpenedCallback(Scene scene, OpenSceneMode mode)
    {
        LoadEdges();
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    public static void InitializeOnLoad()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        LoadEdges();
    }
}


