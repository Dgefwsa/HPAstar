using System.Collections;
using NUnit.Framework;
using UnityEngine.TestTools;

public class PositionTests
{
    [TestCase(0, 0, 0, 0, 1, 0, false, ExpectedResult = 1f)]
    [TestCase(0, 0, 0, 0, -1, 0, false, ExpectedResult = 1f)]
    [TestCase(0, 0, 0, 1, 0, 0, false, ExpectedResult = 1f)]
    [TestCase(0, 0, 0, -1, 0, 0, false, ExpectedResult = 1f)]
    [TestCase(0, 0, 0, 0, 1, 0, true, ExpectedResult = 1.41f)]
    [TestCase(0, 0, 0, 1, 1, 0, true, ExpectedResult = 1.41f)]
    [TestCase(0, 0, 0, -1, 1, 0, true, ExpectedResult = 1.41f)]
    [TestCase(0, 0, 0, 1, -1, 0, true, ExpectedResult = 1.41f)]
    [TestCase(0, 0, 0, -1, -1, 0, true, ExpectedResult = 1.41f)]
    public float NeighbourCheck(int x1, int y1, int z1, int x2, int y2, int z2, bool diagonal)
    {
        var pos1 = new int3(x1, y1, z1);
        var pos2 = new int3(x2, y2, z2);
        var weight = PositionUtils.IsNeighbour(pos1, pos2, diagonal);
        return weight;
    }
}
