using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Profiling;

public class IterativeDeepeningAstar : IPathfinder
{
    private const float FOUND = -1;
    private Graph _graph;

    public IterativeDeepeningAstar(Graph graph)
    {
        _graph = graph;
    }
    public List<int3> FindPath(int3 startPos, int3 endPos)
    {
        Profiler.BeginSample("Iterative Deepening A*");
        if (!_graph.TryGetNode(startPos, out var start) || !_graph.TryGetNode(endPos, out var goal))
            return null;
        
        float threshold = Heuristic(startPos, endPos);
        var path = new List<Node> { start };

        var counter = 0;
        while (counter < _graph.NodesMap.Count)
        {
            counter++;
            var visited = new HashSet<Node>();
            float temp = Search(start, goal, 0, threshold, path, visited);

            if (temp == FOUND)
                return path.ConvertAll(n => n.pos);

            if (float.IsPositiveInfinity(temp))
                return null;

            threshold = temp;
        }
        Profiler.EndSample();
        return null;
    }
    private float Search(
        Node current,
        Node goal,
        float g,
        float threshold,
        List<Node> path,
        HashSet<Node> visited)
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

            if (visited.Contains(neighbor))
                continue;

            path.Add(neighbor);
            float t = Search(
                neighbor,
                goal,
                g + edge.weight,
                threshold,
                path,
                visited
            );

            if (t == FOUND)
                return FOUND;

            if (t < min)
                min = t;

            path.RemoveAt(path.Count - 1);
        }

        visited.Remove(current);
        return min;
    }

    private float Heuristic(int3 a, int3 b)
    {
        return math.abs(a.x - b.x) + math.abs(a.y - b.y) + math.abs(a.z - b.z);
    }
    
}