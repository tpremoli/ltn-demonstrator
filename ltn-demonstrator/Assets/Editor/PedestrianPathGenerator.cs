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
                // For a single adjacent, place two pedestrian waypoints at 120-degree intervals
                CreatePedestrianWaypointsForCulDeSac(waypoint);
            }
            else
            {
                // For multiple adjacents, calculate the bisecting direction for each pair
                for (int i = 0; i < numAdjacent; i++)
                {
                    Waypoint adjacent1 = waypoint.adjacentWaypoints[i];
                    Vector3 dir1 = (adjacent1.transform.position - waypoint.transform.position).normalized;

                    int nextIndex = (i + 1) % numAdjacent;
                    Waypoint adjacent2 = waypoint.adjacentWaypoints[nextIndex];
                    Vector3 dir2 = (adjacent2.transform.position - waypoint.transform.position).normalized;

                    // Calculate the bisecting direction
                    Vector3 bisectingDir = AverageDirection(dir1, dir2);
                    Vector3 offset = bisectingDir * laneWidth;

                    CreatePedestrianWaypointAt(waypoint.transform.position + offset);
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