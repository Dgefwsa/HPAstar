using System.Collections.Generic;

public class LayeredGridPreprocessing
{
    public int InitialClusterSize;
    public int ClusterLevelMultiplier = 3;
    public int HierarchyMaxLevel; // level 0 is always actual map graph
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
        var clusterSize = InitialClusterSize * ClusterLevelMultiplier * level;
        for (int x = 0; x < tiles.Length; x++)
            
    }
    
    private void DefineAdditionalConnections(Dictionary<Tile, Tile> additionalConnections, Graph graph)
    {
        foreach (KeyValuePair<Tile,Tile> connection in additionalConnections)
        {
            graph.SetEdge(connection.Key.pos, connection.Value.pos, 1, HierarchyMaxLevel, true);
        }
    }
    
    
}