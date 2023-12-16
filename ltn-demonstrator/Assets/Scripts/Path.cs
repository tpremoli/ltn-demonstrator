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

    public Path(Building startBuilding, Building terminalBuilding, Traveller traveller){
        // These just get the edges that the buildings are on, however, the offsets should also be taken into account.
        // Buildings have a .getEdgeLocation() method that returns the offset along the edge that the building is on.
        // However this is not currently implemented, as it hasn't been implemented in the edge class yet.
        Edge startEdge = startBuilding.edge;
        Edge endEdge = terminalBuilding.edge;

        List<Edge> generatedPath = new List<Edge> { startEdge, endEdge };

        this.path = generatedPath;
        this.startTick = 0; // replace null with current unity tick
        this.startBuilding = startBuilding;
        this.terminalBuilding = terminalBuilding;
        this.traveller = traveller;

        // Subscribe the traveler to the starting edge
        startEdge.subscribe(traveller);
        // Pass the generated path to the traveler
        // traveller.spawnAtStartingPoint(this);
    }

    public void endPath()
    {
        this.endTick = 0; // replace null with current unity tick
    }
}
