using System.Linq;
using NUnit.Framework;

public class GraphTests
{
    [TestCase(1, 1, 1)]
    [TestCase(5, 5, 2)]
    [TestCase(2, 7, 3)]
    public void GraphActualMapNodesTest(int length, int width, int height)
    {
        var tiles = GridUtils.CreateGrid(length, width, height);
        var map = new GridMap(tiles);
        LayeredGridPreprocessing layeredGridPreprocessing = new LayeredGridPreprocessing(map, new IterativeDeepeningAstar(), 5, 3, false);
        var graph = layeredGridPreprocessing.CreateGraph();
        foreach (Tile tile in tiles)
        {
            var node = graph.NodesMap[tile.pos];
            Assert.IsNotNull(node);
            Assert.AreEqual(tile.pos, node.pos);
        }
    }
    
    [TestCase(1, 1, 1, -1, Axis.X)]
    [TestCase(5, 5, 2, 1, Axis.X)]
    [TestCase(2, 7, 3, -1, Axis.Y)]
    [TestCase(5, 5, 2, 1, Axis.Y)]
    public void ActualMapEdgesTest(int length, int width, int height, int neighbourDirection, Axis axis)
    {
        var tiles = GridUtils.CreateGrid(length, width, height);
        var map = new GridMap(tiles);
        LayeredGridPreprocessing layeredGridPreprocessing = new LayeredGridPreprocessing(map, new IterativeDeepeningAstar(), 5, 3, false);
        var graph = layeredGridPreprocessing.CreateGraph();
        foreach (var tile in tiles)
        {
            var node = graph.NodesMap[tile.pos];
            int3 neigbourPos;
            if (axis == Axis.X)
            {
                if (!(tile.pos.y + neighbourDirection < width && node.pos.y + neighbourDirection > 0)) return;
                neigbourPos = new int3(tile.pos.x, tile.pos.y + neighbourDirection, tile.pos.z);
            }
            else
            {
                if (!(tile.pos.x + neighbourDirection < length && node.pos.x + neighbourDirection > 0)) return;
                neigbourPos = new int3(tile.pos.x + neighbourDirection, tile.pos.y, tile.pos.z);
            }
            var neighbourNode = graph.NodesMap[neigbourPos];
            Assert.True(node.outgoingEdges.FirstOrDefault(e => e.destinationNode == neighbourNode) != null);
        }
    }

    [TestCase(0, 0, 0, 5, 0, 0, 5, Axis.X)]
    [TestCase(0, 0, 0, 0, 5, 0, 5, Axis.Y)]
    [TestCase(5, 0, 0, 0, 0, 0, 5, Axis.X)]
    [TestCase(0, 5, 0, 0, 0, 0, 5, Axis.Y)]
    public void AdjacentClustersTest(int x1, int y1, int z1, int x2, int y2, int z2, int clusterSize, Axis axis)
    {
        var clusterA = new IntBound2D(new int2(x1, y1), new int2(x1 + clusterSize - 1, y1 + clusterSize - 1), z1);
        var clusterB = new IntBound2D(new int2(x2, y2), new int2(x2 + clusterSize - 1, y2 + clusterSize - 1), z2);
        Assert.True(LayeredGridPreprocessing.IsClustersAdjacent(clusterA, clusterB, axis, out _, out _));
    }
    
}