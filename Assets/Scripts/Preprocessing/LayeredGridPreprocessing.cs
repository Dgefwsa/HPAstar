using System.Collections.Generic;

public class LayeredGridPreprocessing
{
    public int InitialClusterSize;
    public int ClusterLevelMultiplier = 3;
    public int HierarchyMaxLevel; // level 0 is always actual map graph
    private List<int3> clusters;
    private int currentLevel;
    public bool useDiagonal = false;

    public Graph CreateGraph(Tile[] tiles, Dictionary<Tile, Tile> additionalConnections = null)
    {
        Graph graph = new Graph();
        DefineActualMap(graph, tiles);

        for (int i = 0; i < HierarchyMaxLevel; i++)
        {
            DefineGraphAbstraction(tiles, graph, i);
        }
        
        DefineAdditionalConnections(additionalConnections, graph);
        
        return graph;
    }

    private void DefineActualMap(Graph graph, Tile[] tiles)
    {
        for (int i = 0; i < tiles.Length; i++)
        for (int j = i + 1; j < tiles.Length; j++)
        {
            graph.SetNode(tiles[i].pos, 0);
            var weight = PositionUtils.IsNeighbour(tiles[i].pos, tiles[j].pos, useDiagonal);
            if (weight != 0)
            {
                graph.SetEdge(tiles[i].pos, tiles[j].pos, weight, 0, true);
            }
        }
    }

    private void DefineGraphAbstraction(Tile[] tiles, Graph graph, int level)
    {
        Map map = new Map();
        var clusterSize = InitialClusterSize * ClusterLevelMultiplier * level;
        var clusterLength = map.Length / clusterSize;
        var clusterWidth = map.Width / clusterSize;
        int x, y, z;


        clusters = new List<int3>(clusterLength * clusterWidth * map.Height);
        for (x = 0; x < clusterLength; x++)
        for (y = 0; y < clusterWidth; y++)
        for (z = 0; z < map.Height; z++)
        {
            int3 minCluster = new int3(x*clusterSize, y*cluster, z)
            int3 maxCluster = new int3(minCluster.x + clusterSize, minCluster.y + clusterSize, z);
            IntBound clusterBound = new IntBound(minCluster, maxCluster);
            clusters.Add(clusterBound);

            //TODO: Check neighbours and create inter edges between clusters with level i;
            
            // 0,0,0 9 9 9, 10 10 10 19 19 19 (10)
        }

    }
    
    private void DefineAdditionalConnections(Dictionary<Tile, Tile> additionalConnections, Graph graph)
    {
        foreach (KeyValuePair<Tile,Tile> connection in additionalConnections)
        {
            graph.SetEdge(connection.Key.pos, connection.Value.pos, 1, HierarchyMaxLevel, true);
        }
    }
    
    
}