using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor.Graphs;

public class LayeredGridPreprocessing
{
    private readonly int _initialClusterSize;
    private readonly int _clusterSizeMultiplier = 2; // actual cluster size if CurrentLevel > 0 = InitialClusterSize * ClusterSizeMultiplier * CurrentLevel
    private readonly int _hierarchyMaxLevel; // level 0 is always actual map graph
    private readonly GridMap _map;
    private List<IntBound2D> _clusters;
    private int _currentLevel;
    private readonly bool _useDiagonal;

    public LayeredGridPreprocessing(GridMap map, int initialClusterSize, int hierarchyMaxLevel, bool useDiagonal)
    {
        _initialClusterSize = initialClusterSize;
        _hierarchyMaxLevel = hierarchyMaxLevel;
        _useDiagonal = useDiagonal;
        _map = map;
    }
    
    public Graph CreateGraph(Dictionary<Tile, Tile> additionalConnections = null)
    {
        Graph graph = new Graph();
        DefineActualMapInGraph(graph);

        for (int i = 0; i <= _hierarchyMaxLevel; i++)
        {
            DefineMapAbstractionInGraph(graph, i);
        }
        
        if (additionalConnections != null)
            DefineAdditionalConnections(additionalConnections, graph);
        
        return graph;
    }

    private void DefineActualMapInGraph(Graph graph)
    {
        for (int i = 0; i < _map.Tiles.Count; i++)
        {
            if (!_map.Tiles[i].isObstacle) 
                graph.SetNode(_map.Tiles[i].pos, 0);
        }
        for (int i = 0; i < _map.Tiles.Count; i++)
        for (int j = -1; j < 1; j += 2)
        {
            var pos = _map.Tiles[i].pos;
            if (_map.TilesMap.TryGetValue(pos, out Tile tile) && _map.TilesMap.TryGetValue(new int3(pos.x + j, pos.y, pos.z), out Tile tile2))
                if (!tile.isObstacle && !tile2.isObstacle)
                    graph.SetEdge(tile.pos, tile2.pos, 1, 0, true);
            if (_map.TilesMap.TryGetValue(pos, out tile) && _map.TilesMap.TryGetValue(new int3(pos.x, pos.y + j, pos.z), out tile2))
                if (!tile.isObstacle && !tile2.isObstacle)
                    graph.SetEdge(tile.pos, tile2.pos, 1, 0, true);
            //TODO add diagonals SetEdge
        }
    }
    
    private void DefineMapAbstractionInGraph(Graph graph, int level)
    {
        var clusterSize = _initialClusterSize * _clusterSizeMultiplier * level;
        if (clusterSize > _map.Length || clusterSize > _map.Width || clusterSize == 0) return;
        
        var clusterLength = _map.Length / clusterSize;
        var clusterWidth = _map.Width / clusterSize;
        _clusters = new List<IntBound2D>(clusterLength * clusterWidth * _map.Height);
        
        for (int x = 0; x < clusterLength; x++)
        for (int y = 0; y < clusterWidth; y++)
        for (int z = 0; z < _map.Height; z++)
        {
            int2 minCluster = new int2(x * clusterSize, y * clusterSize);
            int2 maxCluster = new int2(minCluster.x + clusterSize - 1, minCluster.y + clusterSize - 1);
            IntBound2D clusterBound = new IntBound2D(minCluster, maxCluster, z);
            _clusters.Add(clusterBound);
        }

        BuildAdjacentClustersConnection(_map, graph, level);


    }

    private void BuildAdjacentClustersConnection(GridMap gridMap, Graph graph, int level)
    {
        for (int i = 0; i < _clusters.Count; i++)
        for (int j = i + 1; j < _clusters.Count; j++)
        {
            if (IsClustersAdjacent(_clusters[i], _clusters[j], Axis.X, out var left , out var right))
            {
                DefineAdjacentClustersInterEdges(left, right, gridMap, graph, Axis.X, level);
            }
            if (IsClustersAdjacent(_clusters[i], _clusters[j], Axis.Y, out var bot , out var top))
            {
                DefineAdjacentClustersInterEdges(bot, top, gridMap, graph, Axis.Y, level);
            }
        }
    }

    private void DefineAdjacentClustersInterEdges(IntBound2D clusterA, IntBound2D clusterB, GridMap gridMap, Graph graph, Axis axis, int level)
    {
        int iMin, iMax;
        if (axis == Axis.X)
        {
            iMin = clusterA.min.y;
            iMax = iMin + clusterA.Length;
        }
        else
        {
            iMin = clusterA.min.x;
            iMax = iMin + clusterA.Width;
        }

        int lineSize = 0;
        int i;
        for (i = iMin; i < iMax; i++)
        {
            var posA = axis == Axis.X ? new int3(clusterA.max.x, i, clusterA.CurrentHeight) : new int3(i, clusterA.max.y, clusterA.CurrentHeight);
            var posB = axis == Axis.X ? new int3(clusterB.min.x, i, clusterB.CurrentHeight) : new int3(i, clusterB.min.y, clusterB.CurrentHeight);
            if (!gridMap.TilesMap[posA].isObstacle && !gridMap.TilesMap[posB].isObstacle)
            {
                lineSize++;
            }
            else
            {
                if (lineSize > 0)
                {
                    if (lineSize > 5)
                    {
                        var entranceA = axis == Axis.X ? new int3(clusterA.max.x, i - 1, clusterA.CurrentHeight) : new int3(i - 1, clusterA.max.y, clusterA.CurrentHeight);
                        var entranceB = axis == Axis.X ? new int3(clusterB.min.x, i - 1, clusterB.CurrentHeight) : new int3(i - 1, clusterB.min.y, clusterB.CurrentHeight);
                        graph.SetNode(entranceA, level);
                        graph.SetNode(entranceB, level);
                        graph.SetEdge(entranceA, entranceB, 1, level, true);
                        entranceA = axis == Axis.X ? new int3(clusterA.max.x, i - lineSize, clusterA.CurrentHeight) : new int3(i - lineSize, clusterA.max.y, clusterA.CurrentHeight);
                        entranceB = axis == Axis.X ? new int3(clusterB.min.x, i - lineSize, clusterB.CurrentHeight) : new int3(i - lineSize, clusterB.min.y, clusterB.CurrentHeight);
                        graph.SetEdge(entranceA, entranceB, 1, level, true);
                    }
                    else
                    {
                        var entranceA = axis == Axis.X ? new int3(clusterA.max.x, i - (lineSize / 2 + 1), clusterA.CurrentHeight) : new int3(i - 1, clusterA.max.y, clusterA.CurrentHeight);
                        var entranceB = axis == Axis.X ? new int3(clusterB.min.x, i - (lineSize / 2 + 1), clusterB.CurrentHeight) : new int3(i - 1, clusterB.min.y, clusterB.CurrentHeight);
                        graph.SetNode(entranceA, level);
                        graph.SetNode(entranceB, level);
                        graph.SetEdge(entranceA, entranceB, 1, level, true);
                    }
                    lineSize = 0;
                }
            }
        }
        if (lineSize > 0)
        {
            if (lineSize > 5)
            {
                var entranceA = axis == Axis.X ? new int3(clusterA.max.x, i, clusterA.CurrentHeight) : new int3(i, clusterA.max.y, clusterA.CurrentHeight);
                var entranceB = axis == Axis.X ? new int3(clusterB.min.x, i, clusterB.CurrentHeight) : new int3(i, clusterB.min.y, clusterB.CurrentHeight);
                graph.SetNode(entranceA, level);
                graph.SetNode(entranceB, level);
                graph.SetEdge(entranceA, entranceB, 1, level, true);
                entranceA = axis == Axis.X ? new int3(clusterA.max.x, i - lineSize, clusterA.CurrentHeight) : new int3(i - lineSize, clusterA.max.y, clusterA.CurrentHeight);
                entranceB = axis == Axis.X ? new int3(clusterB.min.x, i - lineSize, clusterB.CurrentHeight) : new int3(i - lineSize, clusterB.min.y, clusterB.CurrentHeight);
                graph.SetNode(entranceA, level);
                graph.SetNode(entranceB, level);
                graph.SetEdge(entranceA, entranceB, 1, level, true);
            }
            else
            {
                var entranceA = axis == Axis.X ? new int3(clusterA.max.x, i - (lineSize / 2 + 1), clusterA.CurrentHeight) : new int3(i, clusterA.max.y, clusterA.CurrentHeight);
                var entranceB = axis == Axis.X ? new int3(clusterB.min.x, i - (lineSize / 2 + 1), clusterB.CurrentHeight) : new int3(i, clusterB.min.y, clusterB.CurrentHeight);
                graph.SetNode(entranceA, level);
                graph.SetNode(entranceB, level);
                graph.SetEdge(entranceA, entranceB, 1, level, true);
            }
            lineSize = 0;
        }
    }

    private void DefineAdditionalConnections(Dictionary<Tile, Tile> additionalConnections, Graph graph)
    {
        foreach (KeyValuePair<Tile,Tile> connection in additionalConnections)
        {
            graph.SetEdge(connection.Key.pos, connection.Value.pos, 1, _hierarchyMaxLevel, true);
        }
    }
    
    /// <summary>
    /// Clusters are adjacent if each tile in two adjacent lines,
    /// that determine the border edge between cluster A and B,
    /// being the symmetrical with respect to the border
    /// </summary>
    public static bool IsClustersAdjacent(IntBound2D boundA, IntBound2D boundB, Axis axis, out IntBound2D clusterA, out IntBound2D clusterB)
    {
        clusterA = null;
        clusterB = null;
        
        if (boundA.CurrentHeight != boundB.CurrentHeight)
            return false;
        
        int aMin, aMax, bMin, bMax;
        int aMinOtherAxis1, aMaxOtherAxis1, bMinOtherAxis1, bMaxOtherAxis1;

        if (axis == Axis.X)
        {
            aMin = boundA.min.x;
            aMax = boundA.max.x;
            bMin = boundB.min.x;
            bMax = boundB.max.x;

            aMinOtherAxis1 = boundA.min.y;
            aMaxOtherAxis1 = boundA.max.y;
            bMinOtherAxis1 = boundB.min.y;
            bMaxOtherAxis1 = boundB.max.y;
        }
        else // Axis.Y
        {
            aMin = boundA.min.y;
            aMax = boundA.max.y;
            bMin = boundB.min.y;
            bMax = boundB.max.y;

            aMinOtherAxis1 = boundA.min.x;
            aMaxOtherAxis1 = boundA.max.x;
            bMinOtherAxis1 = boundB.min.x;
            bMaxOtherAxis1 = boundB.max.x;
        }
        
        if (aMinOtherAxis1 != bMinOtherAxis1 || aMaxOtherAxis1 != bMaxOtherAxis1)
            return false;
        
        if (aMax + 1 == bMin)
        {
            clusterA = boundA;
            clusterB = boundB;
            return true;
        }

        if (bMax + 1 == aMin)
        {
            clusterA = boundA;
            clusterB = boundB;
            return true;
        }

        return false;
    }
    
    
}