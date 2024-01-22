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
