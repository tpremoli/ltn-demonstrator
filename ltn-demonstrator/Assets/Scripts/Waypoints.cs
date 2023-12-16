using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoints : MonoBehaviour
{
    [Range(0f, 2f)]
    [SerializeField] private float waypointSize = 1f;

    private void OnDrawGizmos()
    {
        foreach (Transform child in transform)
        {
            Waypoint waypoint = child.GetComponent<Waypoint>();
            if (waypoint != null)
            {
                // Draw the gizmo for each waypoint
                Gizmos.color = Color.blue;
                Gizmos.DrawWireSphere(child.position, waypointSize);

                Gizmos.color = Color.red;
                // Draw lines to adjacent waypoints
                foreach (Waypoint adjacent in waypoint.adjacentWaypoints)
                {
                    if (adjacent != null)
                    {
                        Gizmos.DrawLine(child.position, adjacent.transform.position);
                    }
                }
            }
        }
    }
}
