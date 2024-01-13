using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Edge
{
    public static List<Edge> allEdges = new List<Edge>();

    public Edge() {
        allEdges.Add(this);
    }

    [SerializeField]
    private Waypoint startWaypoint;

    [SerializeField]
    private Waypoint endWaypoint;

    [SerializeField]
    private float distance;

    public Waypoint StartWaypoint { get { return startWaypoint; } }
    public Waypoint EndWaypoint { get { return endWaypoint; } }
    public float Distance { get { return distance; } }

    /// <summary>
    /// Barrier stores if there's a barrier in the path of the edge
    /// isBarricated is true if there is a barrier in the path of the edge
    /// barricadeLocation is the z position of the barrier in the path of the edge. -1 if there is no barrier
    /// </summary>
    public bool isBarricated;
    public float barrierLocation;
    public Barrier barrier;
    public Edge(Waypoint startWaypoint, Waypoint endWaypoint)
    {
        this.startWaypoint = startWaypoint;
        this.endWaypoint = endWaypoint;
        this.distance = Vector3.Distance(startWaypoint.transform.position, endWaypoint.transform.position);

        // finds barrier in path of edge 
        this.barrier = getBarrierInPath();
        this.isBarricated = barrier != null;
        this.barrierLocation = barrier != null ? convertToPositionAlongEdge(barrier.transform.position) : -1f;

        if (this.isBarricated)
        {
            Debug.Log("Edge between " + startWaypoint.name + " and " + endWaypoint.name + " is barricaded at " + this.barrierLocation);
        }

        allEdges.Add(this);
    }

    public void DrawGizmo()
    {
        // Draw arrow pointing in the edge's direction
        Vector3 startpoint = startWaypoint.transform.position;
        Vector3 endpoint = endWaypoint.transform.position;
        Vector3 direction = endpoint - startpoint;
        
        // Make the arrows shorter by 20%
        float shortenedMagnitude = direction.magnitude * 0.7f;
        Vector3 shortenedDirection = direction.normalized * shortenedMagnitude;
        
        // Calculate the middle position
        Vector3 middlePosition = startpoint + direction * 0.5f - shortenedDirection * 0.5f;
        
        // Shift the middle position slightly to the left
        Vector3 shiftedMiddlePosition = middlePosition + Vector3.Cross(direction, Vector3.up).normalized * 0.6f;
        
        // Draw the arrow with the shifted middle position and shortened direction
        DrawArrow.ForGizmo(shiftedMiddlePosition, shortenedDirection, Color.green, 1f, 30f);
    }

    public bool isPointOnEdge(Vector3 point)
    {
        Vector3 start = startWaypoint.transform.position;
        Vector3 end = endWaypoint.transform.position;

        float distanceFromStart = Vector3.Distance(point, start);
        float distanceFromEnd = Vector3.Distance(point, end);
        float edgeLength = Vector3.Distance(start, end);

        return distanceFromStart + distanceFromEnd <= edgeLength + 0.2f;
    }

    public float DistanceToEdge(Vector3 position){
        Vector3 closestPoint = this.GetClosestPoint(position);
        return Vector3.Distance(position, closestPoint);
    }

    public Vector3 GetClosestPoint(Vector3 point)
    {
        Vector3 start = startWaypoint.transform.position;
        Vector3 end = endWaypoint.transform.position;

        Vector3 edgeDirection = end - start;
        Vector3 pointDirection = point - start;

        float edgeLength = edgeDirection.magnitude;
        edgeDirection.Normalize();

        float dotProduct = Vector3.Dot(pointDirection, edgeDirection);

        if (dotProduct <= 0)
        {
            return start;
        }
        else if (dotProduct >= edgeLength)
        {
            return end;
        }
        else
        {
            Vector3 closestPointOnEdge = start + edgeDirection * dotProduct;
            return closestPointOnEdge;
        }
    }

    public Vector3 GetRandomPointOnEdge()
    {
        Vector3 start = startWaypoint.transform.position;
        Vector3 end = endWaypoint.transform.position;

        Vector3 edgeDirection = end - start;
        float edgeLength = edgeDirection.magnitude;
        edgeDirection.Normalize();

        float randomDistance = Random.Range(0f, edgeLength);
        Vector3 randomPointOnEdge = start + edgeDirection * randomDistance;

        return randomPointOnEdge;
    }

    public bool isSameEdge(Edge otherEdge)
    {
        return (this.startWaypoint == otherEdge.startWaypoint && this.endWaypoint == otherEdge.endWaypoint) ||
               (this.startWaypoint == otherEdge.endWaypoint && this.endWaypoint == otherEdge.startWaypoint);
    }

    public float convertToPositionAlongEdge(Vector3 point)
    {
        Vector3 start = startWaypoint.transform.position;
        Vector3 end = endWaypoint.transform.position;

        Vector3 edgeDirection = end - start;
        Vector3 pointDirection = point - start;

        float edgeLength = edgeDirection.magnitude;
        edgeDirection.Normalize();

        float dotProduct = Vector3.Dot(pointDirection, edgeDirection);

        if (dotProduct <= 0)
        {
            return 0f;
        }
        else if (dotProduct >= edgeLength)
        {
            return 1f;
        }
        else
        {
            return dotProduct / edgeLength;
        }
    }

    public Barrier getBarrierInPath()
    {
        // we go through the Barrier and check if the edge intersects with any of them
        Barrier[] allBarriers = GameObject.FindObjectsOfType<Barrier>();
        for (int i = 0; i < allBarriers.Length; i++)
        {
            if (allBarriers[i].isPointInBarrier(this.GetClosestPoint(allBarriers[i].transform.position)))
            {
                return allBarriers[i];
            }
        }
        return null;
    }

    public Waypoint getClosestWaypoint(Vector3 point)
    {
        if (!isPointOnEdge(point))
        {
            Debug.Log("Position is not on edge");
            return null;
        }

        float distanceToStart = Vector3.Distance(point, startWaypoint.transform.position);
        float distanceToEnd = Vector3.Distance(point, endWaypoint.transform.position);

        if (distanceToStart < distanceToEnd)
        {
            return startWaypoint;
        }
        else
        {
            return endWaypoint;
        }
    }

    /// <summary>
    /// This method returns the closest waypoint that is not blocked by a barrier, given a point.
    /// If both waypoints are blocked, it returns null.
    /// </summary>
    /// <param name="point">the point we want to check</param>
    /// <returns></returns>
    public Waypoint getClosestAccesibleWaypoint(Vector3 point)
    {
        float distanceToStart = Vector3.Distance(point, startWaypoint.transform.position);
        float distanceToEnd = Vector3.Distance(point, endWaypoint.transform.position);

        // Debug.LogWarning($"Distance to Start Waypoint: {distanceToStart}");
        // Debug.LogWarning($"Distance to End Waypoint: {distanceToEnd}");

        if (distanceToStart < distanceToEnd)
        {
            // If there is no barrier between the point and the start waypoint, return start waypoint
            if (!isBarrierBetween(point, startWaypoint.transform.position))
            {
                return startWaypoint;
            }
            // Otherwise, check if the end waypoint is accessible
            else if (!isBarrierBetween(point, endWaypoint.transform.position))
            {
                return endWaypoint;
            }
        }
        else
        {
            // If there is no barrier between the point and the end waypoint, return end waypoint
            if (!isBarrierBetween(point, endWaypoint.transform.position))
            {
                return endWaypoint;
            }
            // Otherwise, check if the start waypoint is accessible
            else if (!isBarrierBetween(point, startWaypoint.transform.position))
            {
                return startWaypoint;
            }
        }

        // If both waypoints are blocked by a barrier, return null
        // Debug.LogWarning("No accessible waypoint found, returning null");
        return null;
    }
    
    /// <summary>
    /// This checks if there is a barrier between a start point and an end point on the edge
    /// </summary>
    /// <param name="start"></param>
    /// <param name="destination"></param>
    /// <returns></returns>
    public bool isBarrierBetween(Vector3 start, Vector3 destination)
    {
        // Check if barricade is active
        if (!isBarricated)
        {
            return false;
        }

        // Convert positions to distances along the edge
        float startDistance = convertToPositionAlongEdge(start);
        float destinationDistance = convertToPositionAlongEdge(destination);
        float barrierDistance = convertToPositionAlongEdge(barrier.transform.position);

        // Check if the barrier is between start and destination
        // Assuming lower distance value is closer to the starting point of the edge
        return barrierDistance > System.Math.Min(startDistance, destinationDistance) && 
            barrierDistance < System.Math.Max(startDistance, destinationDistance);
    }
}