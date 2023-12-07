public class pathFunctionality
{
    private destinationDistance float;
    public path &edge[];
    public startTick float;
    public endTick float;
    public terminalBuilding &Building;
    public startBuilding &Building;
    public traveller Traveller;

    public DestinationDistance float
    {
        get { return destinationDistance; }
        private set { destinationDistance = value; }
    }

    //public float DestinationDistance { get; set; }
    // This line might be a better getter / setter, not sure 

    public void GeneratePath(Building startBuilding, Building terminalBuilding, Traveller traveller)
    {
        Edge startEdge = &startBuilding.getEdgeLocation();
        Edge endEdge = &terminalBuilding.getEdgeLocation();
        List<&Edge> generatedPath = new List<&Edge> { startEdge, endEdge };
        self.path = generatedPath
        // This code is supposed to create pointers to start and end Edges,
        // then put them in a list of pointers to edges
        // then set the sel.path to that path
    }

    public void endPath
    {
        endTick = null; // replace null with current tick
    }
}
