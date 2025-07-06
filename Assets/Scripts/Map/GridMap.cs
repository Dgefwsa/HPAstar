using System.Collections.Generic;

public class GridMap
{
    public readonly int Length; //x
    public readonly int Width; //y
    public readonly int Height; //z


    public Dictionary<int3, Tile> TilesMap;
    public List<Tile> Tiles;
    public Dictionary<Tile, Tile> AdditionalConnections;
    
    public IntBound3D MapBounds3D;

    public GridMap(List<Tile> tiles)
    {
        Tiles = tiles;
        TilesMap = new Dictionary<int3, Tile>();
        int maxX = 0, maxY = 0, maxZ = 0;
        foreach (var tile in tiles)
        {
            if (tile.pos.x > maxX) maxX = tile.pos.x;
            if (tile.pos.y > maxY) maxY = tile.pos.y;
            if (tile.pos.z > maxZ) maxZ = tile.pos.z;
            TilesMap.Add(tile.pos, tile);
        }
        Length = maxX + 1;
        Width = maxY + 1;
        Height = maxZ + 1;
        
        var botLeft = new int3(0, 0, 0);
        var topRight = new int3(Length - 1, Width - 1, Height - 1);
        MapBounds3D = new IntBound3D(botLeft, topRight);
    }
}