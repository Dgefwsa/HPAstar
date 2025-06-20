using System;
using UnityEngine;

public class Entry : MonoBehaviour
{
    private void Start() 
    {
        var graphVisualizer = GetComponent<GraphVisualization>();
        var tiles = GridUtils.CreateGrid(80, 80, 1);
        var map = new GridMap(tiles);
        LayeredGridPreprocessing layeredGridPreprocessing = new LayeredGridPreprocessing(map, 5, 3, false);
        var graph = layeredGridPreprocessing.CreateGraph();
        graphVisualizer.Graph = graph;
    }
}