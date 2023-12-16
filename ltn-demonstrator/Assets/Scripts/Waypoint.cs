using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class Waypoint : MonoBehaviour
{
    public List<Waypoint> adjacentWaypoints; // Manually set in the editor

    private void OnDrawGizmos()
    {
        float waypointSize = this.transform.parent.GetComponent<Graph>().WaypointSize;

        // Draw the gizmo for this waypoint
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, waypointSize);

        // Draw lines to adjacent waypoints in red
        foreach (Waypoint adjacent in adjacentWaypoints)
        {
            if (adjacent != null)
            {
                // if the adjacent waypoint is selected, draw the line in yellow, and if not, draw it in red
                Gizmos.color = Selection.Contains(adjacent.gameObject) ? Color.yellow : Color.red;
                Gizmos.DrawLine(transform.position, adjacent.transform.position);
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
    }

    public void AddAdjacentWaypoint(Waypoint newAdjacent)
    {
        if (!adjacentWaypoints.Contains(newAdjacent))
        {
            adjacentWaypoints.Add(newAdjacent);
        }

        if (!newAdjacent.adjacentWaypoints.Contains(this))
        {
            newAdjacent.adjacentWaypoints.Add(this);
        }
    }
}
