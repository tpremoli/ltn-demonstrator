using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointMover : MonoBehaviour 
{
    [SerializeField] private Waypoints waypoints;
    [SerializeField] private float speed = 5f;

    [SerializeField] private float distanceThreshold = 0.1f;

    // current target the object is moving towards
    private Transform currentWaypoint;

    void Start() {
        // Set initial position
        currentWaypoint = waypoints.GetNextWaypoint(currentWaypoint);
        transform.position = currentWaypoint.position;
        
        // set the next waypoint target
        currentWaypoint = waypoints.GetNextWaypoint(currentWaypoint);
    }
    
    void Update() {
        transform.position = Vector3.MoveTowards(transform.position, currentWaypoint.position, speed * Time.deltaTime);
        if (Vector3.Distance(transform.position, currentWaypoint.position) < distanceLimit) {
            currentWaypoint = waypoints.GetNextWaypoint(currentWaypoint);
        }
    }

}