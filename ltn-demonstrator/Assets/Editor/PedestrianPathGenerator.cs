using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class PedestrianPathGenerator
{
    public static float laneWidth = 3f; // Width of a lane, adjust as needed

    private static Dictionary<Waypoint, List<Waypoint>> pedestrianWaypointsMap = new Dictionary<Waypoint, List<Waypoint>>();

    [MenuItem("Tools/Generate Pedestrian Paths")]
    [RuntimeInitializeOnLoadMethod]
    public static void GenerateParallelWaypoints()
    {
        pedestrianWaypointsMap = new Dictionary<Waypoint, List<Waypoint>>();

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

    [MenuItem("Tools/Connect Pedestrian Paths")]
    [RuntimeInitializeOnLoadMethod]
    public static void ConnectPedestrianPaths()
    {
        // Iterate through all mappings
        foreach (var pair in pedestrianWaypointsMap)
        {
            Waypoint originalWaypoint = pair.Key;
            List<Waypoint> pedestrianWaypoints = pair.Value;

            // Connect each pedestrian waypoint with appropriate waypoints of adjacent waypoints
            foreach (var pedestrianWaypoint in pedestrianWaypoints)
            {
                foreach (var adjacent in originalWaypoint.adjacentWaypoints)
                {
                    if (pedestrianWaypointsMap.TryGetValue(adjacent, out List<Waypoint> adjacentPedestrianWaypoints))
                    {
                        foreach (var adjacentPedestrianWaypoint in adjacentPedestrianWaypoints)
                        {
                            // Check if connection intersects with original graph
                            if (!DoesIntersectWithGraph(originalWaypoint, adjacent, pedestrianWaypoint, adjacentPedestrianWaypoint))
                            {
                                pedestrianWaypoint.AddAdjacentWaypoint(adjacentPedestrianWaypoint);
                            }
                        }
                    }
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
            CreatePedestrianWaypointAt(waypoint.transform.position + offset, waypoint);
        }
    }

    static void CreatePedestrianWaypointsForTwoAdjacents(Waypoint waypoint)
    {
        Vector3 dir1 = (waypoint.adjacentWaypoints[0].transform.position - waypoint.transform.position).normalized;
        Vector3 dir2 = (waypoint.adjacentWaypoints[1].transform.position - waypoint.transform.position).normalized;

        // Calculate the bisecting direction
        Vector3 bisectingDir = AverageDirection(dir1, dir2);

        // Create two waypoints, one on each side of the bisecting line
        CreatePedestrianWaypointAt(waypoint.transform.position + bisectingDir * laneWidth, waypoint);
        CreatePedestrianWaypointAt(waypoint.transform.position - bisectingDir * laneWidth, waypoint);
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

        CreatePedestrianWaypointAt(waypoint.transform.position + avgDir * laneWidth, waypoint);
    }

    static float AngleFromReference(Vector3 referencePoint, Vector3 targetPoint)
    {
        Vector3 direction = (targetPoint - referencePoint).normalized;
        return Mathf.Atan2(direction.z, direction.x) * Mathf.Rad2Deg;
    }
    static void CreatePedestrianWaypointAt(Vector3 position, Waypoint currentWaypoint)
    {
        GameObject newWaypointObj = new GameObject("Pedestrian Waypoint");
        newWaypointObj.transform.position = position;
        Waypoint newWaypoint = newWaypointObj.AddComponent<Waypoint>();
        newWaypoint.adjacentWaypoints = new List<Waypoint>();  // Initialize the list here
        newWaypoint.isPedestrianOnly = true;
        newWaypointObj.transform.parent = Object.FindObjectOfType<Graph>().transform;

        // Store the new waypoint in the map
        if (pedestrianWaypointsMap.ContainsKey(currentWaypoint))
        {
            pedestrianWaypointsMap[currentWaypoint].Add(newWaypoint);
        }
        else
        {
            pedestrianWaypointsMap[currentWaypoint] = new List<Waypoint> { newWaypoint };
        }

    }


    static bool DoesIntersectWithGraph(Waypoint original1, Waypoint original2, Waypoint pedestrian1, Waypoint pedestrian2)
    {
        // Line segment from original1 to original2
        Vector3 a = original1.transform.position;
        Vector3 b = original2.transform.position;

        // Line segment from pedestrian1 to pedestrian2
        Vector3 c = pedestrian1.transform.position;
        Vector3 d = pedestrian2.transform.position;

        // Check for intersection
        return LineSegmentsIntersect(a, b, c, d);
    }

    static bool LineSegmentsIntersect(Vector3 a, Vector3 b, Vector3 c, Vector3 d)
    {
        // Check if line segments ab and cd intersect
        float denominator = (b.x - a.x) * (d.z - c.z) - (b.z - a.z) * (d.x - c.x);
        if (denominator == 0) return false; // Lines are parallel

        float numerator1 = (a.z - c.z) * (d.x - c.x) - (a.x - c.x) * (d.z - c.z);
        float numerator2 = (a.z - c.z) * (b.x - a.x) - (a.x - c.x) * (b.z - a.z);

        float r = numerator1 / denominator;
        float s = numerator2 / denominator;

        // Intersection occurs if 0 <= r <= 1 and 0 <= s <= 1
        return (r >= 0 && r <= 1) && (s >= 0 && s <= 1);
    }
    static Vector3 AverageDirection(Vector3 dir1, Vector3 dir2)
    {
        Vector3 sum = dir1.normalized + dir2.normalized;
        return sum.normalized;
    }

}