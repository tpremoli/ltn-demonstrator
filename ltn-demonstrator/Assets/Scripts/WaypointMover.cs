using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointMover : MonoBehaviour
{
    // Attributes controlling the object state
    bool initialised;   // Controls whether object has been initialised and should begin travelling

    // Attributes controlling vehicle's type
    VehicleProperties vType;

    // Statistic measures
    private float totalDistanceMoved;

    // Attribute serving movement
    public float leftToMove // Control how much the traveller has left to move in this frame
    {
        get; private set;
    }
    private float distanceAlongEdge;    // Controls how far along current edge the traveller is
    private float terminalLength;       // Controls how far along the last edge in pathEdges / currentEdge if the previous is empty the terminal destination is
    private Edge currentEdge;           // Controls which edge the traveller currently occupies
    private List<Edge> pathEdges;       // List of edges the traveller needs to travel

    // Attributes serving collissions
    private List<WaypointMover> travsBlockedByThis;     // List of travellers prevented from moving by this traveller
    private List<WaypointMover> travsBlockingThis;      // List of travellers preventing this traveller from moving
    private List<WaypointMover> CachedRejectedWaiting;  // Travellers that should be declines from addition to travsBlockedByThis
    private float length = 0;                           // The length of the edge the traveller occupies
    private float hLen = 0;                             // half-length

    // Debug attributes - velocity & velocity calculation
    public float velocity
    {
        get; private set;
    }
    private Edge edgeAtFrameBeginning;
    private float distanceAlongEdgeAtFrameBeginning;

    // Debug attributes - collissions
    private List<WaypointMover> travsBlockedByThisDEBUG;
    private List<WaypointMover> travsBlockingThisDEBUG;

    // Attributes from the WaypointMover class
    [SerializeField] private float distanceThreshold = 0.1f;
    [SerializeField] private float leftLaneOffset = 1f;

    // Attributes for pathfinding
    private WaypointPath path;           // Instance of the pathfinding class
    private Graph graph;                 // Instance of the graph class
    private Building destinationBuilding;

    void Start()
    {
        // Start by making the object unready
        this.initialised = false;
        gameObject.GetComponent<Renderer>().enabled = false;

        // Initialising object
        this.pathEdges = new List<Edge>();
        this.travsBlockedByThis = new List<WaypointMover>();
        this.travsBlockedByThisDEBUG = new List<WaypointMover>();
        this.travsBlockingThis = new List<WaypointMover>();
        this.travsBlockingThisDEBUG = new List<WaypointMover>();

        // pick a random model and material
        this.vType = pickRandomVehicleType();
        setVehicleModelAndMaterial();

        var r = GetComponent<Collider>();
        if (r != null)
        {
            var bounds = r.bounds;
            this.length = Mathf.Max(Mathf.Max(bounds.size.x, bounds.size.y), bounds.size.z);
            this.hLen = length / 2;
        }

        // Start generating path to be taken
        this.graph = GameObject.Find("Graph").GetComponent<Graph>();

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
        while (iter.MoveNext())
        {
            old_wp = wp;
            wp = iter.Current;
            Edge nextOne = this.graph.getEdge(old_wp, wp);
            Debug.Log("Path from: " + old_wp.name + "  to: " + wp.name + "\nEdge: ");
            if (nextOne == null)
            {
                Debug.LogError("There is no path connecting nodes.");
            }
            else
            {
                this.pathEdges.Add(nextOne);
            }

        }
        // Get origin Edge
        Edge originEdge = graph.getClosetEdge(this.transform.position);
        if (path.path.Count > 0)
        {
            if (originEdge.EndWaypoint != path.path[0])
            {
                // If the edge does not end in the correct waypoint, look for counterpart
                originEdge = graph.getEdge(originEdge.endWaypoint, originEdge.startWaypoint);
                // If counterpart does not exist, terminate
                if (originEdge == null)
                {
                    Destroy(this.gameObject);
                    return;
                }
            }
        }
        // Get Terminal Edge
        Edge terminalEdge = destinationBuilding.closestEdge;
        if (path.path.Count > 0)
        {
            if (terminalEdge.StartWaypoint != path.path[path.path.Count - 1])
            {
                // If the edge does not end in the correct waypoint, look for counterpart
                terminalEdge = graph.getEdge(terminalEdge.endWaypoint, terminalEdge.startWaypoint);
                // If counterpart does not exist, terminate
                if (terminalEdge == null)
                {
                    Destroy(this.gameObject);
                    return;
                }
            }
        }
        // If the destination is further along the same Edge, do not add the terminal edge
        if (originEdge != terminalEdge)
        {
            this.pathEdges.Add(terminalEdge);
        }

        // Position the traveller on the current Edge
        this.currentEdge = originEdge;

        this.distanceAlongEdge = Vector3.Distance(
            this.currentEdge.startWaypoint.transform.position,
            this.transform.position
            );
        // Obtain terminal location
        Vector3 terminal = destinationBuilding.closestPointOnEdge;
        //this.terminalLength = terminalEdge.GetClosestPointAsFractionOfEdge(terminal);
        this.terminalLength = Vector3.Distance(
            terminalEdge.startWaypoint.transform.position,
            terminalEdge.GetClosestPoint(terminal)
            );
        // If the destination is along the same edge, but in opposite direction
        // reverse the current direction of travel, and regenerate the positions
        if (originEdge == terminalEdge && terminalLength < distanceAlongEdge)
        {
            this.currentEdge = graph.getEdge(currentEdge.endWaypoint, currentEdge.startWaypoint);
            if (currentEdge == null)
            {
                Destroy(this.gameObject);
                return;
            }
            this.distanceAlongEdge = Vector3.Distance(
                this.currentEdge.startWaypoint.transform.position,
                this.currentEdge.GetClosestPoint(this.transform.position)
                );
            this.terminalLength = Vector3.Distance(
                currentEdge.startWaypoint.transform.position,
                currentEdge.GetClosestPoint(terminal)
                );
        }

        Debug.Log("Traveller Instantiated");
        // Rotate the Traveller to align with the current edge
        updateHeading();

        // Attempt to register the Traveller with road
        StartCoroutine(waitUntilSpawnable());

        // DEBUG
        //DebugDrawPath();
    }

    // registers the traveller with the edge and makes it visible; 
    // it is necessarry for delayed spawns in case the traveller's spawn point would lead it to being inside another
    private IEnumerator waitUntilSpawnable()
    {
        //Calculating traveller's possition
        float myFront = this.distanceAlongEdge + hLen;
        float myRear = this.distanceAlongEdge - hLen;

        bool collided = true;
        while (collided)
        {
            collided = false;
            // COLLISION CHECK
            // Check for collision at that point in the edge with every traveller present on the edge
            foreach (WaypointMover wp in currentEdge.TravellersOnEdge)
            {
                if (wp == this) continue;
                //Calculate position of other traveller on the edge in Real units
                float wpPOEReal = wp.distanceAlongEdge;
                //Calculate position of
                float wpFront = wpPOEReal + wp.hLen + 0.5f;
                float wpRear = wpPOEReal - wp.hLen - 0.5f;

                // Check whether this' front would end up within the vehicle
                if (myFront <= wpFront && myFront >= wpRear)
                {
                    // Collision occurred
                    // re-loop
                    collided = true;
                    break;
                }

                // Check whether this' rear would end up within a vehilce
                if (myRear <= wpFront && myRear >= wpRear)
                {
                    // collision occurred

                    // re-loop
                    collided = true;
                    break;
                }
            }
            if (collided)
            {
                yield return new WaitForSeconds(0.5f);
            }
        }

        // Registering the traveller with edge and allowing movement
        this.currentEdge.Subscribe(this);
        this.initialised = true;
        gameObject.GetComponent<Renderer>().enabled = true;

        Debug.Log("Traveller initialised - safe travels!");
    }

    // registers another traveller with this one to be notified in case this one moves.
    private void registerForMove(WaypointMover trav)
    {
        // check that trav only gets added if it is not within the list already
        foreach (WaypointMover wp in this.CachedRejectedWaiting)
        {
            if (wp == trav)
            {
                return;
            }
        }
        // Check that there are no cycles
        List<WaypointMover> visited = new List<WaypointMover>();
        List<WaypointMover> toCheck = new List<WaypointMover>();
        toCheck.Add(this);

        while (toCheck.Count > 0)
        {
            foreach (WaypointMover wp in toCheck[0].travsBlockingThis)
            {
                if (visited.Contains(wp))
                {
                    continue;
                }
                if (wp == trav)
                {
                    this.CachedRejectedWaiting.Add(trav);
                    return;
                }
                toCheck.Add(wp);
            }
            visited.Add(toCheck[0]);
            toCheck.RemoveAt(0);
        }

        this.travsBlockedByThis.Add(trav);
        this.CachedRejectedWaiting.Add(trav);
    }

    // visually draws the path with gizmos
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
    public void chooseDestinationBuilding()
    {
        // Get list of buildings.
        // Choose random destination building.
        destinationBuilding = graph.buildings[Random.Range(0, graph.buildings.Count)];

        // Edge case where the chosen building is the same as the building the traveller spawned at.
        while (Vector3.Distance(this.transform.position, destinationBuilding.closestPointOnEdge) < distanceThreshold)
        {
            // Choose new random destination building.
            destinationBuilding = graph.buildings[Random.Range(0, graph.buildings.Count)];
        }
    }

    void Update()
    {
        // Save location at the frame beginning for calulating velocity
        this.edgeAtFrameBeginning = currentEdge;
        this.distanceAlongEdgeAtFrameBeginning = this.distanceAlongEdge;
        //Make sure the logic is only called if the Traveller was initialised
        if (initialised)
        {
            // Calculate distance covered between frames
            this.leftToMove = this.vType.MaxVelocity * Time.deltaTime; // Currently obtained from maxVelocity attribute, later should also consider the maximum velocity permitted by the edge
            Move();
        }
    }

    // prepares the traveller for next frame
    void LateUpdate()
    {

        // Make backups for debug visualisation
        foreach (WaypointMover trav in this.travsBlockedByThis)
        {
            travsBlockedByThisDEBUG.Add(trav);
        }
        foreach (WaypointMover trav in this.travsBlockingThis)
        {
            travsBlockingThisDEBUG.Add(trav);
        }
        // Renew the blocking logic
        this.travsBlockedByThis = new List<WaypointMover>();
        this.travsBlockingThis = new List<WaypointMover>();
        this.CachedRejectedWaiting = new List<WaypointMover>();

        if (initialised)
        {
            UpdatePosition();

            // Calculate velocity
            if (currentEdge == edgeAtFrameBeginning)
            {
                this.velocity = (distanceAlongEdge - distanceAlongEdgeAtFrameBeginning) / Time.deltaTime;
            }
            else
            {
                if (edgeAtFrameBeginning == null)
                {
                    this.velocity = 0;
                }
                else
                {
                    this.velocity = edgeAtFrameBeginning.Distance - distanceAlongEdgeAtFrameBeginning;
                    this.velocity += this.distanceAlongEdge;
                    this.velocity = this.velocity / Time.deltaTime;
                }

            }
        }
    }

    // updates the traveller's position along the edge
    void UpdatePosition()
    {
        // ~~~~~
        // edge_origin * (1-positionOnEdge) + edge_end * positionOnEdge
        //Vector3 newPosition = this.currentEdge.StartWaypoint.transform.position*(1-this.positionOnEdge) + this.currentEdge.EndWaypoint.transform.position*(this.positionOnEdge);

        // Move the object's transform to:
        // edge_origin + edge_diraction * distanceAlongEdge
        Vector3 newPosition =
            this.currentEdge.StartWaypoint.transform.position
            + this.currentEdge.GetDirection() * distanceAlongEdge;
        // Add offset to the new position to no be in the middle of the road
        newPosition = newPosition + this.leftLaneOffset * Vector3.Cross(Vector3.up, this.currentEdge.GetDirection());
        // Update the object's position
        transform.position = newPosition;
    }

    // updates the traveller's heading to match the edge's direction
    private void updateHeading()
    {
        Vector3 direction = this.currentEdge.GetDirection();
        Quaternion rotation = Quaternion.LookRotation(direction, Vector3.up);
        transform.rotation = rotation;
    }

    // moves the traveller along the edge. Check the trello card for specifics on how this works.
    private void Move()
    {
        // Check if Traveller has moved to the end of its edge
        float proposedMovement = this.leftToMove;
        // Check for collisions at the new location
        bool proposalAccepted = false;
        while (!proposalAccepted)
        {
            proposalAccepted = true;
            if (proposedMovement < 0)
            {
                proposedMovement = 0;
                break;
            }
            // Determine which Edge the Traveller ends up on, and how much movement it uses up in the process
            float TravelledOverEdges = 0;
            Edge terminalEdge = this.currentEdge;
            float offset = this.distanceAlongEdge;

            // Check whether proposed movement exceeds this edge
            //if(proposedMovement>(this.currentEdge.Distance*(1-this.positionOnEdge))){
            if (proposedMovement > (this.currentEdge.Distance - this.distanceAlongEdge))
            {
                offset = 0;
                // Add the current edge to the distance TravelledOverEdges
                TravelledOverEdges += (this.currentEdge.Distance - this.distanceAlongEdge);
                // Calculate how much the Traveller travels over subsequent edges
                IEnumerator<Edge> iter = this.pathEdges.GetEnumerator();
                if (!iter.MoveNext())
                {
                    // Check whether there is a next edge, if not, the Traveller has reached its destination
                    this.currentEdge.Unsubscribe(this);
                    arriveToDestination();
                }
                // Calculate distance Travelled over subsequent Edges
                while (iter.Current.Distance < proposedMovement - TravelledOverEdges)
                {
                    TravelledOverEdges += iter.Current.Distance;
                    if (!iter.MoveNext())
                    {
                        // Check whether there is a next edge, if not, the Traveller has reached its destination
                        this.currentEdge.Unsubscribe(this);
                        arriveToDestination();
                    }
                }
                terminalEdge = iter.Current;
            }
            // Determine how much movement in Edge the Traveller has left upon entering the edge
            float TravelledInEdge = proposedMovement - TravelledOverEdges;
            //Calculate position of
            float myFront = TravelledInEdge + hLen + offset;
            float myRear = TravelledInEdge - hLen + offset;

            // COLLISSION CHECK
            // Check for collision at that point in the edge with every traveller present on the edge
            foreach (WaypointMover wp in terminalEdge.TravellersOnEdge)
            {
                if (wp == this) continue;
                //Calculate position of other traveller on the edge in Real units
                float wpPOEReal = wp.distanceAlongEdge;
                //Calculate position of
                float wpFront = wpPOEReal + wp.hLen;
                float wpRear = wpPOEReal - wp.hLen;

                // Check whether this' front would end up within the vehicle
                if (myFront <= wpFront && myFront >= wpRear)
                {
                    // Collision occurred
                    // Reduce proposed movement
                    proposedMovement -= myFront - wpRear + 0.01f;

                    // register for notification
                    this.travsBlockingThis.Add(wp);
                    wp.registerForMove(this);

                    // and re-loop
                    proposalAccepted = false;
                    break;
                }

                // Check whether this' rear would end up within a vehilce
                if (myRear <= wpFront && myRear >= wpRear)
                {
                    // collision occurred
                    // Reduce proposed movement
                    proposedMovement -= myFront - wpRear + 0.0001f;

                    // register for notification
                    this.travsBlockingThis.Add(wp);
                    wp.registerForMove(this);

                    // and re-loop
                    proposalAccepted = false;
                    break;
                }
            }
            // No collision occurred, the proposed movement is accepted
        }
        // Beginning to carry out proposed movement
        this.leftToMove -= proposedMovement;
        this.totalDistanceMoved += proposedMovement;
        bool escapedEdge = false;
        // Keep switching edges until the proposed movement is insufficient to escape the edge
        while (proposedMovement > (this.currentEdge.Distance - this.distanceAlongEdge))
        {
            proposedMovement -= this.currentEdge.Distance - this.distanceAlongEdge;
            // Exit current edge
            this.currentEdge.Unsubscribe(this);
            // Enter new edge
            this.currentEdge = this.pathEdges[0];
            this.pathEdges.RemoveAt(0);
            this.currentEdge.Subscribe(this);
            //this.positionOnEdge = 0;
            this.distanceAlongEdge = 0;
            // Set the variable
            escapedEdge = true;
        }
        this.distanceAlongEdge += proposedMovement;
        // Proposed movement carried out

        // Rotate the traveller if it changed an edge
        if (escapedEdge)
        {
            updateHeading();
        }

        // Check for arriving to destination
        if (this.pathEdges.Count == 0 && this.distanceAlongEdge >= this.terminalLength)
        {
            this.currentEdge.Unsubscribe(this);
            arriveToDestination();
        }
        bool finished = false;
        while (!finished)
        {
            try
            {
                // Call Move() on all Travellers waiting to Move
                foreach (WaypointMover trav in this.travsBlockedByThis)
                {
                    trav.Move();
                }
                finished = true;
            }
            catch
            {

            }
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
        return this.vType.RateOfEmission * totalDistanceMoved;
    }

    private void OnDrawGizmos()
    {
        if (Application.isPlaying && !(path == null))
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

            if (this.travsBlockedByThisDEBUG.Count > 0)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(transform.position + Vector3.up * 3, 0.5f);
                //this.travsBlockedByThisDEBUG = new List<WaypointMover>();
            }
        }
    }
    private void OnDrawGizmosSelected()
    {
        if (Application.isPlaying && !(path == null))
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

            // Draw arrow to vehicles collided with
            Gizmos.color = Color.magenta;
            travsBlockingThisDEBUG.RemoveAll(item => item == null);
            Vector3 startPosition = this.transform.position + Vector3.up * 4;
            foreach (WaypointMover wp in travsBlockingThisDEBUG)
            {
                Vector3 endPosition = wp.transform.position + Vector3.up * 4;//- startPosition
                Gizmos.DrawLine(startPosition, endPosition);
            }
            // this.travsBlockingThisDEBUG = new List<WaypointMover>();

            // Draw the path from the agent's current position
            tracePath(Color.red, 2f, 10f);

            // Draw vehicle's collider box
            Gizmos.color = Color.blue;
            Vector3 position =
                this.currentEdge.startWaypoint.transform.position
                + this.currentEdge.GetDirection() * this.distanceAlongEdge;
            Gizmos.DrawWireCube(position, new Vector3(this.hLen * 2, this.hLen * 2, this.hLen * 2));
        }
        //debugText();
    }

    private void tracePath(Color c, float height, float thickness)
    {
        // Draw the path from the agent's current position
        Gizmos.color = c;
        Vector3 startPosition = currentEdge.startWaypoint.transform.position + currentEdge.GetDirection() * this.distanceAlongEdge + Vector3.up * height;
        Vector3 endPosition = Vector3.zero;
        if (this.pathEdges.Count > 0)
        {
            endPosition = currentEdge.endWaypoint.transform.position + Vector3.up * height - startPosition;
        }
        else
        {
            endPosition = currentEdge.GetDirection() * (this.terminalLength - this.distanceAlongEdge);
        }

        DrawArrow.ForGizmo(startPosition, endPosition, c, thickness);

        // Draw the path for intermediate segments
        if (this.pathEdges.Count > 1)
        {
            foreach (Edge edge in this.pathEdges.GetRange(0, this.pathEdges.Count - 1))
            {
                startPosition = edge.startWaypoint.transform.position + Vector3.up * height;
                endPosition = edge.endWaypoint.transform.position + Vector3.up * height - startPosition;
                DrawArrow.ForGizmo(startPosition, endPosition, c, thickness);
            }
        }
        // Draw the final segment if it exists
        if (this.pathEdges.Count > 0)
        {
            Edge e = this.pathEdges[this.pathEdges.Count - 1];
            startPosition = e.startWaypoint.transform.position + Vector3.up * height;
            endPosition = e.GetDirection() * this.terminalLength;
            DrawArrow.ForGizmo(startPosition, endPosition, c, thickness);
        }
    }
    private VehicleProperties pickRandomVehicleType()
    {
        return new VehicleProperties();
    }

    private void setVehicleModelAndMaterial()
    {
        // pick a random model and material
        GameObject model = TravellerManager.Instance.pickRandomModelAndMaterial(this.vType.Type);
        if (model != null)
        {
            MeshRenderer thisMeshRenderer = GetComponent<MeshRenderer>();
            MeshFilter thisMeshFilter = GetComponent<MeshFilter>();

            thisMeshRenderer.material = model.GetComponent<MeshRenderer>().sharedMaterial;
            thisMeshFilter.mesh = model.GetComponent<MeshFilter>().sharedMesh;

        }else{
            Debug.LogError("No model found for vehicle type: " + this.vType.Type + ". Fix this please! Worse errors could arise later.");
            DestroyImmediate(this.gameObject);
        }
    }

}
