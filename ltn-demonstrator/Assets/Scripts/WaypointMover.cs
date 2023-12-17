using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointMover : MonoBehaviour 
{
    // attributes from the Traveller class
    private float totalDistanceMoved;
    private float positionOnEdge;
    private float currentVelocity; 
    private float maxVelocity; // will be assigned according to agent category enum (EdgeFunctionality)
    private int noOfPassengers; 
    private float rateOfEmission;

    // attributes from the WaypointMover class
    [SerializeField] private Waypoint startingWaypoint;
    [SerializeField] private float speed = 5f;
    [SerializeField] private float distanceThreshold = 0.1f;

    private WaypointPath path;
    private Waypoint currentWaypoint;

    void Start() {
        path = new WaypointPath(startingWaypoint);
        currentWaypoint = path.GetNextWaypoint();
        transform.position = currentWaypoint.transform.position;
        Debug.Log("Traveller Instantiated");
    }
    
    void Update() {
        if (currentWaypoint == null) return;

        transform.position = Vector3.MoveTowards(transform.position, currentWaypoint.transform.position, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, currentWaypoint.transform.position) < distanceThreshold) {
            currentWaypoint = path.GetNextWaypoint();
        }
    }

    public float calculateEmissions()
    {
        // Calculate emissions using the rateOfEmission attribute and totalDistanceMoved
        return rateOfEmission * totalDistanceMoved;
    }

}
