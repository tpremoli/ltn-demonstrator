using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class Waypoint : MonoBehaviour
{
    // List of bidirectional connections
    public List<Waypoint> adjacentWaypoints;

    // List of one-way connections
    public List<Waypoint> singleConnectionWaypoints;

    private void OnDrawGizmos()
    {
        float waypointSize = this.transform.parent.GetComponent<Graph>().WaypointSize;

        // Draw the gizmo for this waypoint
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, waypointSize);

        // Draw lines to bidirectional adjacent waypoints in red
        foreach (Waypoint adjacent in adjacentWaypoints)
        {
            if (adjacent != null)
            {
                Gizmos.color = Selection.Contains(adjacent.gameObject) ? Color.yellow : Color.red;
                Gizmos.DrawLine(transform.position, adjacent.transform.position);
            }
        }

        // Draw lines to one-way connected waypoints in magenta
        foreach (Waypoint singleConnection in singleConnectionWaypoints)
        {
            if (singleConnection != null)
            {
                Gizmos.color = Selection.Contains(singleConnection.gameObject) ? Color.yellow : Color.magenta;
                Gizmos.DrawLine(transform.position, singleConnection.transform.position);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        float waypointSize = this.transform.parent.GetComponent<Graph>().WaypointSize;

        // Change the gizmo color when selected
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, waypointSize);

        // Optionally, draw connections in a different color when selected
        Gizmos.color = Color.yellow;
        foreach (Waypoint adjacent in adjacentWaypoints)
        {
            if (adjacent != null)
            {
                Gizmos.DrawLine(transform.position, adjacent.transform.position);
            }
        }

        // Draw one-way connections in yellow when selected
        Gizmos.color = Color.yellow;
        foreach (Waypoint singleConnection in singleConnectionWaypoints)
        {
            if (singleConnection != null)
            {
                Gizmos.DrawLine(transform.position, singleConnection.transform.position);
            }
        }
    }

    public void AddAdjacentWaypoint(Waypoint newAdjacent, bool isSingleConnection = false)
    {
        if (adjacentWaypoints == null)
        {
            adjacentWaypoints = new List<Waypoint>();
        }
        if (singleConnectionWaypoints == null)
        {
            singleConnectionWaypoints = new List<Waypoint>();
        }

        if (isSingleConnection)
        {
            // Add to the single connection list without checking bidirectional
            singleConnectionWaypoints.Add(newAdjacent);
        }
        else
        {
            // Add bidirectional connection
            if (!adjacentWaypoints.Contains(newAdjacent))
            {
                adjacentWaypoints.Add(newAdjacent);
            }

            // Ensure bidirectional connection
            if (!newAdjacent.adjacentWaypoints.Contains(this))
            {
                newAdjacent.adjacentWaypoints.Add(this);
            }
        }
    }
}
