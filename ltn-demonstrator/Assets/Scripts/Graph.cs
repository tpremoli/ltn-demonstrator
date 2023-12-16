using UnityEngine;

public class Graph : MonoBehaviour
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

                // Draw lines to adjacent waypoints in red
                Gizmos.color = Color.red;
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

    private void OnDrawGizmosSelected()
    {
        foreach (Transform child in transform)
        {
            Waypoint waypoint = child.GetComponent<Waypoint>();
            if (waypoint != null)
            {
                // Change the gizmo color when selected
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(child.position, waypointSize);

                // Optionally, draw connections in a different color when selected
                Gizmos.color = Color.yellow;
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
