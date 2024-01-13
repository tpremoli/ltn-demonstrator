using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

// TODO: make this  into a few different tools.
// this would allow us to have one way roads, and two way roads. To be seen in the future.
public class WaypointConnector : EditorWindow
{
    [MenuItem("Tools/Waypoints/Connect Hanging Waypoints")]
    private static void ConnectAllWaypoints()
    {
        // Find all Waypoints in the scene
        Waypoint[] allWaypoints = FindObjectsOfType<Waypoint>();

        foreach (Waypoint waypoint in allWaypoints)
        {
            if (waypoint.adjacentWaypoints != null)
            {
                foreach (Waypoint adjacentWaypoint in waypoint.adjacentWaypoints)
                {
                    // Ensure mutual connection
                    if (!adjacentWaypoint.adjacentWaypoints.Contains(waypoint))
                    {
                        adjacentWaypoint.adjacentWaypoints.Add(waypoint);
                    }
                }
            }
        }

        // Remove duplicate adjacent waypoints
        foreach (Waypoint waypoint in allWaypoints)
        {
            RemoveDuplicateAdjacentWaypoints(waypoint);
        }

        // Remove self connectionss
        foreach (Waypoint waypoint in allWaypoints)
        {
            RemoveSelfConnections(waypoint);
        }

        // Mark all modified objects as dirty so the changes are saved
        foreach (Waypoint waypoint in allWaypoints)
        {
            EditorUtility.SetDirty(waypoint);
        }

        // as we've changed the waypoints, we need to reload the edges
        EdgeLoader.LoadEdges();

        Debug.Log("Waypoints connections updated.");
    }

    private static void RemoveDuplicateAdjacentWaypoints(Waypoint waypoint)
    {
        if (waypoint.adjacentWaypoints == null)
            return;

        for (int i = waypoint.adjacentWaypoints.Count - 1; i >= 0; i--)
        {
            Waypoint adjacentWaypoint = waypoint.adjacentWaypoints[i];

            if (adjacentWaypoint == null || waypoint.adjacentWaypoints.FindAll(x => x == adjacentWaypoint).Count > 1)
            {
                waypoint.adjacentWaypoints.RemoveAt(i);
            }
        }
    }

    private static void RemoveSelfConnections(Waypoint waypoint)
    {
        if (waypoint.adjacentWaypoints == null)
            return;

        for (int i = waypoint.adjacentWaypoints.Count - 1; i >= 0; i--)
        {
            Waypoint adjacentWaypoint = waypoint.adjacentWaypoints[i];

            if (adjacentWaypoint == waypoint)
            {
                waypoint.adjacentWaypoints.RemoveAt(i);
            }
        }
    }
}
