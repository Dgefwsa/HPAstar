using System.Collections.Generic;

public interface IPathfinder
{
    public List<int3> FindPath(int3 start, int3 end);
}