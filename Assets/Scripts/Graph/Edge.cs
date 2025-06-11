public class Edge
{
    public readonly Node destinationNode;
    
    public int hierrarchyLevel;
    public float weight;
    public EdgeType edgeType;
    


    public Edge(Node destination)
    {
        destinationNode = destination;
    }
}

public enum EdgeType
{
    INTRA,
    INTER
}