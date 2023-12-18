using System.Collections.Generic;
using UnityEngine;
using System.Collections.Generic;

public class Graph : MonoBehaviour
{
    // private waypointsize with getter
    [Range(0f, 2f)] [SerializeField] private float waypointSize = 0.5f;
    [SerializeField] public List<Edge> edges;

    public List<Waypoint> waypoints;

    void Start()
    {
        waypoints = new List<Waypoint>(FindObjectsOfType<Waypoint>());
    }

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

        float minDistance = float.MaxValue;
        Vector3 closestPoint = Vector3.zero;

        foreach (Edge edge in edges)
        {
            Vector3 point = edge.GetClosestPoint(buildingPosition);
            float distance = Vector3.Distance(point, buildingPosition);

            if (distance < minDistance)
            {
                minDistance = distance;
                closestPoint = point;
            }
        }

        return closestPoint;
    }

    private void DrawEdgeGizmos()
    {
        foreach (Edge edge in edges)
        {
            edge.DrawGizmo();
        }
    }

    public Edge getEdge(Waypoint a, Waypoint b)
    {
        foreach (Edge edge in edges)
        {
            if (edge.StartWaypoint == a && edge.EndWaypoint == b)
            {
                return edge;
            }
        }

        return null;
    }

    public Edge getClosetEdge(Vector3 position)
    {
        Edge closestEdge = null;
        float minDistance = float.MaxValue;

        foreach (Edge edge in edges)
        {
            float distance = edge.DistanceToEdge(position);

            if (distance < minDistance)
            {
                minDistance = distance;
                closestEdge = edge;
            }
        }

        return closestEdge;
    }
}
