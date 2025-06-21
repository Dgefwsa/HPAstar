using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;
using Random = UnityEngine.Random;

public class Entry : MonoBehaviour
{
    public int pathCount;
    private IPathfinder _pathfinder;
    private GraphVisualization _graphVisualizer;
    
    private GridMap _map;
    [Header("Map options")]
    public int Length;
    public int Width;
    public int Height;
    [Range(0f, 1f)]
    public float obstacleChance;

    [Header("Graph options")] 
    public int InitialClusterSize;
    public int HierarchyMaxLevel;
    public bool UseDiagonals;
    

    public void FindPath()
    {
        var start = new int3(0, 0, 0);
        var end = new int3(_map.Length - 1, _map.Width - 1, 0);
        var path = _pathfinder.FindPath(start, end);
        _graphVisualizer.Path = path;
        Debug.Log(path.Count);
    }

    public void CreateMap()
    {
        Profiler.BeginSample("CreateMap");
        var tiles = GridUtils.CreateGrid(Length, Width, Height);
        foreach (var tile in tiles)
        {
            var rand  = Random.Range(0f, 1f);
            if (rand < obstacleChance)
                tile.isObstacle = true;
        }
        _map = new GridMap(tiles);
        LayeredGridPreprocessing layeredGridPreprocessing = new LayeredGridPreprocessing(_map, 5, 0, false);
        var graph = layeredGridPreprocessing.CreateGraph();
        _graphVisualizer = GetComponent<GraphVisualization>();
        _graphVisualizer.Graph = graph;
        _pathfinder = new IterativeDeepeningAstar(graph);
        Profiler.EndSample();
    }
}