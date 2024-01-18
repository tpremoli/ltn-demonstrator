using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class PedestrianPathGenerator
{
    public static float laneWidth = 3f; // Width of a lane, adjust as needed

    [MenuItem("Tools/Generate Pedestrian Paths")]
    [RuntimeInitializeOnLoadMethod]
    public static void GenerateParallelWaypoints()
    {
        // Iterate through all waypoints in the scene
        Waypoint[] allWaypoints = Object.FindObjectsOfType<Waypoint>();
        foreach (var waypoint in allWaypoints)
        {
            int numAdjacent = waypoint.adjacentWaypoints.Count;

            if (numAdjacent < 1) continue; // Skip waypoints with no adjacents

            if (numAdjacent == 1)
            {
                // Handle cul-de-sacs
                CreatePedestrianWaypointsForCulDeSac(waypoint);
            }
            else if (numAdjacent == 2)
            {
                // Handle waypoints with exactly two adjacents
                CreatePedestrianWaypointsForTwoAdjacents(waypoint);
            }
            else
            {
                // Sort the adjacent waypoints based on their angle
                List<Waypoint> sortedAdjacentWaypoints = waypoint.adjacentWaypoints
                    .OrderBy(adj => AngleFromReference(waypoint.transform.position, adj.transform.position))
                    .ToList();

                // Create pedestrian waypoints for each pair of sorted adjacents
                for (int i = 0; i < sortedAdjacentWaypoints.Count; i++)
                {
                    int nextIndex = (i + 1) % sortedAdjacentWaypoints.Count;
                    CreatePedestrianWaypointForAdjacentPair(waypoint, sortedAdjacentWaypoints[i], sortedAdjacentWaypoints[nextIndex]);
                }
            }
        }
    }

    static void CreatePedestrianWaypointsForCulDeSac(Waypoint waypoint)
    {
        // Direction from the waypoint to its single adjacent waypoint
        Vector3 directionToAdjacent = (waypoint.adjacentWaypoints[0].transform.position - waypoint.transform.position).normalized;

        // Create two pedestrian waypoints, each at 120 degrees from the direction to the adjacent waypoint
        for (int i = -1; i <= 1; i += 2) // i will be -1 and 1, representing 120 and -120 degrees
        {
            Vector3 rotatedDir = Quaternion.Euler(0, 120 * i, 0) * directionToAdjacent;
            Vector3 offset = rotatedDir * laneWidth;
            CreatePedestrianWaypointAt(waypoint.transform.position + offset);
        }
    }

    static void CreatePedestrianWaypointsForTwoAdjacents(Waypoint waypoint)
    {
        Vector3 dir1 = (waypoint.adjacentWaypoints[0].transform.position - waypoint.transform.position).normalized;
        Vector3 dir2 = (waypoint.adjacentWaypoints[1].transform.position - waypoint.transform.position).normalized;

        // Calculate the bisecting direction
        Vector3 bisectingDir = AverageDirection(dir1, dir2);

        // Create two waypoints, one on each side of the bisecting line
        CreatePedestrianWaypointAt(waypoint.transform.position + bisectingDir * laneWidth);
        CreatePedestrianWaypointAt(waypoint.transform.position - bisectingDir * laneWidth);
    }

    static void CreatePedestrianWaypointForAdjacentPair(Waypoint waypoint, Waypoint adjacent1, Waypoint adjacent2)
    {
        Vector3 dir1 = (adjacent1.transform.position - waypoint.transform.position).normalized;
        Vector3 dir2 = (adjacent2.transform.position - waypoint.transform.position).normalized;

        // Calculate the average direction
        Vector3 avgDir = AverageDirection(dir1, dir2);
        Vector3 crossProduct = Vector3.Cross(dir1, dir2);

        // Determine if the average direction needs to be flipped
        if (Vector3.Dot(crossProduct, Vector3.up) > 0)
        {
            avgDir = -avgDir;
        }

        CreatePedestrianWaypointAt(waypoint.transform.position + avgDir * laneWidth);
    }

    static float AngleFromReference(Vector3 referencePoint, Vector3 targetPoint)
    {
        Vector3 direction = (targetPoint - referencePoint).normalized;
        return Mathf.Atan2(direction.z, direction.x) * Mathf.Rad2Deg;
    }
    static void CreatePedestrianWaypointAt(Vector3 position)
    {
        GameObject newWaypointObj = new GameObject("Pedestrian Waypoint");
        newWaypointObj.transform.position = position;
        newWaypointObj.AddComponent<Waypoint>();
        newWaypointObj.GetComponent<Waypoint>().isPedestrianOnly = true;
        newWaypointObj.transform.parent = Object.FindObjectOfType<Graph>().transform;
    }

    static Vector3 AverageDirection(Vector3 dir1, Vector3 dir2)
    {
        Vector3 sum = dir1.normalized + dir2.normalized;
        return sum.normalized;
    }

}