using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class WaypointConnector : EditorWindow
{
    [MenuItem("Tools/Connect Waypoints")]
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

        // Mark all modified objects as dirty so the changes are saved
        foreach (Waypoint waypoint in allWaypoints)
        {
            EditorUtility.SetDirty(waypoint);
        }

        Debug.Log("Waypoints connections updated.");
    }
}
