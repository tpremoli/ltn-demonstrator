using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class Waypoint : MonoBehaviour
{
    // TODO: This is the "adjacent" waypoints, which signifies the mutual connection between two waypoints.
    // We can add a second list of "connections" which are one-way connections, and use that to create one 
    // way streets.
    // As it is, all adjacent waypoints are two-way connections.
    public List<Waypoint> adjacentWaypoints; 

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
        if (adjacentWaypoints == null)
        {
            adjacentWaypoints = new List<Waypoint>();
        }

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
