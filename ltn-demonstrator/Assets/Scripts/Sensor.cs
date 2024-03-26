using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;

public class Sensor : MonoBehaviour
{
    public int sensor_trav_count = 0;
    public Dictionary<WaypointMover, int> sensor_trav_stats = new Dictionary<WaypointMover, int>();
    private Edge edgeAssigned;


    public void OnSensorClicked()
    {
        Debug.Log("Sensor clicked.");
    }


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
        Graph graph = Graph.Instance;

        Waypoint nearestWaypoint = FindNearestWaypoint();
        if (nearestWaypoint != null)
        {
            Debug.Log("Nearest waypoint to sensor is at " + nearestWaypoint.transform.position);
        }
        else
        {
            Debug.Log("No waypoints found.");
        }

        Vector3 position = this.transform.position;
        Edge nearestEdge = graph.getClosetRoadEdge(position);
        edgeAssigned = nearestEdge;
        if (nearestEdge != null && nearestEdge.StartWaypoint != null && nearestEdge.EndWaypoint != null)
        {
            Debug.Log("Nearest edge to sensor is " + nearestEdge.StartWaypoint.transform.position + " to " + nearestEdge.EndWaypoint.transform.position);
        }
        else
        {
            Debug.Log("No edges found or edge waypoints are null.");
        }

    }

    public Edge FindNearestEdge()
    {
        Graph graph = Graph.Instance;
        if (graph == null || graph.edges == null)
        {
            Debug.Log("Graph or edges list is null.");
            return null;
        }

        Edge nearestEdge = null;
        float minDistance = float.MaxValue;

        foreach (Edge edge in graph.edges)
        {
            if (edge != null)
            {
                Vector3 closestPoint = edge.GetClosestPoint(this.transform.position);
                if (closestPoint != null)
                {
                    float distance = Vector3.Distance(this.transform.position, closestPoint);
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        nearestEdge = edge;
                    }
                }
            }
        }

        if (nearestEdge != null)
        {
            Debug.Log("Nearest edge to sensor is " + nearestEdge.StartWaypoint.transform.position + " to " + nearestEdge.EndWaypoint.transform.position);
        }
        else
        {
            Debug.Log("No edges found.");
        }

        return nearestEdge;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector3 startPos = this.transform.position + Vector3.up * 2;
        Vector3 startWaypointRef = edgeAssigned.StartWaypoint.transform.position + Vector3.up * 2;
        Vector3 endWaypointRef = edgeAssigned.EndWaypoint.transform.position + Vector3.up * 2;
        Gizmos.DrawLine(startPos, startWaypointRef);
        Gizmos.DrawLine(startPos, endWaypointRef);
    }
    public int CollectDataOnLeave(WaypointMover trav)
    {
        // This method is a stub TO BE EXTENDED for collecting data from trav
        // TED ;)

        // increment traveller count for this sensor
        sensor_trav_count += 1;
        sensor_trav_stats[trav] += 1;
        //sensor_trav_stats[trav] += trav.getSpeed();
        // Ted maybe adding other traveller stats here
        //sensor_trav_stats[trav] += trav.
        return sensor_trav_count;


    }
    public int CollectDataOnEnter(WaypointMover trav)
    {
        // This method is a stub TO BE EXTENDED for collecting data from trav
        // TED ;)
        sensor_trav_count -= 1;
        return sensor_trav_count;
    }
}