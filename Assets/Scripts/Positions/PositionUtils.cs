using UnityEngine;

public static class PositionUtils
{
    public static float SQRT2 = 1.41f;
    
    
    /// <summary>
    /// 
    /// </summary>
    /// <returns>distance between neighbours (1 - straight, SQRT2 - diagonal. Return 0 if positions not neighbours</returns>
    public static float IsNeighbour(int3 pos1, int3 pos2, bool diagonal)
    {
        int i, j;
        var weight = diagonal ? SQRT2 : 1f;
        var isNeighbour = false;
        for (i = -1; i < 2; i += 2)
        {
            isNeighbour |= pos2.Equals(new int3(pos1.x + i, pos1.y, pos1.z));
            isNeighbour |= pos2.Equals(new int3(pos1.x - i, pos1.y, pos1.z));
            isNeighbour |= pos2.Equals(new int3(pos1.x, pos1.y + i, pos1.z));
            isNeighbour |= pos2.Equals(new int3(pos1.x, pos1.y - i, pos1.z));
        }
        if (isNeighbour) return weight;
        
        if (diagonal)
            for (i = -1; i < 2; i += 2)
            for (j = -1; j < 2; j += 2)
            {
                isNeighbour |= pos2.Equals(new int3(pos1.x + i, pos1.y + i, pos1.z));
                isNeighbour |= pos2.Equals(new int3(pos1.x - i, pos1.y + i, pos1.z));
                isNeighbour |= pos2.Equals(new int3(pos1.x + i, pos1.y - i, pos1.z));
                isNeighbour |= pos2.Equals(new int3(pos1.x - i, pos1.y - i, pos1.z));
            }
        if (isNeighbour) return weight;

        return 0;
    }
}