using UnityEngine;

[System.Serializable]
public class Edge : MonoBehaviour
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

    public void Initialize(Waypoint startWaypoint, Waypoint endWaypoint)
    {
        this.startWaypoint = startWaypoint;
        this.endWaypoint = endWaypoint;
        this.distance = Vector3.Distance(startWaypoint.transform.position, endWaypoint.transform.position);
    }

    private void OnDrawGizmos()
    {
        // Draw arrow pointing in the edge's direction
        Vector3 startpoint = startWaypoint.transform.position;
        Vector3 endpoint = endWaypoint.transform.position;
        Vector3 direction = endpoint - startpoint;
        
        // Reverse the direction vector
        Vector3 reversedDirection = -direction;
        
        // Make the arrows shorter by 20%
        float shortenedMagnitude = reversedDirection.magnitude * 0.8f;
        Vector3 shortenedDirection = reversedDirection.normalized * shortenedMagnitude;
        
        // Calculate the middle position
        Vector3 middlePosition = startpoint + direction * 0.5f - shortenedDirection * 0.5f;
        
        // Shift the middle position slightly to the left
        Vector3 shiftedMiddlePosition = middlePosition - Vector3.Cross(direction, Vector3.up).normalized * 0.5f;
        
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

        return distanceFromStart + distanceFromEnd <= edgeLength + 0.1f;
    }
}