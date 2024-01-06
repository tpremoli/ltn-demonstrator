using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointMover : MonoBehaviour
{
    // Attributes from the Traveller class
    private float totalDistanceMoved;
    private float positionOnEdge;
    private float currentVelocity;
    private float maxVelocity; // Will be assigned according to agent category enum (EdgeFunctionality)
    private int noOfPassengers;
    private float rateOfEmission;

    // Attributes from the WaypointMover class
    [SerializeField] private Waypoint startingWaypoint;
    [SerializeField] private float speed = 5f;
    [SerializeField] private float distanceThreshold = 0.1f;
    [SerializeField] private float leftLaneOffset = 1f;

    private WaypointPath path;           // Instance of the pathfinding class
    private Waypoint currentWaypoint;    // Current waypoint the mover is heading towards
    private Vector3 currentTargetPosition;
    private Graph graph;                 // Instance of the graph class

    void Start()
    {
        this.graph = GameObject.Find("Graph").GetComponent<Graph>();

        // The building spawns the Mover, so Mover is at the Building position
        Edge endEdge = graph.edges[Random.Range(0, graph.edges.Count)];

        // Initialize the path with the starting waypoint
        path = new WaypointPath(this.transform.position, endEdge.GetRandomPointOnEdge(), this);

        if (path.path == null)
        {
            // If no path is found, destroy the object.
            // Later on, we should change this so that the traveller changes their mode of transport
            Debug.LogWarning("Path doesn't exist for Traveller " + this.gameObject.name + ". Destroying object.");
            Debug.LogWarning("End edge start: " + endEdge.StartWaypoint.name + " End edge end: " + endEdge.EndWaypoint.name);
            Destroy(this.gameObject);
        }

        // Get the first waypoint in the path and set the initial position
        currentWaypoint = path.PopNextWaypoint();
        updateTargetPosition();

        // if the path is null, then the traveller is on the same edge as the destination
        faceNextDestination();
        moveToLeftLaneInit();
        // TODO: posittion to left lane + set destination to left of waypoint

        Debug.Log("Traveller Instantiated");
        DebugDrawPath();
    }

    void DebugDrawPath()
    {
        if (path != null && path.path != null && path.path.Count > 0)
        {
            Vector3[] pathPositions = new Vector3[path.path.Count + 1];
            pathPositions[0] = transform.position;

            for (int i = 0; i < path.path.Count; i++)
            {
                pathPositions[i + 1] = path.path[i].transform.position;
            }

            // Draw the path using Debug.DrawLine
            for (int i = 0; i < pathPositions.Length - 1; i++)
            {
                Debug.DrawLine(pathPositions[i], pathPositions[i + 1], Color.yellow);
            }
        }
    }


    void Update()
    {
        // Move towards the target position using linear interpolation
        transform.position = Vector3.MoveTowards(transform.position, currentTargetPosition, speed * Time.deltaTime);

        // Check the distance to the target position
        if (Vector3.Distance(transform.position, currentTargetPosition) < distanceThreshold)
        {
            if (currentWaypoint != null)
            {
                moveToLeftLane();
                faceNextDestination();
                currentWaypoint = path.PopNextWaypoint();
                updateTargetPosition();
            }
            else
            {
                // If we're close enough to the destination and there's no waypoint, destroy the object
                arriveToDestination();
                updateTargetPosition();
            }
        }
    }

    private void updateTargetPosition()
    {
        // converts the current target position to the left of the line between waypoints
        Vector3 nextPosition =  currentWaypoint == null ? path.destinationPos : currentWaypoint.transform.position;
        Vector3 nextMoveDirection = nextPosition - transform.position;
        nextMoveDirection.Normalize();
        Vector3 leftVector = new Vector3(-nextMoveDirection.z, 0, nextMoveDirection.x);
        currentTargetPosition = nextPosition + leftVector * leftLaneOffset;
    }
    private void faceNextDestination()
    {
        Waypoint nextWaypoint = path.PeekNextWaypoint();
        Vector3 nextDestination;
        if (nextWaypoint == null)
        {
            // If the next waypoint is null, we face destination
            nextDestination = path.destinationPos;
        }
        else
        {
            // Otherwise, we face the next waypoint
            nextDestination = nextWaypoint.transform.position;
        }

        // Calculate the arrow direction from the current position to the next destination
        Vector3 arrowDirection = nextDestination - transform.position;

        // Normalize the arrow direction to ensure a consistent offset magnitude
        arrowDirection.Normalize();

        // Calculate the offset position based on the arrow direction
        Vector3 offsetPosition = nextDestination - arrowDirection * 2f;

        // Rotate towards the offset position
        transform.LookAt(offsetPosition);
    }

    private void moveToLeftLane()
    {
        Waypoint nextWaypoint = path.PeekNextWaypoint();
        Vector3 nextLocation;
        if (nextWaypoint == null)
        {
            // If the next waypoint is null, we face destination
            nextLocation = path.destinationPos;
        }
        else
        {
            // Otherwise, we face the next waypoint
            nextLocation = nextWaypoint.transform.position;
        }
        // we want to offset the position to the left, to simulate the agent being on the left side of the road
        Vector3 nextMoveDirection = nextLocation - transform.position;
        nextMoveDirection.Normalize();
        Vector3 leftVector = new Vector3(-nextMoveDirection.z, 0, nextMoveDirection.x);

        // Set the new position to the left of the line between waypoints
        transform.position = currentWaypoint.transform.position + leftVector * leftLaneOffset;
    }

    private void moveToLeftLaneInit()
    {
        Waypoint nextWaypoint = path.PeekNextWaypoint();
        Vector3 nextLocation;
        if (nextWaypoint == null)
        {
            // If the next waypoint is null, we face destination
            nextLocation = path.destinationPos;
        }
        else
        {
            // Otherwise, we face the next waypoint
            nextLocation = nextWaypoint.transform.position;
        }
        // we want to offset the position to the left, to simulate the agent being on the left side of the road
        Vector3 nextMoveDirection = nextLocation - transform.position;
        nextMoveDirection.Normalize();
        Vector3 leftVector = new Vector3(-nextMoveDirection.z, 0, nextMoveDirection.x);

        // Set the new position to the left of the line between waypoints
        transform.position = transform.position + leftVector * leftLaneOffset;
    }

    public void arriveToDestination()
    {
        Debug.Log("Arrived to destination. Destroying object.");
        Destroy(this.gameObject);
    }

    public float calculateEmissions()
    {
        // Calculate emissions using the rateOfEmission attribute and totalDistanceMoved
        return rateOfEmission * totalDistanceMoved;
    }

    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            // Draw the destination sphere
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(path.destinationPos, 1f);
            if (currentWaypoint != null)
            {
                // Draw the current waypoint sphere
                Gizmos.DrawSphere(currentWaypoint.transform.position, 1f);
            }

            // Draw the path from the agent's current position
            Gizmos.color = Color.yellow;

            Vector3 startPosition = currentWaypoint != null ? currentWaypoint.transform.position : transform.position; // Use the agent's position if the currentWaypoint is null

            if (path.path.Count > 0)
            {
                // Iterate through the remaining waypoints
                foreach (var waypoint in path.path)
                {
                    // Draw a sphere for each waypoint
                    Gizmos.DrawSphere(waypoint.transform.position, 1f);

                    // Draw an arrow from the start position to the current waypoint
                    DrawArrow.ForGizmo(startPosition + Vector3.up, (waypoint.transform.position - startPosition) + Vector3.up, Color.yellow, 1f);

                    // Update the start position
                    startPosition = waypoint.transform.position;
                }
            }

            // Always draw an arrow to the destination from the current position or last waypoint
            DrawArrow.ForGizmo(startPosition + Vector3.up, (path.destinationPos - startPosition) + Vector3.up, Color.yellow, 1f);
        }
    }
}
