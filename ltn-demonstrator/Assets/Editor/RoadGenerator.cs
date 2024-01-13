using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class RoadLoader : EditorWindow
{
    private static Material roadMaterial;

    // Road width and height (thickness)
    static float roadWidth = 4.0f;
    static float roadHeight = 0.2f;


    [MenuItem("Tools/Load Road Objects")]
    public static void LoadRoads()
    {
        // first reload the edges
        EdgeLoader.LoadEdges();
        Graph graph = Object.FindFirstObjectByType<Graph>();

        // first clear the old roads
        GameObject oldRoadManager = GameObject.Find("RoadGraphicsManager");
        if (oldRoadManager != null)
        {
            DestroyImmediate(oldRoadManager);
        }

        // we make a RoadGraphicsManager gameobject to hold all the roads
        GameObject roadManager = new GameObject("RoadGraphicsManager");
        roadManager.transform.position = Vector3.zero;

        // create a material for the roads
        roadMaterial = new Material(Shader.Find("Standard"));
        roadMaterial.color = Color.black;

        // then create the roads
        foreach (Edge edge in graph.edges)
        {
            generateRoadForEdge(edge, roadManager);
        }
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
    }

    [MenuItem("Tools/Clear Road Objects")]
    public static void ClearRoads()
    {
        GameObject oldRoadManager = GameObject.Find("RoadGraphicsManager");
        if (oldRoadManager != null)
        {
            DestroyImmediate(oldRoadManager);
        }
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
    }

    private static void generateRoadForEdge(Edge edge, GameObject roadManager)
    {
        Vector3 startPoint = edge.startWaypoint.transform.position;
        Vector3 endPoint = edge.endWaypoint.transform.position;

        // Calculate direction and length of the road segment
        Vector3 direction = (endPoint - startPoint).normalized;
        float length = Vector3.Distance(startPoint, endPoint);

        // Midpoint for positioning the road object
        Vector3 midPoint = (startPoint + endPoint) / 2;

        // Instantiate a primitive cube to represent the road
        GameObject roadObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
        roadObject.name = "RoadSegment";

        // Scale the cube to match the road dimensions
        roadObject.transform.localScale = new Vector3(roadWidth, roadHeight, length);

        // Position and rotate the road object
        roadObject.transform.position = midPoint;
        roadObject.transform.rotation = Quaternion.LookRotation(direction, Vector3.up);

        // Lower the road by half of its height to align it with the ground
        roadObject.transform.position -= new Vector3(0, roadHeight / 2, 0);

        // Apply a material to the mesh renderer for visual appearance
        roadObject.GetComponent<MeshRenderer>().material = roadMaterial;

        roadObject.transform.parent = roadManager.transform;

    }
}
