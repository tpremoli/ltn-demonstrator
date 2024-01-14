using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointMover : MonoBehaviour
{
    // Attributes from the Traveller class
    private float totalDistanceMoved;
    private float positionOnEdge;
    private float currentVelocity;
    private float maxVelocity = 5f; // Will be assigned according to agent category enum (EdgeFunctionality)
    private int noOfPassengers;
    private float rateOfEmission;
    private Building destinationBuilding;
    // Movement controlling attributes
    private float leftToMove = 0;
    private float length = 0;
    private float terminalLength;
    private Edge currentEdge;
    private List<WaypointMover> WaitingToMove;
    private List<Edge> pathEdges;

    // Attributes from the WaypointMover class
    [SerializeField] private float distanceThreshold = 0.1f;
    [SerializeField] private float leftLaneOffset = 1f;

    private WaypointPath path;           // Instance of the pathfinding class
    private Graph graph;                 // Instance of the graph class
    
    void Start()
    {
        // Initialising object
        this.WaitingToMove = new List<WaypointMover>();
        this.pathEdges = new List<Edge>();

        // Start generating path to be taken
        this.graph = GameObject.Find("Graph").GetComponent<Graph>();

        // Choose a random destination building.
        chooseDestinationBuilding();

        // Initialize the path with the starting waypoint
        path = new WaypointPath(this.transform.position, destinationBuilding.GetClosestPointOnEdge(), this);

        if (path.path == null)
        {
            Edge endEdge = destinationBuilding.closestEdge;
            // If no path is found, destroy the object.
            // Later on, we should change this so that the traveller changes their mode of transport
            Debug.LogWarning("Path doesn't exist for Traveller " + this.gameObject.name + ". Destroying object.");
            Debug.LogWarning("End edge start: " + endEdge.StartWaypoint.name + " End edge end: " + endEdge.EndWaypoint.name);
            Destroy(this.gameObject);
        }
        IEnumerator<Waypoint> iter = path.path.GetEnumerator();
        iter.MoveNext();
        Waypoint old_wp = null;
        Waypoint wp = iter.Current;

        // Convert the path into a list of edges
        while (iter.MoveNext()) {
            old_wp = wp;
            wp = iter.Current;
            Edge nextOne = this.graph.getEdge(old_wp, wp);
            Debug.Log("Path from: "+old_wp.name+"  to: "+wp.name+"\nEdge: ");
            if (nextOne==null){
                Debug.LogError("There is no path connecting nodes.");
            } else{
                this.pathEdges.Add(nextOne);
            }
            
        }
        // Position the traveller on the current Edge
        this.currentEdge = graph.getClosetEdge(this.transform.position);
        this.currentEdge.Subscribe(this);
        this.positionOnEdge = this.currentEdge.GetCosestPointAsFractionOfEdge(this.transform.position);
        // Obtain terminal location
        Vector3 terminal = destinationBuilding.GetClosestPointOnEdge();
        Edge terminalEdge = graph.getClosetEdge(terminal);
        this.pathEdges.Add(terminalEdge);
        this.terminalLength = terminalEdge.GetCosestPointAsFractionOfEdge(terminal);

        Debug.Log("Traveller Instantiated");
        DebugDrawPath();
    }
    private void registerForMove(WaypointMover trav){
        // TODO add check that trav only gets added if it is not within the list already
        WaitingToMove.Add(trav);
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
        while (Vector3.Distance(this.transform.position, destinationBuilding.GetClosestPointOnEdge()) < distanceThreshold) {
            // Choose new random destination building.
            destinationBuilding = graph.buildings[Random.Range(0, graph.buildings.Count)];
        }
    }

    void Update()
    {
        // Calculate current velocity
        this.leftToMove = this.maxVelocity * Time.deltaTime; // Currently obtained from maxVelocity attribute, later should also consider the maximum velocity permitted by the edge
        Move();
    }
    void LateUpdate(){
        this.WaitingToMove = new List<WaypointMover>();
        UpdatePosition();
    }
    void UpdatePosition(){
        // Move the object's transform to:
        // edge_origin * (1-positionOnEdge) + edge_end * positionOnEdge
        Vector3 newPosition = this.currentEdge.StartWaypoint.transform.position*(1-this.positionOnEdge) + this.currentEdge.EndWaypoint.transform.position*(this.positionOnEdge);
        // Add offset to the new position to no be in the middle of the road
        newPosition = newPosition + this.leftLaneOffset*Vector3.Cross(Vector3.up,this.currentEdge.GetDirection());
        // Update the object's position
        transform.position = newPosition;
    }
    private void Move(){
        // Check if Traveller has moved to the end of its edge
        float proposedMovement = this.leftToMove;
        // Check for collissions at the new location
        bool proposalAccepted = false;
        while (!proposalAccepted) {
            proposalAccepted = true;
            // Determine which Edge the Traveller ends up on, and how much movement it uses up in the process
            float TravelledOverEdges = 0;
            Edge terminalEdge = this.currentEdge;

            // Check whether proposed movement exceeds this edge
            if(proposedMovement>(this.currentEdge.Distance*(1-this.positionOnEdge))){
                // Add the current edge to the distance TravelledOverEdges
                TravelledOverEdges+=this.currentEdge.Distance*(1-this.positionOnEdge);
                // Calculate how much the Traveller travels over subsequent edges
                IEnumerator<Edge> iter = this.pathEdges.GetEnumerator();
                if (!iter.MoveNext()){
                        // Check whether there is a next edge, if not, the Traveller has reached its destination
                        this.currentEdge.Unsubscribe(this);
                        arriveToDestination();
                }
                // Calculate distance Travelled over subsequent Edges
                while(iter.Current.Distance < proposedMovement-TravelledOverEdges){
                    TravelledOverEdges+=iter.Current.Distance;
                    if (!iter.MoveNext()){
                        // Check whether there is a next edge, if not, the Traveller has reached its destination
                        this.currentEdge.Unsubscribe(this);
                        arriveToDestination();
                    }
                }
                terminalEdge = iter.Current;
            }
            // Determine how much movement in Edge the Traveller has left upon entering the edge
            float TravelledInEdge = proposedMovement - TravelledOverEdges;
            // Check for collission at that point in the edge with every traveller present on the edge
            foreach(WaypointMover wp in terminalEdge.TravellersOnEdge){
                if (wp==this) continue;
                float wpPositionOnEdgeInReal = terminalEdge.DeltaDToRealDistance(wp.positionOnEdge);
                // Check whether this' front would end up within the vehicle
                if(TravelledInEdge < wpPositionOnEdgeInReal && TravelledInEdge > (wpPositionOnEdgeInReal-wp.length)){
                    // Collission occurred
                    // Reduce proposed movement
                    proposedMovement-=TravelledInEdge-(wpPositionOnEdgeInReal-wp.length);
                    // register for notification
                    wp.registerForMove(this);
                    // and re-loop
                    proposalAccepted = false;
                    break;
                }
                // Check whether this' rear would end up within a vehilce
                if((TravelledInEdge-this.length) < wpPositionOnEdgeInReal && (TravelledInEdge-this.length) > (wpPositionOnEdgeInReal-wp.length)){
                    // Collission occurred
                    // Reduce proposed movement
                    proposedMovement-=TravelledInEdge-(wpPositionOnEdgeInReal-wp.length);
                    // register for notification
                    wp.registerForMove(this);
                    // and re-loop
                    proposalAccepted = false;
                    break;
                }
            }
            // No collission occurred, the proposed movement is accepted
        }
        // Beginning to carry out proposed movement
        this.leftToMove-=proposedMovement;
        // Keep switching edges until the proposed movement is insufficient to escape the edge
        while(proposedMovement>(this.currentEdge.Distance*(1-this.positionOnEdge))){
            proposedMovement-=(this.currentEdge.Distance*(1-this.positionOnEdge));
            // Exit current edge
            this.currentEdge.Unsubscribe(this);
            // Enter new edge
            this.currentEdge = this.pathEdges[0];
            this.pathEdges.RemoveAt(0);
            this.currentEdge.Subscribe(this);
            this.positionOnEdge = 0;
        }
        this.positionOnEdge+=this.currentEdge.RealDistanceToDeltaD(proposedMovement);
        // Proposed movement carried out

        // Check for arriving to destination
        if(this.pathEdges.Count==0&&this.positionOnEdge>=this.terminalLength){
            arriveToDestination();
        }
        
        // Call Move() on all Travellers waiting to Move
        foreach (WaypointMover trav in this.WaitingToMove){
            trav.Move();
        }
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
        if (Application.isPlaying && !(path==null))
        {
            // Draw the destination sphere
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(path.destinationPos, 1f);

            // Draw the path from the agent's current position
            Gizmos.color = Color.yellow;

            Vector3 startPosition = transform.position; // Use the agent's position if the currentWaypoint is null

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
