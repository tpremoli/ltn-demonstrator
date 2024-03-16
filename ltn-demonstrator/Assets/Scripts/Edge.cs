using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class ReducedEdge
{
    public Waypoint startWaypoint;
    public Waypoint endWaypoint;

    public ReducedEdge(Edge edge)
    {
        this.startWaypoint = edge.startWaypoint;
        this.endWaypoint = edge.endWaypoint;
    }
    public ReducedEdge(Waypoint startWaypoint, Waypoint endWaypoint)
    {
        this.startWaypoint = startWaypoint;
        this.endWaypoint = endWaypoint;
    }

    public override bool Equals(object obj)
    {
        if (obj == null || this.GetType() != obj.GetType())
            return false;

        ReducedEdge other = (ReducedEdge)obj;
        return (this.startWaypoint == other.startWaypoint && this.endWaypoint == other.endWaypoint);
    }

    public override int GetHashCode()
    {
        unchecked // Overflow is fine, just wrap
        {
            int hash = 17;

            // Order the waypoints to ensure that the hash code is the same for 
            // ReducedEdge(start, end) and ReducedEdge(end, start) if that symmetry is desired.
            // If the direction matters, remove the ordering (Min, Max).
            int startHash = startWaypoint.GetHashCode();
            int endHash = endWaypoint.GetHashCode();

            hash = hash * 31 + System.Math.Min(startHash, endHash);
            hash = hash * 31 + System.Math.Max(startHash, endHash);
            return hash;
        }
    }
}


[System.Serializable]
public class Edge : ISerializationCallbackReceiver
{
    [SerializeField] private static int IDcounter = 0;
    [SerializeField] private static Dictionary<int, Edge> edgesByID = new Dictionary<int, Edge>();
    [SerializeField] private int EdgeID;
    public Vector3 position;
    public Vector3 direction;
    public Waypoint startWaypoint;

    private Waypoint startWaypointLane;

    public Waypoint endWaypoint;
    private Waypoint endWaypointLane;

    public float length;

    public Waypoint StartWaypoint { get { return startWaypoint; } }
    public Waypoint EndWaypoint { get { return endWaypoint; } }
    public List<WaypointMover> TravellersOnEdge;

    [SerializeField] public List<ReducedEdge> IntersectingEdgesByReducedEdge;
    public float Distance { get { return length; } }

    /// <summary>
    /// Barrier stores if there's a barrier in the path of the edge
    /// isBarricated is true if there is a barrier in the path of the edge
    /// barricadeLocation is the z position of the barrier in the path of the edge. -1 if there is no barrier
    /// </summary>
    public bool isBarricated;
    public float barrierLocation;
    public Barrier barrier;
    public bool isPedestrianOnly;

    [System.NonSerialized]
    public List<Edge> IntersectingEdges;

    public Edge(Waypoint startWaypoint, Waypoint endWaypoint)
    {
        this.EdgeID = Edge.IDcounter;
        Edge.IDcounter++;

        this.startWaypoint = startWaypoint;
        this.endWaypoint = endWaypoint;
        this.length = Vector3.Distance(startWaypoint.transform.position, endWaypoint.transform.position);

        this.TravellersOnEdge = new List<WaypointMover>();
        this.IntersectingEdges = new List<Edge>();
        this.IntersectingEdgesByReducedEdge = new List<ReducedEdge>();

        this.barrier = getBarrierInPath();
        this.isBarricated = barrier != null;
        this.barrierLocation = barrier != null ? convertToPositionAlongEdge(barrier.transform.position) : -1f;

        this.isPedestrianOnly = startWaypoint.isPedestrianOnly || endWaypoint.isPedestrianOnly;

        if (this.isBarricated)
        {
            Debug.Log("Edge between " + startWaypoint.name + " and " + endWaypoint.name + " is barricaded at " + this.barrierLocation);
        }

        Edge.edgesByID.Add(this.EdgeID, this);
    }

    // Implement this method, but you can leave it empty if you don't need it
    public void OnBeforeSerialize() { }
    public void OnAfterDeserialize()
    {
        this.IntersectingEdges = new List<Edge>();

        foreach (ReducedEdge r in IntersectingEdgesByReducedEdge)
        {
            this.IntersectingEdges.Add(Graph.Instance.GetEdge(r));
        }
    }

    public void RecheckBarriers()
    {
        this.barrier = getBarrierInPath();
        this.isBarricated = barrier != null;
        this.barrierLocation = barrier != null ? convertToPositionAlongEdge(barrier.transform.position) : -1f;
        if (this.isBarricated)
        {
            Debug.Log("Edge between " + startWaypoint.name + " and " + endWaypoint.name + " is barricaded at " + this.barrierLocation);
        }
    }

    // converting between distance along edge and distance in world space
    public float RealDistanceToDeltaD(float len)
    {
        return len / this.length;
    }
    public float DeltaDToRealDistance(float len)
    {
        return len * this.length;
    }

    // adds and removes travellers from the edge
    public void Subscribe(WaypointMover trav)
    {
        this.TravellersOnEdge.Add(trav);
    }
    public void Unsubscribe(WaypointMover trav)
    {
        this.TravellersOnEdge.Remove(trav);
    }
    public void RegisterIntersectingEdge(Edge e)
    {
        this.IntersectingEdges.Add(e);
        this.IntersectingEdgesByReducedEdge.Add(e.Reduce());
    }
    public bool IntersectingEdgesBusy()
    {
        foreach (Edge e in this.IntersectingEdges)
        {
            if (e.TravellersOnEdge.Count > 0) return true;
        }
        return false;
    }

    public void DrawGizmo()
    {
        Color edgeColor;
        if (IntersectingEdges.Count > 0)
        {
            edgeColor = Color.yellow;
            if (isPedestrianOnly)
            {
                Gizmos.color = edgeColor;
                Gizmos.DrawLine(startWaypoint.transform.position, endWaypoint.transform.position);
                return;
            }
        }
        else
        {
            edgeColor = Color.green;
            if (isPedestrianOnly)
            {
                return;
            }

        }


        // Draw arrow pointing in the edge's direction
        Vector3 startpoint = startWaypoint.transform.position;
        Vector3 endpoint = endWaypoint.transform.position;
        Vector3 direction = endpoint - startpoint;
        // Debug.Log("Direction of the Road Edge: " + direction);

        // Make the arrows shorter by 20%
        float shortenedMagnitude = direction.magnitude * 0.7f;
        Vector3 shortenedDirection = direction.normalized * shortenedMagnitude;

        // Calculate the middle position
        Vector3 middlePosition = startpoint + direction * 0.5f - shortenedDirection * 0.5f;

        // Shift the middle position slightly to the left
        Vector3 shiftedMiddlePosition = middlePosition + Vector3.Cross(direction, Vector3.up).normalized * 0.6f;

        // Draw the arrow with the shifted middle position and shortened direction
        DrawArrow.ForGizmo(shiftedMiddlePosition, shortenedDirection, edgeColor, 1f, 30f);
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

    // returns the distance from the edge to a point
    public float DistanceToEdge(Vector3 position)
    {
        Vector3 closestPoint = this.GetClosestPoint(position);
        return Vector3.Distance(position, closestPoint);
    }

    // returns the direction of the edge
    public Vector3 GetDirection()
    {
        Vector3 start = startWaypoint.transform.position;
        Vector3 end = endWaypoint.transform.position;

        Vector3 edgeDirection = end - start;
        edgeDirection.Normalize();
        return edgeDirection;
    }

    // returns the closest point on the edge to a point
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

    // returns the closest point on the edge to a point, as a fraction of the edge
    public float GetClosestPointAsFractionOfEdge(Vector3 point)
    {
        Vector3 start = startWaypoint.transform.position;
        Vector3 end = endWaypoint.transform.position;

        Vector3 edgeDirection = end - start;
        Vector3 pointDirection = point - start;

        float edgeLength = edgeDirection.magnitude;
        edgeDirection.Normalize();

        return Vector3.Dot(pointDirection, edgeDirection) / edgeLength;
    }

    // returns a random point on the edge
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

    // checks if two edges are the same (i.e. have the same waypoints)
    public bool isSameEdge(Edge otherEdge)
    {
        return (this.startWaypoint == otherEdge.startWaypoint && this.endWaypoint == otherEdge.endWaypoint) ||
               (this.startWaypoint == otherEdge.endWaypoint && this.endWaypoint == otherEdge.startWaypoint);
    }

    // returns the distance along the edge of a point
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
        Barrier[] allBarriers;

        if (BarrierManager.Instance == null)
        {
            allBarriers = GameObject.FindObjectsOfType<Barrier>();
        }
        else
        {
            List<GameObject> allBarrierGameObjects;
            allBarrierGameObjects = BarrierManager.Instance.allBarriers;
            // get all the Barrier objects from the list of GameObjects
            allBarriers = new Barrier[allBarrierGameObjects.Count];
            for (int i = 0; i < allBarrierGameObjects.Count; i++)
            {
                allBarriers[i] = allBarrierGameObjects[i].GetComponent<Barrier>();
            }
        }

        // we go through the Barrier and check if the edge intersects with any of them
        foreach (Barrier barrier in allBarriers)
        {
            if (barrier.isPointInBarrier(this.GetClosestPoint(barrier.transform.position)))
            {
                Debug.Log("Barrier found at " + barrier.transform.position);
                // Calculate the angle between the barrier's forward direction and the edge direction
                Vector3 edgeDirection = this.GetDirection();
                float angle = Vector3.Angle(barrier.transform.forward, edgeDirection);

                // Apply the rotation to the barrier
                barrier.transform.rotation = Quaternion.Euler(0, angle, 0);

                return barrier;
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

    public ReducedEdge Reduce()
    {
        return new ReducedEdge(this.startWaypoint, this.endWaypoint);
    }
}