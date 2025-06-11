using System.Collections.Generic;

public class Node
{
    public readonly int3 pos;
    public int hierrarchyLevel;

    public List<Edge> outgoingEdges;

    public Node(int3 pos)
    { 
        this.pos = pos;   
        outgoingEdges = new List<Edge>();
    }
}