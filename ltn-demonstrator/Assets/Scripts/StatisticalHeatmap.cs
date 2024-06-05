using System;
using System.Collections.Generic;
using System.Drawing;


public class HeatmapEdge
{
    public int ID;
    public int start;
    public int end;
    public int weight;

    public HeatmapEdge(SerialisableEdge edge)
    {
        this.ID = edge.ID;
        this.start = edge.startWaypoint.ID;
        this.end = edge.endWaypoint.ID;
        this.weight = 0;
    }
}

public class HeatmapNode
{
    public int ID;
    public PointF location;

    public HeatmapNode(SerialisableWaypoint waypoint)
    {
        this.ID = waypoint.ID;
        this.location = new PointF(waypoint.x, waypoint.y);
    }

    public class Heatmap
    {
        private float scale;
        public List<HeatmapEdge> edges;
        public List<HeatmapNode> nodes;
        public List<PathData> paths;

        public Heatmap(float scale)
        {
            this.scale = scale;
            this.edges = new List<HeatmapEdge>();
            this.nodes = new List<HeatmapNode>();
            this.paths = new List<PathData>();
        }


        public void convertNodesToHeatmap(List<SerialisableWaypoint> nodes)
        {
            //converts the nodes to a heatmap
            for (int node = 0; node < nodes.Count; node++)
            {
                //add the node to the heatmap
                this.nodes.Add(new HeatmapNode(nodes[node]));
            }

        }

        public void convertEdgesToHeatmap(List<SerialisableEdge> edges)
        {
            //converts the edges to a heatmap
            for (int edge = 0; edge < edges.Count; edge++)
            {
                //add the edge to the heatmap
                this.edges.Add(new HeatmapEdge(edges[edge]));
            }
        }

        public void getEdgeWeights()
        {
            //gets the weights of the edges
            for (int path = 0; path < paths.Count; path++)
            {
                //for each edge in the path
                for (int e = 0; e < paths[path].serialisablePath.Count; e++)
                {
                    //find correlatory edge, search through heatmap edges
                    for (int edge = 0; edge < edges.Count; edge++)
                    {
                        //check if ID is same
                        if (edges[edge].ID == paths[path].serialisablePath[e].ID)
                        {
                            //if it is, increment the weight
                            edges[edge].weight++;
                        }
                    }
                }
            }
        }


        public string MakeHeatmap(List<SerialisableWaypoint> serialisableNodes, List<SerialisableEdge> serialisableEdges, List<PathData> serialisablePaths)
        {
            //makes the heatmap
            convertNodesToHeatmap(serialisableNodes);
            convertEdgesToHeatmap(serialisableEdges);
            getEdgeWeights();

            return "string to file location of heatmap image";
        }
    }
}



