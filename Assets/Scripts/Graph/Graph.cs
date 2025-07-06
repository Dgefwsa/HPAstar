using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Graph
{
    private Dictionary<int3, Node> _nodesMap = new();
    private Dictionary<int, List<IntBound2D>> _clusters = new();
    private Dictionary<IntBound2D, List<Node>> _nodesClustersMap = new();

    public Dictionary<int3, Node> NodesMap => _nodesMap;
    public Dictionary<int, List<IntBound2D>> Clusters => _clusters;
    public Dictionary<IntBound2D, List<Node>> NodesClustersMap => _nodesClustersMap;

    public void SetNode(int3 pos, int level)
    {
        if (_nodesMap.TryGetValue(pos, out var node))
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
    public void SetEdge(int3 start, int3 dest, float weight, int level, bool isUndirected, EdgeType edgeType, List<int3> path = null) 
    {
        if (_nodesMap.TryGetValue(start, out Node startNode))
            if (_nodesMap.TryGetValue(dest, out Node destNode))
            {
                var existingEdge = startNode.outgoingEdges.FirstOrDefault(e => e.from == startNode && e.destinationNode == destNode);
                if (existingEdge != null)
                {
                    existingEdge.weight = weight;
                    existingEdge.hierrarchyLevel = level;
                }
                else
                {
                    var edge = new Edge(startNode ,destNode)
                    {
                        weight = weight,
                        hierrarchyLevel = level,
                        edgeType = edgeType,
                        path = path
                    };
                    startNode.outgoingEdges.Add(edge);
                }
            }
            else Debug.LogWarning($"{dest.x}:{dest.y}:{dest.z} does not exist as destination node");
        else Debug.LogWarning($"{start.x}:{start.y}:{start.z} does not exist as start node");


        if (isUndirected)
        {
            path?.Reverse();
            SetEdge(dest, start, weight, level, false, edgeType, path);
        }
    }

    public bool TryGetNode(int3 pos, out Node node)
    {
        return _nodesMap.TryGetValue(pos, out node);
    }
    
}