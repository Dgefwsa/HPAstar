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
    private AstarPathfinderManager _astarPathfinderManager;
    
    private GridMap _map;
    private Graph _graph;
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
    public int FindPathLevel;
    

    public void FindPath()
    {
        var start = new int3(0, 0, 0);
        var end = new int3(_map.Length - 1, _map.Width - 1, 0);
        var path = _astarPathfinderManager.FindPath(start, end);
        if(path == null) return;
        _graphVisualizer.Path = path;
        Debug.Log(path.Count);
    }

    public void FindPathOld()
    {
        Profiler.BeginSample("FindPathOld");
        var start = new int3(0, 0, 0);
        var end = new int3(_map.Length - 1, _map.Width - 1, 0);
        var path = _pathfinder.FindPath(_graph, start, end, 0);
        if(path == null) return;
        _graphVisualizer.Path = path;
        Debug.Log(path.Count);
        Profiler.EndSample();
    }
    
    public void CreateMap()
    {
        Profiler.BeginSample("CreateMap");
        
        var tiles = GridUtils.CreateGrid(Length, Width, Height);
        foreach (var tile in tiles)
        {
            var rand  = Random.Range(0f, 1f);
            if (rand < obstacleChance && !tile.pos.Equals(new int3(0, 0, 0)) && !tile.pos.Equals(new int3(Length - 1, Width - 1, 0)))
                tile.isObstacle = true;
        }

        _map = new GridMap(tiles);
        _pathfinder = new IterativeDeepeningAstar();
        LayeredGridPreprocessing layeredGridPreprocessing = new LayeredGridPreprocessing(_map, _pathfinder, InitialClusterSize, HierarchyMaxLevel, UseDiagonals);
        
        _graph = layeredGridPreprocessing.CreateGraph();
        _astarPathfinderManager = new AstarPathfinderManager(_graph);
        
        _graphVisualizer = GetComponent<GraphVisualization>();
        _graphVisualizer.Graph = _graph;
        _graphVisualizer.Path = new List<int3>();
        
        Profiler.EndSample();
    }
}