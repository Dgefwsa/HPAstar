using System.Collections.Generic;
using UnityEngine.Profiling;

public class AstarPathfinderManager
{
    private Graph Graph { get; }
    private int MaxHierarchyLevel = 1;
    private readonly IterativeDeepeningAstar _pathfinder;

    public AstarPathfinderManager(Graph graph)
    {
        Graph = graph;
        _pathfinder = new IterativeDeepeningAstar();
    }

    public List<int3> FindPath(int3 from, int3 to)
    {
        Profiler.BeginSample("FindPath");
        if (!Graph.TryGetNode(from, out Node start) || !Graph.TryGetNode(to, out Node goal)) return null;
        
        var oldStartLevel = start.hierrarchyLevel;
        var oldGoalLevel = goal.hierrarchyLevel;
        start.hierrarchyLevel = MaxHierarchyLevel;
        goal.hierrarchyLevel = MaxHierarchyLevel;
        InsertNode(start);
        InsertNode(goal);
        var path = _pathfinder.FindPath(Graph, from, to, MaxHierarchyLevel);
        Profiler.EndSample();
        return path;
    }

    public void InsertNode(Node node)
    {
        Profiler.BeginSample("Insert node");
        IntBound2D currentNodeCluster = null;
        foreach (var cluster in Graph.Clusters[MaxHierarchyLevel])
        {
            if (cluster.InBounds(node.pos))
            {
                currentNodeCluster = cluster;
                break;
            }
        }
        if (currentNodeCluster == null) return;
        foreach (var nodeA in Graph.NodesClustersMap[currentNodeCluster])
        {
            if (nodeA.hierrarchyLevel < MaxHierarchyLevel) continue;
            var path = _pathfinder.FindPath(Graph, node.pos, nodeA.pos, MaxHierarchyLevel - 1);
            if (path == null) continue;

            Graph.SetEdge(node.pos, nodeA.pos, path.Count - 1, MaxHierarchyLevel, true, EdgeType.INTRA, path);
        }
        Profiler.EndSample();
    }

    public void GetNextStepOnPath(int agentId, int3 end, int3 int3)
    {
        
    }

}

public struct PathRequest
{
    public int3 start;
    public int3 end;
    public int HierarchyLevel;
}