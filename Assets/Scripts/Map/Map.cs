using System.Collections.Generic;

public class GridMap
{
    public int Length; //x
    public int Width; //y
    public int Height; //z


    public Dictionary<int3, Tile> tilesMap;

    public IntBound bounds;
}