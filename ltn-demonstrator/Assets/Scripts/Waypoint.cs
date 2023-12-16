using UnityEngine;
using System.Collections.Generic;

public class Waypoint : MonoBehaviour
{
    public List<Waypoint> adjacentWaypoints; // Manually set in the editor

    public void AddAdjacentWaypoint(Waypoint newAdjacent)
    {
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
