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
    public float movementUpperBound // Controls the maximum distance the traveller moves in a frame
    {
        get; private set;
    }
    public float movementLowerBound // Controls the minimum distance the traveller moves in a frame
    {
        get;
        private set;
    }
    public float brakingDistance    // Calculated as a distance the vehicle needs to travel, before coming to a stop if deaccelerating at maximum rate
    {
        get;
        private set;
    }
    private float distanceAlongEdge;    // Controls how far along current edge the traveller is
    private float terminalLength;       // Controls how far along the last edge in pathEdges / currentEdge if the previous is empty the terminal destination is
    private Edge currentEdge;           // Controls which edge the traveller currently occupies
    private List<Edge> pathEdges;       // List of edges the traveller needs to travel

    // Movement attributes - velocity & velocity calculation
    public float velocity
    {
        get; private set;
    }
    private Edge edgeAtFrameBeginning;
    private float distanceAlongEdgeAtFrameBeginning;

    // Attributes serving collissions
    private List<WaypointMover> travsBlockedByThis;     // List of travellers prevented from moving by this traveller
    private List<WaypointMover> travsBlockingThis;      // List of travellers preventing this traveller from moving
    private List<WaypointMover> CachedRejectedWaiting;  // Travellers that should be declines from addition to travsBlockedByThis
    private float length = 0;                           // The length of the edge the traveller occupies
    private float hLen = 0;                             // half-length

    // Debug attributes - collissions
    private List<WaypointMover> travsBlockedByThisDEBUG;
    private List<WaypointMover> travsBlockingThisDEBUG;
    private List<WaypointMover> travsSlowingThisDEBUG;

    // Attributes from the WaypointMover class
    private float distanceThreshold = 0.001f;
    [SerializeField] private float leftLaneOffset = 1f;

    // Attributes for pathfinding
    private WaypointPath path;           // Instance of the pathfinding class
    private Graph graph;                 // Instance of the graph class
    private Building destinationBuilding;
    private Building originBuilding;
    [SerializeField] private BuildingType destinationBuildingType;

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
        this.travsSlowingThisDEBUG = new List<WaypointMover>();

        // pick a random model and material
        this.vType = pickRandomVehicleType();
        if (!setVehicleModelAndMaterial())
        {
            return;
        }

        var r = GetComponent<Collider>();
        if (r != null)
        {
            var bounds = r.bounds;
            this.length = Mathf.Max(Mathf.Max(bounds.size.x, bounds.size.y), bounds.size.z);
            this.hLen = length / 2;
        }

        // Start generating path to be taken
        this.graph = GameObject.Find("Graph").GetComponent<Graph>();

        this.destinationBuilding = chooseDestinationBuilding();
        if (this.destinationBuilding == null)
        {
            Debug.LogWarning("No destination building found. Destroying object.");
            Destroy(this.gameObject);
            return;
        }

        // Initialize the path with the starting waypoint
        path = new WaypointPath(this.transform.position, destinationBuilding.closestPointOnRoadEdge, this);

        if (path.path == null)
        {
            Edge endEdge = destinationBuilding.closestRoadEdge;
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
            // Debug.Log("Path from: " + old_wp.name + "  to: " + wp.name + "\nEdge: ");
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
        Edge originEdge = graph.getClosetRoadEdge(this.transform.position);
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
        Edge terminalEdge = destinationBuilding.closestRoadEdge;
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
        if (!originEdge.isSameEdge(terminalEdge))
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
        Vector3 terminal = destinationBuilding.closestPointOnRoadEdge;
        //this.terminalLength = terminalEdge.GetClosestPointAsFractionOfEdge(terminal);
        this.terminalLength = Vector3.Distance(
            terminalEdge.startWaypoint.transform.position,
            terminalEdge.GetClosestPoint(terminal)
            );
        // If the destination is along the same edge, but in opposite direction
        // reverse the current direction of travel, and regenerate the positions
        if (originEdge.isSameEdge(terminalEdge))
        {
            // Get the closest points on the edge to the traveller's position and the destination
            Vector3 travellerpos = this.transform.position;
            Vector3 destination = destinationBuilding.closestPointOnRoadEdge;

            // direction from traveller to destination
            Vector3 direction = destination - travellerpos;

            currentEdge = chooseEdgeInDirection(direction, currentEdge);

            this.terminalLength = Vector3.Distance(
                currentEdge.startWaypoint.transform.position,
                destination
                );
            this.distanceAlongEdge = Vector3.Distance(
                currentEdge.startWaypoint.transform.position,
                this.transform.position
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

    public void setOriginBuilding(Building building)
    {
        this.originBuilding = building;
    }

    Edge chooseEdgeInDirection(Vector3 direction, Edge currentEdge)
    {
        // Normalize the direction vector for comparison
        direction.Normalize();

        // Get the direction vectors for both possible travel directions on the current edge
        Vector3 edgeForwardDirection = (currentEdge.endWaypoint.transform.position - currentEdge.startWaypoint.transform.position).normalized;
        Vector3 edgeBackwardDirection = -edgeForwardDirection;

        // Calculate the dot product to determine alignment with the edge directions
        float forwardDot = Vector3.Dot(direction, edgeForwardDirection);
        float backwardDot = Vector3.Dot(direction, edgeBackwardDirection);

        // Choose the edge direction that aligns more closely with the traveller's desired direction
        if (forwardDot > backwardDot)
        {
            // The traveller's direction aligns more with the forward direction of the edge
            return currentEdge;
        }
        else
        {
            // The traveller's direction aligns more with the backward direction, flip the edge
            Edge flippedEdge = graph.getEdge(currentEdge.endWaypoint, currentEdge.startWaypoint);
            if (flippedEdge != null)
            {
                return flippedEdge;
            }
            else
            {
                Debug.LogError("Flipped edge does not exist.");
                return null; // or handle the error appropriately
            }
        }
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
    public Building chooseDestinationBuilding()
    {
        // Choose random destination building type.
        destinationBuildingType = BuildingProperties.getRandomWeightedDestinationType();

        if (graph.buildingsByType[destinationBuildingType].Count == 0) {
            Debug.LogWarning("No buildings of type " + destinationBuildingType + " exist in the simulation. Cannot choose building.");
            return null;
        }


        // Select a random building by type. If the selected building is the same as the origin building,
        // choose a new building.
        Building building = graph.getRandomBuildingByType(destinationBuildingType);
        while (building == originBuilding)
        {
            Debug.LogWarning("Spawned traveller with same origin and destination building! Choosing new building.");

            if (graph.buildingsByType[destinationBuildingType].Count == 1) {
                Debug.LogWarning("Fewer than 2 buildings of type " + destinationBuildingType + " exist in the simulation. Cannot choose new building.");
                return null;
            }

            building = graph.getRandomBuildingByType(destinationBuildingType);
        }

        return building;
    }

    void Update()
    {
        // Save location at the frame beginning for calulating velocity
        this.edgeAtFrameBeginning = currentEdge;
        this.distanceAlongEdgeAtFrameBeginning = this.distanceAlongEdge;
        //Make sure the logic is only called if the Traveller was initialised
        if (initialised)
        {
            // Calculate minimum and maximum velocity
            float maxV = Mathf.Min(this.velocity + Time.deltaTime * vType.Acceleration, this.vType.MaxVelocity);
            float minV = Mathf.Max(this.velocity - Time.deltaTime * vType.Deacceleration, 0);

            // Calculate maximum movement
            this.movementUpperBound = Time.deltaTime * velocity;
            this.movementUpperBound += Time.deltaTime * (maxV - velocity) * 0.5f;

            // Calculate minimum movement
            this.movementLowerBound = Time.deltaTime * velocity;
            this.movementLowerBound -= Time.deltaTime * (velocity - minV) * 0.5f;

            // Calculate braking distance based on current velocity
            this.brakingDistance = Mathf.Pow(velocity, 2) / (2 * vType.Deacceleration);
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
        //Debug.Log("distanceAlongEdge: " + distanceAlongEdge);
        //Debug.Log("this.currentEdge.StartWaypoint.transform.position" + this.currentEdge.StartWaypoint.transform.position);
        //Debug.Log("this.currentEdge.GetDirection()" + this.currentEdge.GetDirection());
        Vector3 newPosition =
            this.currentEdge.StartWaypoint.transform.position
            + this.currentEdge.GetDirection() * distanceAlongEdge;
        //Debug.Log("distanceAlongEdge: " + distanceAlongEdge);
        // Add offset to the new position to no be in the middle of the road
        newPosition = newPosition + this.leftLaneOffset * Vector3.Cross(Vector3.up, this.currentEdge.GetDirection());
        //Debug.Log("distanceAlongEdge: " + distanceAlongEdge);
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
        float proposedMovement = this.movementUpperBound;
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
            Edge superTerminalEdge = null;
            if (pathEdges.Count > 0)
            {
                superTerminalEdge = pathEdges[0];
            }
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
                    goto notification;
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
                iter.MoveNext();
                superTerminalEdge = iter.Current;
            }
            // Determine how much movement in Edge the Traveller has left upon entering the edge
            float TravelledInEdge = proposedMovement - TravelledOverEdges;

            // COLLISSION CHECK
            //Calculate position of
            float myFront = TravelledInEdge + hLen + offset;
            float myRear = TravelledInEdge - hLen + offset;
            float myBrDis = TravelledInEdge + hLen + brakingDistance + offset;

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
                    proposedMovement -= myFront - wpRear + 0.01f;

                    // register for notification
                    this.travsBlockingThis.Add(wp);
                    wp.registerForMove(this);

                    // and re-loop
                    proposalAccepted = false;
                    break;
                }

                // Check whether there is a vehicle within braking path
                if (wpRear <= myBrDis && wpRear >= myFront && proposedMovement > movementLowerBound)
                {
                    // Reduce proposed movement to maintain separation
                    // Note that a car may not choose to reduce its movement below lowerBound
                    float intersection = myBrDis - wpRear;
                    proposedMovement = Mathf.Max(this.movementLowerBound, proposedMovement - intersection);
                    proposedMovement -= 0.01f;

                    // Register for notification
                    this.travsSlowingThisDEBUG.Add(wp);
                    wp.registerForMove(this);

                    // and re-loop
                    proposalAccepted = false;
                    break;
                }
            }
            // Do the same for super-terminal edge
            if (superTerminalEdge != null)
                foreach (WaypointMover wp in superTerminalEdge.TravellersOnEdge)
                {
                    if (wp == this) continue;
                    //Calculate position of other traveller on the edge in Real units
                    float wpPOEReal = wp.distanceAlongEdge + terminalEdge.Distance;
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
                        proposedMovement -= myFront - wpRear + 0.01f;

                        // register for notification
                        this.travsBlockingThis.Add(wp);
                        wp.registerForMove(this);

                        // and re-loop
                        proposalAccepted = false;
                        break;
                    }

                    // Check whether there is a vehicle within braking path
                    if (wpRear <= myBrDis && wpRear >= myFront && proposedMovement > movementLowerBound)
                    {
                        // Reduce proposed movement to maintain separation
                        // Note that a car may not choose to reduce its movement below lowerBound
                        float intersection = myBrDis - wpRear;
                        proposedMovement = Mathf.Max(this.movementLowerBound, proposedMovement - intersection);
                        proposedMovement -= 0.01f;

                        // Register for notification
                        this.travsSlowingThisDEBUG.Add(wp);
                        wp.registerForMove(this);

                        // and re-loop
                        proposalAccepted = false;
                        break;
                    }
                }

            // No collision occurred, the proposed movement is accepted
        }
        // Beginning to carry out proposed movement
        this.movementUpperBound -= proposedMovement;
        this.movementLowerBound -= proposedMovement;
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
        // Notify all traveller waiting for this one
        notification:
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

            // Draw line to vehicles collided with
            Gizmos.color = Color.magenta;
            travsBlockingThisDEBUG.RemoveAll(item => item == null);
            Vector3 startPosition = this.transform.position + Vector3.up * 4;
            foreach (WaypointMover wp in travsBlockingThisDEBUG)
            {
                Vector3 endPosition = wp.transform.position + Vector3.up * 4;//- startPosition
                Gizmos.DrawLine(startPosition, endPosition);
            }
            //this.travsBlockingThisDEBUG = new List<WaypointMover>();

            // Draw line to vehicles slowing this one
            Gizmos.color = Color.green;
            travsSlowingThisDEBUG.RemoveAll(item => item == null);
            foreach (WaypointMover wp in travsSlowingThisDEBUG)
            {
                Vector3 endPosition = wp.transform.position + Vector3.up * 4;//- startPosition
                Gizmos.DrawLine(startPosition, endPosition);
            }

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

    private bool setVehicleModelAndMaterial()
    {
        // pick a random model and material
        GameObject model = TravellerManager.Instance.pickRandomModelAndMaterial(this.vType.Type);
        if (model != null)
        {
            MeshRenderer thisMeshRenderer = GetComponent<MeshRenderer>();
            MeshFilter thisMeshFilter = GetComponent<MeshFilter>();

            thisMeshRenderer.material = model.GetComponent<MeshRenderer>().sharedMaterial;
            thisMeshFilter.mesh = model.GetComponent<MeshFilter>().sharedMesh;

        }
        else
        {
            Debug.LogError("No model found for vehicle type: " + this.vType.Type + ". Fix this please! Worse errors could arise later.");
            DestroyImmediate(this.gameObject);
            return false;
        }
        return true;
    }

}
