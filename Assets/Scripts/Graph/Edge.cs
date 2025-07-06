using System.Collections.Generic;

public class Edge
{
    public readonly Node from;
    public readonly Node destinationNode;
    
    public int hierrarchyLevel;
    public float weight;
    public EdgeType edgeType;

    public List<int3> path;

    public Edge(Node from, Node destination)
    {
        this.from = from;
        destinationNode = destination;
        path = new List<int3>();
    }

    public List<int3> GetPath()
    {
        if (weight == 1) return new List<int3> {from.pos, destinationNode.pos };
        return path;
    }
}

public enum EdgeType
{
    INTRA,
    INTER
}