using System.Collections.Generic;

public class Node
{
    public readonly int3 pos;
    public int hierrarchyLevel; //If check graph level is n, we use only nodes with level >= n

    public List<Edge> outgoingEdges;

    public Node(int3 pos)
    { 
        this.pos = pos;   
        outgoingEdges = new List<Edge>();
    }
}