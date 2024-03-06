using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Edge
{
    public Vector3 position;
    public Vector3 direction;
    public Waypoint startWaypoint;

    private Waypoint startWaypointLane;

    public Waypoint endWaypoint;
    private Waypoint endWaypointLane;

    public float length;

    public Waypoint StartWaypoint { get { return startWaypoint; } }
    public Waypoint EndWaypoint { get { return endWaypoint; } }
    public List<WaypointMover> TravellersOnEdge;
    public float Distance { get { return length; } }

    /// <summary>
    /// Barrier stores if there's a barrier in the path of the edge
    /// isBarricated is true if there is a barrier in the path of the edge
    /// barricadeLocation is the z position of the barrier in the path of the edge. -1 if there is no barrier
    /// </summary>
    public bool isBarricated;
    public float barrierLocation;
    public Barrier barrier;

    public bool isPedestrianOnly;

    public Sensor sensor;
    public float sensorLocation;

    private List<Sensor> sensors = new List<Sensor>();

    public void RegisterSensor(Sensor sensor)
    {
        if (sensor != null && !sensors.Contains(sensor))
        {
            sensors.Add(sensor);
        }
    }

    public Edge(Waypoint startWaypoint, Waypoint endWaypoint)
    {
        this.startWaypoint = startWaypoint;
        this.endWaypoint = endWaypoint;
        this.length = Vector3.Distance(startWaypoint.transform.position, endWaypoint.transform.position);

        this.TravellersOnEdge = new List<WaypointMover>();

        this.barrier = getBarrierInPath();
        this.isBarricated = barrier != null;
        this.barrierLocation = barrier != null ? convertToPositionAlongEdge(barrier.transform.position) : -1f;

        this.isPedestrianOnly = startWaypoint.isPedestrianOnly || endWaypoint.isPedestrianOnly;

        if (this.isBarricated)
        {
            Debug.Log("Edge between " + startWaypoint.name + " and " + endWaypoint.name + " is barricaded at " + this.barrierLocation);
        }
    }

    public void RecheckBarriers()
    {
        this.barrier = getBarrierInPath();
        this.isBarricated = barrier != null;
        this.barrierLocation = barrier != null ? convertToPositionAlongEdge(barrier.transform.position) : -1f;
        if (this.isBarricated)
        {
            Debug.Log("Edge between " + startWaypoint.name + " and " + endWaypoint.name + " is barricaded at " + this.barrierLocation);
        }
    }

    public void RecheckSensors()
    {
        this.sensor = getSensorInPath();
        this.isBarricated = sensor != null;
        this.sensorLocation = sensor != null ? convertToPositionAlongEdge(sensor.transform.position) : -1f;
        if (this.isBarricated)
        {
            Debug.Log("Edge between " + startWaypoint.name + " and " + endWaypoint.name + " is barricaded at " + this.sensorLocation);
        }
    }

    // converting between distance along edge and distance in world space
    public float RealDistanceToDeltaD(float len)
    {
        return len / this.length;
    }
    public float DeltaDToRealDistance(float len)
    {
        return len * this.length;
    }

    // adds and removes travellers from the edge
    public void Subscribe(WaypointMover trav)
    {
        this.TravellersOnEdge.Add(trav);
    }
    public void Unsubscribe(WaypointMover trav)
    {
        this.TravellersOnEdge.Remove(trav);
    }

    public void DrawGizmo()
    {
        // drawing the edge would be too much clutter
        if (isPedestrianOnly)
        {
            return;
        }

        // Draw arrow pointing in the edge's direction
        Vector3 startpoint = startWaypoint.transform.position;
        Vector3 endpoint = endWaypoint.transform.position;
        Vector3 direction = endpoint - startpoint;
        // Debug.Log("Direction of the Road Edge: " + direction);

        // Make the arrows shorter by 20%
        float shortenedMagnitude = direction.magnitude * 0.7f;
        Vector3 shortenedDirection = direction.normalized * shortenedMagnitude;

        // Calculate the middle position
        Vector3 middlePosition = startpoint + direction * 0.5f - shortenedDirection * 0.5f;

        // Shift the middle position slightly to the left
        Vector3 shiftedMiddlePosition = middlePosition + Vector3.Cross(direction, Vector3.up).normalized * 0.6f;

        // Draw the arrow with the shifted middle position and shortened direction
        DrawArrow.ForGizmo(shiftedMiddlePosition, shortenedDirection, Color.green, 1f, 30f);
    }

    public bool isPointOnEdge(Vector3 point)
    {
        Vector3 start = startWaypoint.transform.position;
        Vector3 end = endWaypoint.transform.position;

        float distanceFromStart = Vector3.Distance(point, start);
        float distanceFromEnd = Vector3.Distance(point, end);
        float edgeLength = Vector3.Distance(start, end);

        return distanceFromStart + distanceFromEnd <= edgeLength + 0.2f;
    }

    // returns the distance from the edge to a point
    public float DistanceToEdge(Vector3 position)
    {
        Vector3 closestPoint = this.GetClosestPoint(position);
        return Vector3.Distance(position, closestPoint);
    }

    // returns the direction of the edge
    public Vector3 GetDirection()
    {
        Vector3 start = startWaypoint.transform.position;
        Vector3 end = endWaypoint.transform.position;

        Vector3 edgeDirection = end - start;
        edgeDirection.Normalize();
        return edgeDirection;
    }

    // returns the closest point on the edge to a point
    public Vector3 GetClosestPoint(Vector3 point)
    {
        Vector3 start = startWaypoint.transform.position;
        Vector3 end = endWaypoint.transform.position;

        Vector3 edgeDirection = end - start;
        Vector3 pointDirection = point - start;

        float edgeLength = edgeDirection.magnitude;
        edgeDirection.Normalize();

        float dotProduct = Vector3.Dot(pointDirection, edgeDirection);

        if (dotProduct <= 0)
        {
            return start;
        }
        else if (dotProduct >= edgeLength)
        {
            return end;
        }
        else
        {
            Vector3 closestPointOnEdge = start + edgeDirection * dotProduct;
            return closestPointOnEdge;
        }
    }

    // returns the closest point on the edge to a point, as a fraction of the edge
    public float GetClosestPointAsFractionOfEdge(Vector3 point)
    {
        Vector3 start = startWaypoint.transform.position;
        Vector3 end = endWaypoint.transform.position;

        Vector3 edgeDirection = end - start;
        Vector3 pointDirection = point - start;

        float edgeLength = edgeDirection.magnitude;
        edgeDirection.Normalize();

        return Vector3.Dot(pointDirection, edgeDirection) / edgeLength;
    }

    // returns a random point on the edge
    public Vector3 GetRandomPointOnEdge()
    {
        Vector3 start = startWaypoint.transform.position;
        Vector3 end = endWaypoint.transform.position;

        Vector3 edgeDirection = end - start;
        float edgeLength = edgeDirection.magnitude;
        edgeDirection.Normalize();

        float randomDistance = Random.Range(0f, edgeLength);
        Vector3 randomPointOnEdge = start + edgeDirection * randomDistance;

        return randomPointOnEdge;
    }

    // checks if two edges are the same (i.e. have the same waypoints)
    public bool isSameEdge(Edge otherEdge)
    {
        return (this.startWaypoint == otherEdge.startWaypoint && this.endWaypoint == otherEdge.endWaypoint) ||
               (this.startWaypoint == otherEdge.endWaypoint && this.endWaypoint == otherEdge.startWaypoint);
    }

    // returns the distance along the edge of a point
    public float convertToPositionAlongEdge(Vector3 point)
    {
        Vector3 start = startWaypoint.transform.position;
        Vector3 end = endWaypoint.transform.position;

        Vector3 edgeDirection = end - start;
        Vector3 pointDirection = point - start;

        float edgeLength = edgeDirection.magnitude;
        edgeDirection.Normalize();

        float dotProduct = Vector3.Dot(pointDirection, edgeDirection);

        if (dotProduct <= 0)
        {
            return 0f;
        }
        else if (dotProduct >= edgeLength)
        {
            return 1f;
        }
        else
        {
            return dotProduct / edgeLength;
        }
    }

    public Barrier getBarrierInPath()
    {
        Barrier[] allBarriers;

        if (BarrierManager.Instance == null)
        {
            allBarriers = GameObject.FindObjectsOfType<Barrier>();
        }
        else
        {
            List<GameObject> allBarrierGameObjects;
            allBarrierGameObjects = BarrierManager.Instance.allBarriers;
            // get all the Barrier objects from the list of GameObjects
            allBarriers = new Barrier[allBarrierGameObjects.Count];
            for (int i = 0; i < allBarrierGameObjects.Count; i++)
            {
                allBarriers[i] = allBarrierGameObjects[i].GetComponent<Barrier>();
            }
        }

        // we go through the Barrier and check if the edge intersects with any of them
        foreach (Barrier barrier in allBarriers)
        {
            if (barrier.isPointInBarrier(this.GetClosestPoint(barrier.transform.position)))
            {
                Debug.Log("Barrier found at " + barrier.transform.position);
                // Calculate the angle between the barrier's forward direction and the edge direction
                Vector3 edgeDirection = this.GetDirection();
                float angle = Vector3.Angle(barrier.transform.forward, edgeDirection);

                // Apply the rotation to the barrier
                barrier.transform.rotation = Quaternion.Euler(0, angle, 0);

                return barrier;
            }
        }
        return null;
    }

    public Waypoint FindNearestWaypoint(Sensor sensor)
    {
        Waypoint[] allWaypoints = GameObject.FindObjectsOfType<Waypoint>();
        Waypoint nearestWaypoint = null;
        float minDistance = float.MaxValue;

        foreach (Waypoint waypoint in allWaypoints)
        {
            float distance = Vector3.Distance(sensor.transform.position, waypoint.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestWaypoint = waypoint;
            }
        }

        return nearestWaypoint;
    }

    public Sensor getSensorInPath()
    {
        Sensor[] allSensors;

        if (SensorManager.Instance == null)
        {
            allSensors = GameObject.FindObjectsOfType<Sensor>();
        }
        else
        {
            List<GameObject> allSensorGameObjects;
            allSensorGameObjects = SensorManager.Instance.allSensors;
            // get all the Sensor objects from the list of GameObjects
            allSensors = new Sensor[allSensorGameObjects.Count];
            for (int i = 0; i < allSensorGameObjects.Count; i++)
            {
                allSensors[i] = allSensorGameObjects[i].GetComponent<Sensor>();
            }
        }

        foreach (Sensor sensor in allSensors)
        {
            if (sensor.isPointInSensor(this.GetClosestPoint(sensor.transform.position)))
            {
                Debug.Log("Sensor found at " + sensor.transform.position);
                // Calculate the angle between the sensor's forward direction and the edge direction
                Vector3 edgeDirection = this.GetDirection();
                float angle = Vector3.Angle(sensor.transform.forward, edgeDirection);

                // Apply the rotation to the sensor
                sensor.transform.rotation = Quaternion.Euler(0, angle, 0);

                // Find the nearest waypoint to the sensor
                Waypoint nearestWaypoint = FindNearestWaypoint(sensor);
                if (nearestWaypoint != null)
                {
                    Debug.Log("Nearest waypoint to sensor is at " + nearestWaypoint.transform.position);
                }
                else
                {
                    Debug.Log("No waypoints found.");
                }

                return sensor;
            }
        }

        return null; // return null if no sensor is found in the path
    }

    public Waypoint getClosestWaypoint(Vector3 point)
    {
        if (!isPointOnEdge(point))
        {
            Debug.Log("Position is not on edge");
            return null;
        }

        float distanceToStart = Vector3.Distance(point, startWaypoint.transform.position);
        float distanceToEnd = Vector3.Distance(point, endWaypoint.transform.position);

        if (distanceToStart < distanceToEnd)
        {
            return startWaypoint;
        }
        else
        {
            return endWaypoint;
        }
    }

    /// <summary>
    /// This method returns the closest waypoint that is not blocked by a barrier, given a point.
    /// If both waypoints are blocked, it returns null.
    /// 
    /// Beyond that, if we pass in a ModeOfTransport, it will consider the modes of the barriers as well
    /// when collecting the accessible waypoint.
    /// </summary>
    /// <param name="point">the point we want to check</param>
    /// <param name="mode">The mode of transport we're checking accessibility for</param>
    /// <returns></returns>
    public Waypoint getClosestAccesibleWaypoint(Vector3 point, ModeOfTransport mode = ModeOfTransport.Car)
    {
        float distanceToStart = Vector3.Distance(point, startWaypoint.transform.position);
        float distanceToEnd = Vector3.Distance(point, endWaypoint.transform.position);

        // Debug.LogWarning($"Distance to Start Waypoint: {distanceToStart}");
        // Debug.LogWarning($"Distance to End Waypoint: {distanceToEnd}");

        if (distanceToStart < distanceToEnd)
        {
            // If there is no barrier between the point and the start waypoint, return start waypoint
            if (!isEdgeTraversableThroughBarrier(point, startWaypoint.transform.position))
            {
                return startWaypoint;
            }
            // Otherwise, check if the end waypoint is accessible
            else if (!isEdgeTraversableThroughBarrier(point, endWaypoint.transform.position))
            {
                return endWaypoint;
            }
        }
        else
        {
            // If there is no barrier between the point and the end waypoint, return end waypoint
            if (!isEdgeTraversableThroughBarrier(point, endWaypoint.transform.position))
            {
                return endWaypoint;
            }
            // Otherwise, check if the start waypoint is accessible
            else if (!isEdgeTraversableThroughBarrier(point, startWaypoint.transform.position))
            {
                return startWaypoint;
            }
        }

        // If both waypoints are blocked by a barrier, return null
        // Debug.LogWarning("No accessible waypoint found, returning null");
        return null;
    }

    /// <summary>
    /// This checks if the edge is traverasable by the modeoftransport (according to the barrier)
    /// </summary>
    /// <param name="start"></param>
    /// <param name="destination"></param>
    /// <param name="mode"></param>
    /// <returns></returns>
    public bool isEdgeTraversableThroughBarrier(Vector3 start, Vector3 destination, ModeOfTransport mode = ModeOfTransport.Car)
    {
        // Check if barricade is active
        if (!isBarricated)
        {
            return false;
        }

        // Convert positions to distances along the edge
        float startDistance = convertToPositionAlongEdge(start);
        float destinationDistance = convertToPositionAlongEdge(destination);
        float barrierDistance = convertToPositionAlongEdge(barrier.transform.position);

        // Check if the barrier is between start and destination
        // Assuming lower distance value is closer to the starting point of the edge
        return barrierDistance > System.Math.Min(startDistance, destinationDistance) &&
            barrierDistance < System.Math.Max(startDistance, destinationDistance);
    }
}