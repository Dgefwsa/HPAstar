using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class GraphVisualization : MonoBehaviour
{
    public Graph Graph;
    public List<int3> Path = new List<int3>();
    public int HierarchyLevel;

    [Header("Gizmos settings")]
    public float radius;
    public float radiusCube;
    public bool ShowNodes = true;
    public bool ShowEdges;
    
    private void OnDrawGizmos()
    {
        if (Graph == null) return;
        foreach (KeyValuePair<int3,Node> pair in Graph.NodesMap)
        {
            if (pair.Value.hierrarchyLevel < HierarchyLevel) continue;

            Gizmos.color = Color.blue;
            if (Path.Contains(pair.Key)) Gizmos.color = Color.red;
            if (ShowNodes)
                Gizmos.DrawSphere(new Vector3(pair.Value.pos.x, pair.Value.pos.y, pair.Value.pos.z), radius);
            foreach (Edge edge in pair.Value.outgoingEdges)
            {
                if (edge.hierrarchyLevel < HierarchyLevel) continue;
                Gizmos.color = Color.red;
                var from = new Vector3(edge.from.pos.x, edge.from.pos.y, edge.from.pos.z);
                var to = new Vector3(edge.destinationNode.pos.x, edge.destinationNode.pos.y, edge.destinationNode.pos.z);
                if (ShowEdges)
                    Gizmos.DrawLine(from, to);
            }
            
        }
    }
}