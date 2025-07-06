using System.Collections.Generic;

public interface IPathfinder
{
    public List<int3> FindPath(Graph graph, int3 start, int3 end, int level);
}