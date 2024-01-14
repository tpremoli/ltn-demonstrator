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

    // pick rnadom model and material
    [SerializeField] public List<GameObject> vehiclePrefabs;
    
    void Start()
    {
        // Initialising object
        this.WaitingToMove = new List<WaypointMover>();
        this.pathEdges = new List<Edge>();

        // Start generating path to be taken
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
            return;
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
        // Get origin Edge
        Edge originEdge = graph.getClosetEdge(this.transform.position);
        if(path.path.Count>0){
            if(originEdge.EndWaypoint!=path.path[0]){
            // If the edge does not end in the correct waypoint, look for counterpart
            originEdge = graph.getEdge(originEdge.endWaypoint, originEdge.startWaypoint);
            // If counterpart does not exist, terminate
            if (originEdge==null){
                Destroy(this.gameObject);
                return;
            }
        }
        }
        // Get Terminal Edge
        Edge terminalEdge = destinationBuilding.closestEdge;
        if(path.path.Count>0){
            if(terminalEdge.StartWaypoint!=path.path[path.path.Count-1]){
            // If the edge does not end in the correct waypoint, look for counterpart
            terminalEdge = graph.getEdge(terminalEdge.endWaypoint, terminalEdge.startWaypoint);
            // If counterpart does not exist, terminate
            if (terminalEdge==null){
                Destroy(this.gameObject);
                return;
            }
        }
        }

        // Position the traveller on the current Edge
        this.currentEdge = originEdge;
        this.currentEdge.Subscribe(this);
        this.positionOnEdge = this.currentEdge.GetClosestPointAsFractionOfEdge(this.transform.position);
        // Obtain terminal location
        Vector3 terminal = destinationBuilding.closestPointOnEdge;
        this.pathEdges.Add(terminalEdge);
        this.terminalLength = terminalEdge.GetClosestPointAsFractionOfEdge(terminal);

        Debug.Log("Traveller Instantiated");
        // Rotate the Traveller to align with the current edge
        updateHeading();

        // Set Traveller's size
        renderer.bounds.size

        // DEBUG
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
        while (Vector3.Distance(this.transform.position, destinationBuilding.closestPointOnEdge) < distanceThreshold) {
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
    private void updateHeading(){
        Vector3 direction = this.currentEdge.GetDirection();
        Quaternion rotation = Quaternion.LookRotation(direction, Vector3.up);
        transform.rotation = rotation;
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
        bool escapedEdge = false;
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
            // Set the variable
            escapedEdge = true;
        }
        this.positionOnEdge+=this.currentEdge.RealDistanceToDeltaD(proposedMovement);
        // Proposed movement carried out

        // Rotate the traveller if it changed an edge
        if (escapedEdge){
            updateHeading();
        }

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

            if (path.path.Count > 0)
            {
                // Iterate through the remaining waypoints
                foreach (var waypoint in path.path)
                {
                    // Draw a sphere for each waypoint
                    Gizmos.DrawSphere(waypoint.transform.position, 1f);
                }
            }

            // Draw the path from the agent's current position
            tracePath(Color.yellow, 1f, 1f);
        }
    }
    private void OnDrawGizmosSelected(){
        if (Application.isPlaying && !(path==null))
        {
            // Draw the destination sphere
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(path.destinationPos, 1f);

            if (path.path.Count > 0)
            {
                // Iterate through the remaining waypoints
                foreach (var waypoint in path.path)
                {
                    // Draw a sphere for each waypoint
                    Gizmos.DrawSphere(waypoint.transform.position, 1f);
                }
            }

            // Draw the path from the agent's current position
            tracePath(Color.red, 2f, 10f);
        }
    }

    private void tracePath(Color c, float height, float thickness){
        // Draw the path from the agent's current position
        Gizmos.color = c;
        Vector3 startPosition = currentEdge.startWaypoint.transform.position + Vector3.up*height;
        Vector3 endPosition = currentEdge.endWaypoint.transform.position + Vector3.up*height - startPosition;
        DrawArrow.ForGizmo(startPosition, endPosition, c, thickness);

        // Draw the path for intermediate segments
        if (this.pathEdges.Count>1){
            foreach (Edge edge in this.pathEdges.GetRange(0,this.pathEdges.Count-1)){
            startPosition = edge.startWaypoint.transform.position + Vector3.up*height;
            endPosition = edge.endWaypoint.transform.position + Vector3.up*height - startPosition;
            DrawArrow.ForGizmo(startPosition, endPosition, c, thickness);
        }
        }

        // Draw the final segment
        Edge e = this.pathEdges[this.pathEdges.Count-1];
        startPosition = e.startWaypoint.transform.position + Vector3.up*height;
        endPosition = (e.endWaypoint.transform.position + Vector3.up*height - startPosition)*this.terminalLength;
        DrawArrow.ForGizmo(startPosition, endPosition, c, thickness);

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
