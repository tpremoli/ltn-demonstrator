using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class RoadLoader : EditorWindow
{
    private static Material roadMaterial;

    private static Material dashMaterial;

    private static Material curbMaterial;

    // Road width and height (thickness)
    static float roadWidth = 4.0f;
    static float roadHeight = 0.2f;
    static float intersectionSize = 0.4f;

    // Road markings
    static float dashSize = 0.75f; // Size of each dash
    static float dashInterval = 1.0f; // Interval between dashes
    static float noDashZoneRadius = 2.0f; // Distance from waypoints where no dashes should be created

    // curb dimensions
    static float curbWidth = 1f; // controlling how wide the curb is
    static float curbHeight = 0.1f; // controlling how tall the curb is
    static float curbVerticalOffset = -0.1f; // controlling how much the curb curves vertically

    // Intersection shape
    static PrimitiveType intersectionShape = PrimitiveType.Cube;


    [MenuItem("Tools/Roads/Load Road Objects")]
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

        curbMaterial = new Material(Shader.Find("Standard"));
        curbMaterial.color = Color.gray;

        // then create the roads
        generateRoadsFromEdges(graph, roadManager);
        generateIntersections(roadManager);

        // mark scene as dirty so it saves
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
    }

    [MenuItem("Tools/Roads/Clear Road Objects")]
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

        foreach (Edge currentEdge in graph.GetAllEdges())
        {
            if (currentEdge.isPedestrianOnly) continue;

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

            generateRoadMarkings(currentEdge, roadObject);
            generateCurb(roadObject, currentEdge, direction, midPoint);
        }
    }

    private static void generateIntersections(GameObject roadManager)
    {
        List<Waypoint> waypoints = new List<Waypoint>(FindObjectsOfType<Waypoint>());
        foreach (Waypoint waypoint in waypoints)
        {
            if (waypoint.isPedestrianOnly) continue;

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

                generateIntersectionCurb(waypoint, intersectionObject);
            }
        }
    }

    private static void generateRoadMarkings(Edge edge, GameObject roadObject)
    {
        int dashCount = Mathf.FloorToInt(edge.length / (dashSize + dashInterval)); // Calculate how many dashes fit in the road

        // Calculate the total space occupied by dashes and intervals
        float totalDashesLength = dashCount * dashSize + (dashCount - 1) * dashInterval;
        float startLerp = (edge.length - totalDashesLength) / 2 / edge.length; // Starting point for lerp
        float endLerp = 1 - startLerp; // Ending point for lerp

        for (int i = 0; i < dashCount; i++)
        {
            // Calculate the lerp factor for each dash
            float lerpFactor = startLerp + (i * (dashSize + dashInterval) / edge.length);

            // Check if the dash is within the 'no-dash zone' of either waypoint
            bool isNearStartPoint = lerpFactor < (startLerp + noDashZoneRadius / edge.length);
            bool isNearEndPoint = lerpFactor > (endLerp - noDashZoneRadius / edge.length);

            if (isNearStartPoint || isNearEndPoint)
            {
                continue; // Skip creating the dash if it's too close to a waypoint
            }

            // Calculate position for each dash using linear interpolation
            Vector3 dashPosition = Vector3.Lerp(edge.startWaypoint.transform.position, edge.endWaypoint.transform.position, lerpFactor);

            GameObject dash = GameObject.CreatePrimitive(PrimitiveType.Cube);
            dash.name = "Dash";
            dash.transform.localScale = new Vector3(0.1f, 0.05f, dashSize); // Scale for the dash
            dash.GetComponent<MeshRenderer>().material = dashMaterial; // Assign a white material

            dash.transform.position = dashPosition;
            dash.transform.rotation = roadObject.transform.rotation;
            dash.transform.parent = roadObject.transform;
        }
    }

    private static void generateCurb(GameObject roadSegment, Edge edge, Vector3 direction, Vector3 midPoint)
    {
        // Create two curb objects, one for each side of the road
        for (int i = 0; i < 2; i++)
        {
            GameObject curb = GameObject.CreatePrimitive(PrimitiveType.Cube);
            curb.name = "Curb";

            // Scale the curb
            curb.transform.localScale = new Vector3(curbWidth, curbHeight, Vector3.Distance(edge.startWaypoint.transform.position, edge.endWaypoint.transform.position));

            // Position the curb
            Vector3 offsetDirection = Vector3.Cross(direction, Vector3.up).normalized * (roadWidth / 2 + curbWidth / 2);
            Vector3 curbPosition = midPoint + offsetDirection * (i == 0 ? 1 : -1);
            curb.transform.position = curbPosition;

            // Raise the curb above the road surface
            curb.transform.position += new Vector3(0, curbVerticalOffset, 0);

            // Rotate the curb to align with the road
            curb.transform.rotation = Quaternion.LookRotation(direction, Vector3.up);

            // Apply material (assuming curbMaterial is predefined)
            curb.GetComponent<MeshRenderer>().material = curbMaterial;

            // Set the parent of the curb object to the roadManager
            curb.transform.parent = roadSegment.transform;
        }
    }

    private static void generateIntersectionCurb(Waypoint intersectionPoint, GameObject intersection)
    {
        // Define the size of the intersection curb
        float intersectionCurbSize = roadWidth + 2 * curbWidth;

        // Create a curb object for the intersection
        GameObject intersectionCurb = GameObject.CreatePrimitive(PrimitiveType.Cube);
        intersectionCurb.name = "IntersectionCurb";

        // Scale the curb to enclose the intersection
        intersectionCurb.transform.localScale = new Vector3(intersectionCurbSize, curbHeight, intersectionCurbSize);

        // Position the curb at the intersection
        Vector3 position = intersectionPoint.transform.position;
        intersectionCurb.transform.position = position + new Vector3(0, curbVerticalOffset, 0); // raising the curb above the road

        // Apply material to the curb
        intersectionCurb.GetComponent<MeshRenderer>().material = curbMaterial;

        // Set the parent of the intersection curb to the roadManager
        intersectionCurb.transform.parent = intersection.transform;
    }
}
