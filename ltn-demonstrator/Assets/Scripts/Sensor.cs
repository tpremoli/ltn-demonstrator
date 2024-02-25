using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sensor : MonoBehaviour
{
    public bool isPointInSensor(Vector3 point)
    {
        Collider sensorCollider = GetComponent<Collider>();
        if (sensorCollider == null)
        {
            return false;
        }

        // Check if the point is within the sensor's collider
        return sensorCollider.bounds.Contains(point);
    }

    public Waypoint FindNearestWaypoint()
    {
        Waypoint[] allWaypoints = GameObject.FindObjectsOfType<Waypoint>();
        Waypoint nearestWaypoint = null;
        float minDistance = float.MaxValue;

        foreach (Waypoint waypoint in allWaypoints)
        {
            float distance = Vector3.Distance(this.transform.position, waypoint.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestWaypoint = waypoint;
            }
        }

        return nearestWaypoint;
    }

    // Use this for initialization
    void Start()
    {
        Waypoint nearestWaypoint = FindNearestWaypoint();
        if (nearestWaypoint != null)
        {
            Debug.Log("Nearest waypoint to sensor is at " + nearestWaypoint.transform.position);
        }
        else
        {
            Debug.Log("No waypoints found.");
        }
    }
}