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

    private Building destinationBuilding;

    // Attributes from the WaypointMover class
    [SerializeField] private Waypoint startingWaypoint;
    [SerializeField] private float speed = 5f;
    [SerializeField] private float distanceThreshold = 0.1f;
    [SerializeField] private float leftLaneOffset = 1f;

    private WaypointPath path;           // Instance of the pathfinding class
    private Waypoint currentWaypoint;    // Current waypoint the mover is heading towards
    private Vector3 currentTargetPosition;
    private Graph graph;                 // Instance of the graph class


    // pick rnadom model and material
    [SerializeField] public List<GameObject> vehiclePrefabs;
    
    void Start()
    {
        this.graph = GameObject.Find("Graph").GetComponent<Graph>();

        // pick a random model and material
        pickRandomModelAndMaterial();

        chooseDestinationBuilding();

        // Initialize the path with the starting waypoint
        path = new WaypointPath(this.transform.position, destinationBuilding.closestPointOnEdge, this);

        if (path.path == null)
        {
            Edge endEdge = destinationBuilding.closestEdge;
            // If no path is found, destroy the object.
            // Later on, we should change this so that the traveller changes their mode of transport
            Debug.LogWarning("Path doesn't exist for Traveller " + this.gameObject.name + ". Destroying object.");
            Debug.LogWarning("End edge start: " + endEdge.StartWaypoint.name + " End edge end: " + endEdge.EndWaypoint.name);
            Destroy(this.gameObject);
        }

        // Get the first waypoint in the path and set the initial position
        currentWaypoint = path.PopNextWaypoint();
        updateTargetPosition();

        // We face the next destination (either the next waypoint or the destination)
        faceNextDestinationInit();
        moveToLeftLaneInit();

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

    // Choose a random destination from the possible buildings in the grid.
    public void chooseDestinationBuilding() {
        // Get list of buildings.
        // Choose random destination building.
        destinationBuilding = graph.buildings[Random.Range(0, graph.buildings.Count)];

        // Edge case where the chosen building is the same as the building the traveller spawned at.
        while (Vector3.Distance(this.transform.position, destinationBuilding.closestPointOnEdge) < distanceThreshold) {
            // Choose new random destination building.
            destinationBuilding = graph.buildings[Random.Range(0, graph.buildings.Count)];
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
                // this change in transform position is kinda dirty, but it works.
                // it essentially ensures all caluclations on direction & lanes are done relative to the original edges.
                transform.position = currentWaypoint.transform.position;
                faceNextDestination();
                moveToLeftLane();
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
        // Calculate the next destination location (either the next waypoint or the destination)
        Vector3 nextDestination = calcNextDestination();

        // Calculate the arrow direction from the current position to the next destination
        Vector3 arrowDirection = nextDestination - transform.position;

        // Normalize the arrow direction to ensure a consistent offset magnitude
        arrowDirection.Normalize();

        // Calculate the offset position based on the arrow direction
        Vector3 offsetPosition = nextDestination - arrowDirection * leftLaneOffset;

        // Rotate towards the offset position
        transform.LookAt(offsetPosition);
    }

    private void faceNextDestinationInit()
    {
        // this isn't a method as it's different to the above method
        Vector3 nextDestination;
        if (currentWaypoint == null)
        {
            // If the next waypoint is null, we face destination
            nextDestination = path.destinationPos;
        }
        else
        {
            // Otherwise, we face the next waypoint
            nextDestination = currentWaypoint.transform.position;
        }

        // Calculate the arrow direction from the current position to the next destination
        Vector3 arrowDirection = nextDestination - transform.position;

        // Normalize the arrow direction to ensure a consistent offset magnitude
        arrowDirection.Normalize();

        // Calculate the offset position based on the arrow direction
        Vector3 offsetPosition = nextDestination - arrowDirection * leftLaneOffset;

        // Rotate towards the offset position
        transform.LookAt(offsetPosition);
    }


    private void moveToLeftLane()
    {
        // Calculate the next destination location (either the next waypoint or the destination)
        Vector3 nextLocation = calcNextDestination();
        
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
    private Vector3 calcNextDestination(){
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
        return nextDestination;
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

    private void pickRandomModelAndMaterial(){
        // Randomly select a model and material
        GameObject selectedPrefab = vehiclePrefabs[Random.Range(0, vehiclePrefabs.Count-1)];

        // Retrieve the MeshRenderer and MeshFilter of the selected prefab
        MeshRenderer selectedMeshRenderer = selectedPrefab.GetComponent<MeshRenderer>();
        MeshFilter selectedMeshFilter = selectedPrefab.GetComponent<MeshFilter>();

        // Apply the mesh and material to the current GameObject
        if (selectedMeshRenderer != null && selectedMeshFilter != null)
        {
            MeshRenderer thisMeshRenderer = GetComponent<MeshRenderer>();
            MeshFilter thisMeshFilter = GetComponent<MeshFilter>();

            if (thisMeshRenderer != null && thisMeshFilter != null)
            {
                thisMeshRenderer.material = selectedMeshRenderer.sharedMaterial;
                thisMeshFilter.mesh = selectedMeshFilter.sharedMesh;
            }
            else
            {
                Debug.LogError("Current GameObject does not have MeshRenderer and/or MeshFilter.");
            }
        }
        else
        {
            Debug.LogError("Selected prefab does not have MeshRenderer and/or MeshFilter.");
        }
    }
}
