using System.Collections.Generic;
using UnityEngine;


public class PathData {
    public int ID;
    public List<Edge> path;

    public List<SerialisableEdge> serialisablePath;
    public float startTime;
    public float endTime;
    public bool routeChange;
    public ModeOfTransport travellerType;
}

public class SerialisableEdge {
    public int ID;
    public float length;
    public bool isBarricaded;
    public SerialisableWaypoint startWaypoint;
    public SerialisableWaypoint endWaypoint;
    //whichever other ones are needed

    public SerialisableEdge(Edge edge) {
        ID = edge.ID;
        length = edge.length;
        isBarricaded = edge.isBarricated;
    }
}

public class SerialisableWaypoint {
    public int ID;
    public float x;
    public float y;
    public float z;
    //whichever other ones are needed

    public SerialisableWaypoint(Waypoint waypoint) {
        this.ID = waypoint.ID;
        x = waypoint.transform.position.x;
        y = waypoint.transform.position.y;
        z = waypoint.transform.position.z;
    }
}