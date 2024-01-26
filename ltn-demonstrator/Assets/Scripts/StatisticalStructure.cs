using System.Collections.Generic;
using UnityEngine;


public class PathData {
    public int ID;
    public List<Edge> path;
    public float startTime;
    public float endTime;
    public bool routeChange;
    public ModeOfTransport.Mode travellerType;
}

public class SerialisableEdge {
    public float length;
    public bool isBarricaded;
    //whichever other ones are needed
}

public class SerialisableWaypoint {
    public float bruh;
    //whichever other ones are needed
}