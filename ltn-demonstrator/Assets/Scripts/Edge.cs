using UnityEngine;

[System.Serializable]
public class Edge
{
    [SerializeField]
    private Waypoint startWaypoint;

    [SerializeField]
    private Waypoint endWaypoint;

    [SerializeField]
    private float distance;

    public Waypoint StartWaypoint { get { return startWaypoint; } }
    public Waypoint EndWaypoint { get { return endWaypoint; } }
    public float Distance { get { return distance; } }

    public Edge(Waypoint startWaypoint, Waypoint endWaypoint)
    {
        this.startWaypoint = startWaypoint;
        this.endWaypoint = endWaypoint;
        this.distance = Vector3.Distance(startWaypoint.transform.position, endWaypoint.transform.position);
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

}