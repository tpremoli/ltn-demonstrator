using UnityEngine;
using System.Collections.Generic;

public class Waypoint : MonoBehaviour
{
    public List<Waypoint> adjacentWaypoints;
    public float adjacencyThreshold = 5f; // Adjust this value as needed

    void Start()
    {
    }
}
