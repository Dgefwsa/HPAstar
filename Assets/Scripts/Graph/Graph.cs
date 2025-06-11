using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Graph
{
    private Dictionary<int3, Node> _nodesMap;

    public Graph()
    {
        _nodesMap = new Dictionary<int3, Node>();
    }

    public void SetNode(int3 pos, int level)
    {
        if (_nodesMap.TryGetValue(pos, out Node node))
        {
            node.hierrarchyLevel = level;
            return;
        }
        
        var newNode = new Node(pos)
        {
            hierrarchyLevel = level
        };
        _nodesMap.Add(pos, newNode);
    }

    public void SetEdge(int3 start, int3 dest, float weight, int level, bool isUndirected) 
    {
        if (_nodesMap.TryGetValue(start, out Node startNode))
            if (_nodesMap.TryGetValue(dest, out Node destNode))
            {
                var existingEdge = startNode.outgoingEdges.FirstOrDefault(e => e.destinationNode == destNode);
                if (existingEdge != null)
                {
                    existingEdge.weight = weight;
                    existingEdge.hierrarchyLevel = level;
                }
                else
                {
                    var edge = new Edge(destNode)
                    {
                        weight = weight,
                        hierrarchyLevel = level
                    };
                    startNode.outgoingEdges.Add(edge);
                }
            }
            else Debug.LogWarning($"{dest.x}:{dest.y}:{dest.z} does not exist as destination node");
        else Debug.LogWarning($"{start.x}:{start.y}:{start.z} does not exist as start node");
        
        if (isUndirected) SetEdge(dest, start, weight, level, false);
    }
    
}