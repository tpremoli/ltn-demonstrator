using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoints : MonoBehaviour
{
    [Range(0f, 2f)]
    [SerializeField] private float waypointSize = 1f;

    // List of valid waypoints
    [SerializeField] private List<Transform> validWaypoints = new List<Transform>();

    private void OnDrawGizmos()
    {
        // Draw Blue Sphere for all Waypoints
        foreach (Transform t in transform)
        {
            Gizmos.color = validWaypoints.Contains(t) ? Color.blue : Color.yellow;
            Gizmos.DrawWireSphere(t.position, waypointSize);
        }

        // Draw connections between valid waypoints (in blue) and all other waypoints (in yellow)
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform currentWaypoint = transform.GetChild(i);
            Gizmos.color = validWaypoints.Contains(currentWaypoint) ? Color.blue : Color.yellow;

            for (int j = 0; j < transform.childCount; j++)
            {
                Transform otherWaypoint = transform.GetChild(j);

                // Draw a line if the current and other waypoints are both valid or both invalid
                if (validWaypoints.Contains(currentWaypoint) == validWaypoints.Contains(otherWaypoint))
                {
                    Gizmos.DrawLine(currentWaypoint.position, otherWaypoint.position);
                }
            }
        }
    }

    public Transform GetNextWaypoint(Transform currentWaypoint)
    {
        if (validWaypoints.Count == 0 || currentWaypoint == null)
        {
            Debug.LogWarning("No valid waypoints defined or current waypoint is null.");
            return null;
        }

        int index = validWaypoints.IndexOf(currentWaypoint);

        if (index != -1)
        {
            // Get the next waypoint in the valid waypoints list
            int nextIndex = (index + 1) % validWaypoints.Count;
            return validWaypoints[nextIndex];
        }

        Debug.LogWarning("Current waypoint not found in the list of valid waypoints.");
        return null;
    }
}
