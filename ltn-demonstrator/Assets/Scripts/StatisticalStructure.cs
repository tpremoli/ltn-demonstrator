using System.Collections.Generic;
using UnityEngine;


public class PathData {
    public int ID;
    public List<Edge> path;

    public List<SerialisableEdge> serialisablePath;
    public float startTime;
    public float endTime;
    public bool routeChange;
    public VehicleProperties vType;
}

public class SerialisableEdge {
    public int ID;
    public float length;
    public bool isBarricaded;
    public SerialisableWaypoint startWaypoint;
    public SerialisableWaypoint endWaypoint;
    public float weightEmissions;
    public float weightUsage;
    public GameObject EdgeObject { get; set; } 
    //whichever other ones are needed
    

    public SerialisableEdge(Edge edge) {
        ID = edge.ID;
        length = edge.length;
        isBarricaded = edge.isBarricated;
    }

    public SerialisableEdge(int id, SerialisableWaypoint start, SerialisableWaypoint end, float length, float weight) {
        this.ID = id;
        this.length = length;
        this.startWaypoint = start;
        this.endWaypoint = end;

        this.weightEmissions = weight;
    }

    public void IncrementEdgeWeight(float weightIncrement) {
        weightEmissions += weightIncrement;
    }
}


public class SerialisableWaypoint {
    public int ID;
    public float x;
    public float y;
    public float z;
    public GameObject WaypointObject { get; set; }

    //whichever other ones are needed

    public SerialisableWaypoint(Waypoint waypoint) {
        this.ID = waypoint.ID;
        x = waypoint.transform.position.x;
        y = waypoint.transform.position.y;
        z = waypoint.transform.position.z;
    }

    //second constructor
    public SerialisableWaypoint(int id, float x, float y, float z) {
        this.ID = id;
        this.x = x;
        this.y = y;
        this.z = z;
    }

}

public class Cluster {
    public List<SerialisableWaypoint> Waypoints;
    public SerialisableWaypoint centroid;
    public List<SerialisableEdge> edges = new List<SerialisableEdge>();

    public Cluster() {
        Waypoints = new List<SerialisableWaypoint>();
    }

    public void AddWaypoint(SerialisableWaypoint waypoint) {
        Waypoints.Add(waypoint);
    }
}


