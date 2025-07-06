using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Profiling;

public class IterativeDeepeningAstar : IPathfinder
{
    private const float FOUND = -1;
    private float weight = 0.5f;

    public List<int3> FindPath(Graph graph, int3 startPos, int3 endPos, int level)
    {
        if (!graph.TryGetNode(startPos, out var start) || !graph.TryGetNode(endPos, out var goal))
            return null;
        
        if (start.hierrarchyLevel < level || goal.hierrarchyLevel < level)
            return null;
        
        Profiler.BeginSample("Iterative Deepening A*");
        float initialThreshold = Heuristic(startPos, endPos);
        float threshold = initialThreshold;
        var path = new List<Node>((int)threshold) { start };
        var pathEdges = new List<Edge>();
        var visited = new HashSet<Node>((int)threshold);
        
        while (true)
        {
            float temp = Search(start, goal, 0, threshold, path, pathEdges, visited, level);

            if (temp == FOUND)
            {
                var result = new HashSet<int3>();
                foreach (var edge in pathEdges)
                {
                    foreach (int3 pos in edge.GetPath())
                    {
                        result.Add(pos);
                    }
                }
                Profiler.EndSample();
                return result.ToList();
            }

            if (temp == float.PositiveInfinity)
            {
                Profiler.EndSample();
                return null;
            }
            
            threshold = temp;
        }
    }
    private float Search(
        Node current,
        Node goal,
        float g,
        float threshold,
        List<Node> path,
        List<Edge> pathEdges,
        HashSet<Node> visited,
        int level)
    {
        float f = g + Heuristic(current.pos, goal.pos);
        if (f > threshold)
            return f;

        if (current == goal)
            return FOUND;

        visited.Add(current);
        float min = float.PositiveInfinity;

        foreach (var edge in current.outgoingEdges)
        {
            var neighbor = edge.destinationNode;

            if (visited.Contains(neighbor) || neighbor.hierrarchyLevel < level)
                continue;

            pathEdges.Add(edge);
            path.Add(neighbor);
            float t = Search(
                neighbor,
                goal,
                g + edge.weight,
                threshold,
                path,
                pathEdges,
                visited,
                level);

            if (t == FOUND)
                return FOUND;

            if (t < min)
                min = t;    

            if (pathEdges.Count > 0)
                pathEdges.RemoveAt(pathEdges.Count - 1);
            path.RemoveAt(path.Count - 1);
        }
        
        return min;
    }

    private float Heuristic(int3 a, int3 b)
    {
        return math.abs(a.x - b.x) + math.abs(a.y - b.y);
    }
    
}