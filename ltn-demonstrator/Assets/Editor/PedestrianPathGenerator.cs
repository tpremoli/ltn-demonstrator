using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
public class PedestrianPathGenerator
{
    public static float laneWidth = 3.5f; // Width of a lane, adjust as needed

    private static Dictionary<Waypoint, List<Waypoint>> intersectionPedWaypointsMap = new Dictionary<Waypoint, List<Waypoint>>();
    private static Dictionary<Waypoint, Waypoint> pedWaypointCenters = new Dictionary<Waypoint, Waypoint>();

    // this maps an intersection to its supdivided waypoints
    private static List<Waypoint> subdividedWaypoints = new List<Waypoint>();

    // STEP 1: Generate pedestrian waypoints

    [MenuItem("Tools/Sidewalks/1. Generate Pedestrian Waypoints")]
    public static void GeneratePedestrianWaypoints()
    {
        intersectionPedWaypointsMap = new Dictionary<Waypoint, List<Waypoint>>();
        pedWaypointCenters = new Dictionary<Waypoint, Waypoint>();

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

        float adjustedLaneWidth = laneWidth * 0.85f; // Override the lane width for cul-de-sacs

        // Create two pedestrian waypoints, each at 120 degrees from the direction to the adjacent waypoint
        for (int i = -1; i <= 1; i += 2) // i will be -1 and 1, representing 120 and -120 degrees
        {
            Vector3 rotatedDir = Quaternion.Euler(0, 120 * i, 0) * directionToAdjacent;
            Vector3 offset = rotatedDir * adjustedLaneWidth;
            CreatePedestrianWaypointAt(waypoint.transform.position + offset, waypoint);
        }
    }

    static void CreatePedestrianWaypointsForTwoAdjacents(Waypoint waypoint)
    {
        Vector3 dir1 = (waypoint.adjacentWaypoints[0].transform.position - waypoint.transform.position).normalized;
        Vector3 dir2 = (waypoint.adjacentWaypoints[1].transform.position - waypoint.transform.position).normalized;

        // Adjust lane width based on the angle
        // For example, using the cosine of the angle, ensuring it's not less than a minimum threshold
        float angle = Vector3.Angle(dir1, dir2) * Mathf.Deg2Rad;
        float minScaleFactor = 0.7f; // Minimum scale factor
        float scaleFactor = Mathf.Max(Mathf.Sin(angle), minScaleFactor);
        float adjustedLaneWidth = laneWidth * scaleFactor;

        // Calculate the bisecting direction
        Vector3 bisectingDir = AverageDirection(dir1, dir2);

        // Create two waypoints, one on each side of the bisecting line
        CreatePedestrianWaypointAt(waypoint.transform.position + bisectingDir * adjustedLaneWidth, waypoint);
        CreatePedestrianWaypointAt(waypoint.transform.position - bisectingDir * adjustedLaneWidth, waypoint);
    }

    static void CreatePedestrianWaypointForAdjacentPair(Waypoint waypoint, Waypoint adjacent1, Waypoint adjacent2)
    {
        Vector3 dir1 = (adjacent1.transform.position - waypoint.transform.position).normalized;
        Vector3 dir2 = (adjacent2.transform.position - waypoint.transform.position).normalized;

        // Adjust lane width based on the angle
        // For example, using the cosine of the angle, ensuring it's not less than a minimum threshold
        float angle = Vector3.Angle(dir1, dir2) * Mathf.Deg2Rad;
        float minScaleFactor = 0.7f; // Minimum scale factor
        float scaleFactor = Mathf.Max(Mathf.Sin(angle), minScaleFactor);
        float adjustedLaneWidth = laneWidth * scaleFactor;


        // Calculate the average direction
        Vector3 avgDir = AverageDirection(dir1, dir2);
        Vector3 crossProduct = Vector3.Cross(dir1, dir2);

        // Determine if the average direction needs to be flipped
        if (Vector3.Dot(crossProduct, Vector3.up) > 0)
        {
            avgDir = -avgDir;
        }

        CreatePedestrianWaypointAt(waypoint.transform.position + avgDir * adjustedLaneWidth, waypoint);
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
        if (intersectionPedWaypointsMap.ContainsKey(currentWaypoint))
        {
            intersectionPedWaypointsMap[currentWaypoint].Add(newWaypoint);
        }
        else
        {
            intersectionPedWaypointsMap[currentWaypoint] = new List<Waypoint> { newWaypoint };
        }

        pedWaypointCenters[newWaypoint] = currentWaypoint;
    }


    // STEP 2: Connect pedestrian waypoints between different waypoints

    [MenuItem("Tools/Sidewalks/2. Connect between waypoints")]
    public static void ConnectExternalPedestrianWaypoints()
    {
        // Iterate through all mappings
        foreach (var pair in intersectionPedWaypointsMap)
        {
            Waypoint originalWaypoint = pair.Key;
            List<Waypoint> pedestrianWaypoints = pair.Value;

            // Connect each pedestrian waypoint with appropriate waypoints of adjacent waypoints
            foreach (var pedestrianWaypoint in pedestrianWaypoints)
            {
                foreach (var adjacent in originalWaypoint.adjacentWaypoints)
                {
                    if (intersectionPedWaypointsMap.TryGetValue(adjacent, out List<Waypoint> adjacentPedestrianWaypoints))
                    {
                        foreach (var adjacentPedestrianWaypoint in adjacentPedestrianWaypoints)
                        {
                            // Check if connection intersects with original graph
                            if (!DoesIntersectWithGraph(originalWaypoint, pedestrianWaypoint, adjacentPedestrianWaypoint) &&
                                !DoesIntersectWithGraph(adjacent, pedestrianWaypoint, adjacentPedestrianWaypoint))
                            {
                                pedestrianWaypoint.AddAdjacentWaypoint(adjacentPedestrianWaypoint);
                            }
                        }
                    }
                }
            }
        }
    }

    static bool DoesIntersectWithGraph(Waypoint originalWaypoint, Waypoint pedestrian1, Waypoint pedestrian2)
    {
        // Line segment from pedestrian1 to pedestrian2
        Vector3 c = pedestrian1.transform.position;
        Vector3 d = pedestrian2.transform.position;

        // Check for intersection with all edges connected to the original waypoint
        foreach (var adjacent in originalWaypoint.adjacentWaypoints)
        {
            Vector3 a = originalWaypoint.transform.position;
            Vector3 b = adjacent.transform.position;

            if (LineSegmentsIntersect(a, b, c, d))
            {
                return true; // Intersects with one of the edges
            }
        }

        return false; // No intersection found
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

    // STEP 3: Connect pedestrian waypoints within the same waypoint

    [MenuItem("Tools/Sidewalks/3. Connect within waypoints")]
    public static void ConnectInternalPedestrianWaypoints()
    {
        // Assuming pedestrianWaypointsMap is accessible here. If not, you might need to pass it as an argument
        foreach (var pair in PedestrianPathGenerator.intersectionPedWaypointsMap)
        {
            List<Waypoint> pedestrianWaypoints = pair.Value;

            // Only connect for waypoints with 3 or more pedestrian waypoints
            // if (pedestrianWaypoints.Count < 3) continue;

            // Sort the waypoints in a clockwise order
            Vector3 originalPosition = pair.Key.transform.position;
            pedestrianWaypoints = pedestrianWaypoints.OrderBy(p => AngleFromReference(originalPosition, p.transform.position)).ToList();

            // Connect each pedestrian waypoint to the next and previous one in the list
            for (int i = 0; i < pedestrianWaypoints.Count; i++)
            {
                Waypoint current = pedestrianWaypoints[i];
                Waypoint next = pedestrianWaypoints[(i + 1) % pedestrianWaypoints.Count]; // Next in clockwise order
                Waypoint previous = pedestrianWaypoints[(i - 1 + pedestrianWaypoints.Count) % pedestrianWaypoints.Count]; // Previous in clockwise order

                current.AddAdjacentWaypoint(next);
                current.AddAdjacentWaypoint(previous);
            }
        }
    }

    // STEP 4: Connect pedestrian waypoints to the original graph (requires to split graph)
    [MenuItem("Tools/Sidewalks/4. Add Crosswalk intersections to graph")]
    public static void AddCrosswalksToGraph()
    {
        EdgeLoader.LoadEdges();

        var graph = Object.FindFirstObjectByType<Graph>();
        var allEdges = graph.GetAllEdges();
        var pedestrianEdges = allEdges.Where(edge => edge.isPedestrianOnly).ToList();
        var roadEdges = allEdges.Except(pedestrianEdges).ToList();

        // This is a dictionary that will store the intersecting edges for each edge
        Dictionary<ReducedEdge, List<ReducedEdge>> intersectingEdgesOverride = new Dictionary<ReducedEdge, List<ReducedEdge>>();
        // Helper method to add edges to the dictionary
        void AddIntersectingEdge(ReducedEdge keyEdge, ReducedEdge intersectingEdge)
        {
            // we keep track of intersecting edges using reduced edges, which are just
            // stripped down versions of the edges that only contain the start and end waypoints
            if (intersectingEdgesOverride.ContainsKey(keyEdge))
            {
                intersectingEdgesOverride[keyEdge].Add(intersectingEdge);
            }
            else
            {
                intersectingEdgesOverride[keyEdge] = new List<ReducedEdge> { intersectingEdge };
            }
        }


        List<Edge> crosswalkEdges = new List<Edge>();

        foreach (var pedEdge in pedestrianEdges)
        {
            bool hasEdgeBeenProcessed = false;
            foreach (var crosswalk in crosswalkEdges)
            {
                if (pedEdge.isSameEdge(crosswalk))
                {
                    foreach (var intersectingEdge in crosswalk.IntersectingEdges)
                    {
                        AddIntersectingEdge(new ReducedEdge(pedEdge), new ReducedEdge(intersectingEdge));
                    }
                    foreach (var intersectingEdge in pedEdge.IntersectingEdges)
                    {
                        AddIntersectingEdge(new ReducedEdge(crosswalk), new ReducedEdge(intersectingEdge));
                    }
                    hasEdgeBeenProcessed = true;
                    break;
                }
            }
            if (hasEdgeBeenProcessed) continue;

            foreach (var roadEdge in roadEdges)
            {
                if (TryGetIntersection(pedEdge, roadEdge, out Vector3 intersectionPoint))
                {
                    // the center of the intersection
                    Waypoint intersectionCenter = pedWaypointCenters[pedEdge.startWaypoint];

                    // special handling for intersections with only two pedestrian waypoints
                    if (intersectionPedWaypointsMap[intersectionCenter].Count == 2)
                    {
                        float distance = laneWidth * 0.5f; // Override the lane width for intersections with two pedestrian waypoints

                        // getting both waypoint directions
                        Waypoint wp0 = intersectionCenter.adjacentWaypoints[0];
                        Waypoint wp1 = intersectionCenter.adjacentWaypoints[1];

                        Vector3 directionWp0 = (wp0.transform.position - intersectionCenter.transform.position).normalized;
                        Vector3 directionWp1 = (wp1.transform.position - intersectionCenter.transform.position).normalized;

                        Waypoint subdividedWp0 = createSubdividedWaypoint(intersectionCenter.transform.position + directionWp0 * distance, intersectionCenter);
                        Waypoint subdividedWp1 = createSubdividedWaypoint(intersectionCenter.transform.position + directionWp1 * distance, intersectionCenter);

                        // Skip if the waypoints already exist
                        if (subdividedWp0 == null || subdividedWp1 == null) continue;

                        // resetting the adjacent waypoints
                        wp0.adjacentWaypoints.Remove(intersectionCenter);
                        wp1.adjacentWaypoints.Remove(intersectionCenter);

                        wp0.adjacentWaypoints.Add(subdividedWp0);
                        wp1.adjacentWaypoints.Add(subdividedWp1);

                        subdividedWp0.adjacentWaypoints.Add(subdividedWp1);
                        subdividedWp0.adjacentWaypoints.Add(wp0);

                        subdividedWp1.adjacentWaypoints.Add(subdividedWp0);
                        subdividedWp1.adjacentWaypoints.Add(wp1);

                        // get rid of the original intersection
                        intersectionCenter.adjacentWaypoints.Remove(wp0);
                        intersectionCenter.adjacentWaypoints.Remove(wp1);
                        GameObject.DestroyImmediate(intersectionCenter);

                        // set up intersecting edges


                        continue;
                    }


                    Waypoint oppositeIntersectionWaypoint;
                    if (intersectionCenter == roadEdge.startWaypoint)
                    {
                        oppositeIntersectionWaypoint = roadEdge.endWaypoint;
                    }
                    else
                    {
                        oppositeIntersectionWaypoint = roadEdge.startWaypoint;
                    }

                    // Calculate points to divide the pedestrian edge
                    Vector3 pointCloserToIntersection = CalculateDivisionPoint(intersectionPoint, intersectionCenter, laneWidth * 0.2f, true);
                    Vector3 pointFurtherFromIntersection = CalculateDivisionPoint(intersectionPoint, intersectionCenter, laneWidth * 0.2f, false);

                    // Replace or modify the pedestrian edge in the graph
                    var closerWaypoint = createSubdividedWaypoint(pointCloserToIntersection, intersectionCenter);
                    var furtherWaypoint = createSubdividedWaypoint(pointFurtherFromIntersection, intersectionCenter);
                    if (closerWaypoint == null || furtherWaypoint == null) continue; // Skip if the waypoints already exist

                    // 0. Remove intersection adjacency
                    intersectionCenter.adjacentWaypoints.Remove(oppositeIntersectionWaypoint);
                    oppositeIntersectionWaypoint.adjacentWaypoints.Remove(intersectionCenter);

                    // 1. Set closerWaypoint and furtherWaypoint as adjacent to each other
                    closerWaypoint.adjacentWaypoints.Add(furtherWaypoint);
                    furtherWaypoint.adjacentWaypoints.Add(closerWaypoint);

                    // 2. Set closerWaypoint and intersectionCenter as adjacent to each other
                    closerWaypoint.adjacentWaypoints.Add(intersectionCenter);
                    intersectionCenter.adjacentWaypoints.Add(closerWaypoint);

                    // 3. Set furtherWaypoint and oppositeIntersectionWaypoint as adjacent to each other
                    furtherWaypoint.adjacentWaypoints.Add(oppositeIntersectionWaypoint);
                    oppositeIntersectionWaypoint.adjacentWaypoints.Add(furtherWaypoint);

                    // 4. Update the road edge to use the new waypoints
                    var oppositeDirectionPedEdge = graph.GetEdge(pedEdge.endWaypoint, pedEdge.startWaypoint);
                    var oppositeDirectionRoadEdge = graph.GetEdge(roadEdge.endWaypoint, roadEdge.startWaypoint);

                    if (roadEdge.startWaypoint == intersectionCenter)
                    {
                        roadEdge.startWaypoint = furtherWaypoint;
                        oppositeDirectionRoadEdge.endWaypoint = furtherWaypoint;
                    }
                    else
                    {
                        roadEdge.endWaypoint = furtherWaypoint;
                        oppositeDirectionRoadEdge.startWaypoint = furtherWaypoint;
                    }

                    // Adding the intersecting edges
                    ReducedEdge subdividedRoadEdge = new ReducedEdge(closerWaypoint, furtherWaypoint);
                    ReducedEdge subdividedOppositeRoadEdge = new ReducedEdge(furtherWaypoint, closerWaypoint);
                    ReducedEdge reducedPedEdge = new ReducedEdge(pedEdge.startWaypoint, pedEdge.endWaypoint);
                    ReducedEdge reducedOppositePedEdge = new ReducedEdge(pedEdge.endWaypoint, pedEdge.startWaypoint);
                    AddIntersectingEdge(reducedPedEdge, subdividedRoadEdge);
                    AddIntersectingEdge(reducedPedEdge, subdividedOppositeRoadEdge);
                    AddIntersectingEdge(reducedOppositePedEdge, subdividedRoadEdge);
                    AddIntersectingEdge(reducedOppositePedEdge, subdividedOppositeRoadEdge);

                    AddIntersectingEdge(subdividedRoadEdge, reducedPedEdge);
                    AddIntersectingEdge(subdividedRoadEdge, reducedOppositePedEdge);
                    AddIntersectingEdge(subdividedOppositeRoadEdge, reducedPedEdge);
                    AddIntersectingEdge(subdividedOppositeRoadEdge, reducedOppositePedEdge);
                }
            }

            crosswalkEdges.Add(pedEdge);
        }

        // we re-load the edges to update the graph.
        EdgeLoader.LoadEdges(intersectingEdgesOverride);
    }

    private static bool TryGetIntersection(Edge pedestrianEdge, Edge roadEdge, out Vector3 intersectionPoint)
    {
        intersectionPoint = Vector3.zero; // Default to zero if no intersection is found

        Vector3 p1 = pedestrianEdge.startWaypoint.transform.position;
        Vector3 p2 = pedestrianEdge.endWaypoint.transform.position;
        Vector3 p3 = roadEdge.startWaypoint.transform.position;
        Vector3 p4 = roadEdge.endWaypoint.transform.position;

        float denominator = (p1.x - p2.x) * (p3.z - p4.z) - (p1.z - p2.z) * (p3.x - p4.x);

        // Check if lines are parallel (denominator is zero)
        if (Mathf.Approximately(denominator, 0)) return false;

        float t = ((p1.x - p3.x) * (p3.z - p4.z) - (p1.z - p3.z) * (p3.x - p4.x)) / denominator;
        float u = -((p1.x - p2.x) * (p1.z - p3.z) - (p1.z - p2.z) * (p1.x - p3.x)) / denominator;

        // Check if intersection point is on both line segments
        if (t >= 0 && t <= 1 && u >= 0 && u <= 1)
        {
            intersectionPoint.x = p1.x + t * (p2.x - p1.x);
            intersectionPoint.z = p1.z + t * (p2.z - p1.z);
            // Assuming the y-coordinate is constant or not relevant for intersection calculation
            return true;
        }

        return false;
    }

    private static Vector3 CalculateDivisionPoint(Vector3 intersectionPoint, Waypoint intersectionCenter, float distance, bool towardsCenter)
    {
        Vector3 directionToCenter = (intersectionCenter.transform.position - intersectionPoint).normalized;
        if (!towardsCenter)
        {
            // If we want the point away from the center, reverse the direction
            directionToCenter = -directionToCenter;
        }

        // Calculate the new point by moving the intersection point along the direction vector by the specified distance
        Vector3 divisionPoint = intersectionPoint + directionToCenter * distance;

        return divisionPoint;
    }

    private static Waypoint createSubdividedWaypoint(Vector3 position, Waypoint center)
    {
        // check if a waypoint already exists at the position, and if so, return it
        foreach (var waypoint in subdividedWaypoints)
        {
            if (waypoint.transform.position == position)
            {
                return waypoint;
            }
        }

        GameObject newWaypointObj = new GameObject("Subdivided Waypoint");
        newWaypointObj.transform.position = position;
        Waypoint newWaypoint = newWaypointObj.AddComponent<Waypoint>();
        newWaypoint.adjacentWaypoints = new List<Waypoint>();  // Initialize the list here
        newWaypoint.isPedestrianOnly = false;
        newWaypoint.isSubdivided = true;
        newWaypointObj.transform.parent = Object.FindObjectOfType<Graph>().transform;

        subdividedWaypoints.Add(newWaypoint);

        return newWaypoint;
    }

    // STEP 5: clear the map
    [MenuItem("Tools/Sidewalks/5. Clear pedestrian waypoints")]
    public static void ClearPedestrianPaths()
    {
        List<Waypoint> waypoints = Object.FindObjectsOfType<Waypoint>().ToList();
        List<Waypoint> toDelete = new List<Waypoint>();

        for (int i = 0; i < waypoints.Count; i++)
        {
            if (waypoints[i].isPedestrianOnly)
            {
                toDelete.Add(waypoints[i]);
            }
        }

        for (int i = 0; i < toDelete.Count; i++)
        {
            Object.DestroyImmediate(toDelete[i].gameObject);
        }

        ClearStaticVariables();
        WaypointEditor.PruneDeletedWaypoints();
        EdgeLoader.LoadEdges();
    }

    // Run all steps
    [MenuItem("Tools/Sidewalks/0. Run all pedestrian path steps")]
    public static void RunAllSteps()
    {
        ClearPedestrianPaths(); // 5 - clear graph 
        GeneratePedestrianWaypoints(); // 1
        ConnectExternalPedestrianWaypoints(); // 2
        ConnectInternalPedestrianWaypoints(); // 3
    }

    [MenuItem("Tools/Sidewalks/9. Clear Static Variables")]
    public static void ClearStaticVariables()
    {
        // Reset the laneWidth to its default value if necessary
        laneWidth = 3.5f; // Or set it to another default value as needed

        // Clear the collections
        intersectionPedWaypointsMap.Clear();
        pedWaypointCenters.Clear();
        subdividedWaypoints.Clear();
    }
}