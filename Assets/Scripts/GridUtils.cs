using System.Collections.Generic;

public static class GridUtils
{
    public static List<Tile> CreateGrid(int length, int width, int height)
    {
        var grid = new List<Tile>();
        for (int x = 0; x < length; x++)
        for (int y = 0; y < width; y++)
        for (int z = 0; z < height; z++)
        {
            Tile tile = new Tile();
            tile.pos = new int3(x, y, z);
            grid.Add(tile);
        }
        return grid;
    }
}