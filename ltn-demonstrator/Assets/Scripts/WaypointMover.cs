using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointMover : MonoBehaviour
{
    [SerializeField] private Waypoint startingWaypoint;  // Initial waypoint to start the movement
    [SerializeField] private float speed = 5f;           // Movement speed
    [SerializeField] private float distanceThreshold = 0.1f;  // Distance threshold to consider reaching a waypoint

    private WaypointPath path;           // Instance of the pathfinding class
    private Waypoint currentWaypoint;    // Current waypoint the mover is heading towards

    void Start()
    {
        Waypoint startingPoint = GameObject.Find("Waypoint (10)").GetComponent<Waypoint>();
        Waypoint endPoint = GameObject.Find("Waypoint (14)").GetComponent<Waypoint>();

        // Initialize the path with the starting waypoint
        path = new WaypointPath(startingPoint, endPoint);
        
        // Get the first waypoint in the path and set the initial position
        currentWaypoint = path.GetNextWaypoint();
        transform.position = currentWaypoint.transform.position;
    }

    void Update()
    {
        if (currentWaypoint == null) return;  // Stop if there's no current waypoint

        // Move towards the current waypoint using linear interpolation
        transform.position = Vector3.MoveTowards(transform.position, currentWaypoint.transform.position, speed * Time.deltaTime);

        // Check if the distance to the current waypoint is below the threshold
        if (Vector3.Distance(transform.position, currentWaypoint.transform.position) < distanceThreshold)
        {
            // If the threshold is met, get the next waypoint in the path
            currentWaypoint = path.GetNextWaypoint();
        }

    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.magenta;

        List<Waypoint> waypoints = path.path;

        if (path != null)
        {
            foreach (Waypoint waypoint in waypoints)
            {
                Gizmos.DrawSphere(waypoint.transform.position, 1f);
            }

            // also draw the lines between the waypoints
            for (int i = 0; i < waypoints.Count - 1; i++)
            {
                Gizmos.DrawLine(waypoints[i].transform.position, waypoints[i + 1].transform.position);
            }
        }
    }


}
