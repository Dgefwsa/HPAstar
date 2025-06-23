using System.Collections.Generic;

public class Node
{
    public readonly int3 pos;
    public int hierrarchyLevel; //If check graph level is n, we use only nodes with level >= n

    public List<Edge> outgoingEdges;
    public List<int> ids; //if count == 0 - node is  belong to graph, if count > 0 - node is added for patfingding

    public Node(int3 pos)
    { 
        this.pos = pos;
        outgoingEdges = new List<Edge>();
        ids = new List<int>();
    }
}