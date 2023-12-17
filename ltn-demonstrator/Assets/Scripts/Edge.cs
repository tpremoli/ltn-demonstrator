using UnityEngine;

[System.Serializable]
public class Edge
{
    [SerializeField]
    private Waypoint startWaypoint;

    [SerializeField]
    private Waypoint endWaypoint;

    [SerializeField]
    private float distance;

    public Waypoint StartWaypoint { get { return startWaypoint; } }
    public Waypoint EndWaypoint { get { return endWaypoint; } }
    public float Distance { get { return distance; } }

    public Edge(Waypoint startWaypoint, Waypoint endWaypoint, float distance)
    {
        this.startWaypoint = startWaypoint;
        this.endWaypoint = endWaypoint;
        this.distance = distance;
    }
}