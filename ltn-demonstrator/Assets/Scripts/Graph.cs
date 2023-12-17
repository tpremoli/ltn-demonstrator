using UnityEngine;
using System.Collections.Generic;

public class Graph : MonoBehaviour
{
    // private waypointsize with getter
    [Range(0f, 2f)] [SerializeField] private float waypointSize = 0.5f;
    [SerializeField] public List<Edge> edges;

    public float WaypointSize
    {
        get { return waypointSize; }
    }

    private void OnDrawGizmos()
    {
        DrawEdgeGizmos();
    }

    public float CalculateDistance(Waypoint a, Waypoint b)
    {
        return Vector3.Distance(a.transform.position, b.transform.position);
    }

    public float CalculateDistance(GameObject a, GameObject b)
    {
        return Vector3.Distance(a.transform.position, b.transform.position);
    }

    public Vector3 GetClosestPointToBuilding(GameObject building)
    {
        Vector3 buildingPosition = building.transform.position;
        Waypoint[] waypoints = FindObjectsOfType<Waypoint>();

        float minDistance = float.MaxValue;
        Vector3 closestPoint = Vector3.zero;

        foreach (Waypoint waypoint in waypoints)
        {
            foreach (Waypoint adjacentWaypoint in waypoint.adjacentWaypoints)
            {
                Vector3 pointOnEdge = GetClosestPointOnEdge(waypoint.transform.position, adjacentWaypoint.transform.position, buildingPosition);
                float distance = Vector3.Distance(buildingPosition, pointOnEdge);

                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestPoint = pointOnEdge;
                }
            }
        }

        return closestPoint;
    }

    private Vector3 GetClosestPointOnEdge(Vector3 pointA, Vector3 pointB, Vector3 targetPoint)
    {
        Vector3 edgeDirection = pointB - pointA;
        float edgeLength = edgeDirection.magnitude;
        edgeDirection.Normalize();

        Vector3 targetDirection = targetPoint - pointA;
        float projection = Vector3.Dot(targetDirection, edgeDirection);

        if (projection <= 0f)
        {
            return pointA;
        }
        else if (projection >= edgeLength)
        {
            return pointB;
        }
        else
        {
            return pointA + edgeDirection * projection;
        }
    }

    private void Start()
    {
    }

    private void DrawEdgeGizmos()
    {
        foreach (Edge edge in edges)
        {
            edge.DrawGizmo();
        }
    }
}
