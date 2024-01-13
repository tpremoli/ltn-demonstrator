using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class RoadLoader : EditorWindow
{
    private static Material roadMaterial;

    private static Material dashMaterial;

    // Road width and height (thickness)
    static float roadWidth = 4.0f;
    static float roadHeight = 0.2f;
    static float intersectionSize = 0.4f;

    // Road markings
    static float dashSize = 0.75f; // Size of each dash
    static float dashInterval = 1.0f; // Interval between dashes
    static float noDashZoneRadius = 2.0f; // Distance from waypoints where no dashes should be created


    static PrimitiveType intersectionShape = PrimitiveType.Cube;


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

        dashMaterial = new Material(Shader.Find("Standard"));
        dashMaterial.color = Color.white;

        // then create the roads
        generateRoadsFromEdges(graph, roadManager);
        generateIntersections(roadManager);

        // mark scene as dirty so it saves
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

    private static void generateRoadsFromEdges(Graph graph, GameObject roadManager)
    {
        List<Edge> processedEdges = new List<Edge>();

        foreach (Edge currentEdge in graph.edges)
        {
            // we first check if we have already processed this edge, as we don't want to draw it twice
            bool isDuplicate = false;
            foreach (Edge processedEdge in processedEdges)
            {
                if (currentEdge.isSameEdge(processedEdge))
                {
                    isDuplicate = true;
                    break;
                }
            }
            if (isDuplicate)
            {
                continue;
            }

            processedEdges.Add(currentEdge);

            // then we start generating the road for the edge
            Vector3 startPoint = currentEdge.startWaypoint.transform.position;
            Vector3 endPoint = currentEdge.endWaypoint.transform.position;

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

            generateRoadMarkings(graph, roadManager, length, midPoint, direction, roadObject);
        }
    }

    private static void generateIntersections(GameObject roadManager)
    {
        List<Waypoint> waypoints = new List<Waypoint>(FindObjectsOfType<Waypoint>());
        foreach (Waypoint waypoint in waypoints)
        {
            // Check if the waypoint is an intersection (3 or more connected edges)
            if (waypoint.adjacentWaypoints.Count >= 1)
            {
                // Instantiate a primitive cube to represent the intersection
                GameObject intersectionObject = GameObject.CreatePrimitive(intersectionShape);
                intersectionObject.name = "Intersection";

                // Scale the cube to be slightly larger than the road width
                float scale = roadWidth + intersectionSize; // intersectionSize adds extra width
                intersectionObject.transform.localScale = new Vector3(scale, roadHeight, scale);

                // Position the intersection object
                Vector3 position = waypoint.transform.position;
                intersectionObject.transform.position = position;
                intersectionObject.transform.position -= new Vector3(0, roadHeight / 2, 0);

                // Apply a material to the mesh renderer for visual appearance
                intersectionObject.GetComponent<MeshRenderer>().material = roadMaterial;

                // Set the parent of the intersection object to the roadManager
                intersectionObject.transform.parent = roadManager.transform;
            }
        }
    }

    private static void generateRoadMarkings(Graph graph, GameObject roadManager, float length, Vector3 midPoint, Vector3 direction, GameObject roadObject)
    {
        int dashCount = Mathf.FloorToInt(length / (dashSize + dashInterval)); // Calculate how many dashes fit in the road

        for (int i = 0; i < dashCount; i++)
        {
            // Calculate position for each dash
            float dashPos = (i * (dashSize + dashInterval)) - (length / 2) + (dashSize / 2);

            // Check if the dash is within the 'no-dash zone' of either waypoint
            bool isNearStartPoint = Mathf.Abs(dashPos - (-length / 2)) < noDashZoneRadius;
            bool isNearEndPoint = Mathf.Abs(dashPos - (length / 2)) < noDashZoneRadius;

            if (isNearStartPoint || isNearEndPoint)
            {
                continue; // Skip creating the dash if it's too close to a waypoint
            }

            GameObject dash = GameObject.CreatePrimitive(PrimitiveType.Cube);
            dash.name = "Dash";
            dash.transform.localScale = new Vector3(0.1f, 0.05f, dashSize); // Scale for the dash
            dash.GetComponent<MeshRenderer>().material = dashMaterial; // Assign a white material

            dash.transform.position = midPoint + (direction * dashPos);
            dash.transform.rotation = roadObject.transform.rotation;
            dash.transform.parent = roadObject.transform;
        }
    }
}
