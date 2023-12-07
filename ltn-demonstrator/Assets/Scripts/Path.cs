using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Path : MonoBehaviour
{
    private float destinationDistance;
    public List<Edge> path;
    public float startTick;
    public float endTick;
    public Building startBuilding;
    public Building terminalBuilding;
    public Traveller traveller;

    public float getDestinationDistance()
    {
        return destinationDistance;
    }

    public void setDestinationDistance(float value)
    {
        destinationDistance = value;
    }

    public void GeneratePath(Building startBuilding, Building terminalBuilding, Traveller traveller)
    {
        Edge startEdge = startBuilding.getEdgeLocation();
        Edge endEdge = terminalBuilding.getEdgeLocation();
        // these two lines above use getEdgeLocation(), this might be just edgeLocation(), change it if it is
        List<Edge> generatedPath = new List<Edge> { startEdge, endEdge };
        self.path = generatedPath;
        self.startTick = null; // replace null with current unity tick
        self.startBuilding = startBuilding;
        self.terminalBuilding = terminalBuilding;
        self.traveller = traveller;
        
        return self;
        
        // This code is supposed to create pointers to start and end Edges,
        // then put them in a list of pointers to edges
        // then set the sel.path to that path
    }

    public void endPath()
    {
        self.endTick = null; // replace null with current unity tick
    }
}
