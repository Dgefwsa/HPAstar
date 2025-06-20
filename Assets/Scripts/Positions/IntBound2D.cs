public class IntBound2D
{
    public int2 min, max;
    public int CurrentHeight; //TODO: make separate data for clusters
    public int Length => max.x - min.x;
    public int Width => max.y - min.y;
    public IntBound2D(int2 min, int2 max, int currentHeight)
    {
        this.min = min;
        this.max = max;
        this.CurrentHeight = currentHeight;
    }
    
    public bool InBounds(int2 pos) //Include
    {
        return pos.x >= min.x || pos.x <= max.x &&
            pos.y >= min.y || pos.y <= max.y;
    }
}