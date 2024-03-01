using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class Waypoint : MonoBehaviour
{
    // TODO: This is the "adjacent" waypoints, which signifies the mutual connection between two waypoints.
    // We can add a second list of "connections" which are one-way connections, and use that to create one 
    // way streets.
    // As it is, all adjacent waypoints are two-way connections.
    public int ID = -1;
    public List<Waypoint> adjacentWaypoints;

    // New attribute to determine if the waypoint is for pedestrians only
    public bool isPedestrianOnly = false;
    private void OnDrawGizmos()
    {
        float waypointSize = this.transform.parent.GetComponent<Graph>().WaypointSize;
        if (this.isPedestrianOnly)
        {
            waypointSize = waypointSize / 2;
        }

        // Draw the gizmo for this waypoint
        Gizmos.color = isPedestrianOnly ? Color.green : Color.blue; // Green for pedestrian, blue otherwise
        Gizmos.DrawWireSphere(transform.position, waypointSize);

        // Draw lines to adjacent waypoints in red
        foreach (Waypoint adjacent in adjacentWaypoints)
        {
            if (adjacent != null)
            {
                if (adjacent.isPedestrianOnly)
                {
                    Gizmos.color = Color.green;
                    Handles.DrawDottedLine(transform.position, adjacent.transform.position, 5f);
                }
                else
                {
                    // if the adjacent waypoint is selected, draw the line in yellow, and if not, draw it in red
                    Gizmos.color = Selection.Contains(adjacent.gameObject) ? Color.yellow : Color.red;
                    Gizmos.DrawLine(transform.position, adjacent.transform.position);
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        float waypointSize = this.transform.parent.GetComponent<Graph>().WaypointSize;
        if (this.isPedestrianOnly)
        {
            waypointSize = waypointSize / 2;
        }


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
