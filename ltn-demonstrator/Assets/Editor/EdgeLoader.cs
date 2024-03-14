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
        Graph graph = Object.FindFirstObjectByType<Graph>();

        if (graph == null)
        {
            Debug.LogError("No graph found in scene.");
            return;
        }

        graph.ResetEdges();
        Waypoint[] waypoints = Object.FindObjectsOfType<Waypoint>();

        foreach (Waypoint waypoint in waypoints)
        {
            foreach (Waypoint adjacentWaypoint in waypoint.adjacentWaypoints)
            {
                float distance = graph.CalculateDistance(waypoint, adjacentWaypoint);
                // instantiate edge as empty game object
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
                foreach (ReducedEdge reducedEdge in kvp.Value)
                {
                    keyEdge.RegisterIntersectingEdge(graph.GetEdge(reducedEdge.startWaypoint, reducedEdge.endWaypoint));
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


