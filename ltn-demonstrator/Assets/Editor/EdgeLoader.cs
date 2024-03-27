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

    public static void LoadEdges(Dictionary<ReducedEdge, List<ReducedEdge>> intersectingEdgesOverride = null)
    {
        // Find the Graph object in the scene
        Graph graph = Object.FindObjectOfType<Graph>();

        // If no Graph object is found, log an error and return
        if (graph == null)
        {
            Debug.LogError("No graph found in scene.");
            return;
        }

        // Initialize the edges list in the Graph object
        graph.ResetEdges();

        // Find all Waypoint objects in the scene
        Waypoint[] waypoints = Object.FindObjectsOfType<Waypoint>();

        // For each Waypoint object
        foreach (Waypoint waypoint in waypoints)
        {
            // For each adjacent waypoint
            foreach (Waypoint adjacentWaypoint in waypoint.adjacentWaypoints)
            {
                // Create a new Edge object with the waypoint and the adjacent waypoint
                Edge edge = new Edge(waypoint, adjacentWaypoint);
                graph.AddEdge(edge);
            }
        }
        Debug.Log("Calculated " + graph.GetAllEdges().Count + " edges.");

        if (intersectingEdgesOverride != null)
        {
            Debug.Log("Overriding intersecting edges.");

            // these edges will be stale, however, we can get the updated edges from the graph
            foreach (KeyValuePair<ReducedEdge, List<ReducedEdge>> kvp in intersectingEdgesOverride)
            {
                Edge keyEdge = graph.GetEdge(kvp.Key.startWaypoint, kvp.Key.endWaypoint);
                if (keyEdge == null) continue;
                foreach (ReducedEdge reducedEdge in kvp.Value)
                {
                    Edge intersecting = graph.GetEdge(reducedEdge.startWaypoint, reducedEdge.endWaypoint);
                    if (intersecting == null) continue;
                    keyEdge.RegisterIntersectingEdge(intersecting);
                }
            }
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
        // LoadEdges();
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    public static void InitializeOnLoad()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // LoadEdges();
    }
}


