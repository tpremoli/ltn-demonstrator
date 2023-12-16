using UnityEngine;
using System.Collections.Generic;

public class Waypoint : MonoBehaviour
{
    public List<Waypoint> adjacentWaypoints;
    public float adjacencyThreshold = 5f; // Adjust this value as needed

    void Start()
    {
        FindAdjacentWaypoints();
    }

    void FindAdjacentWaypoints()
    {
        adjacentWaypoints = new List<Waypoint>();
        foreach (Waypoint other in FindObjectsOfType<Waypoint>())
        {
            if (other != this && Vector3.Distance(transform.position, other.transform.position) <= adjacencyThreshold)
            {
                adjacentWaypoints.Add(other);
            }
        }
    }
}
