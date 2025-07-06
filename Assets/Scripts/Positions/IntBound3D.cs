public class IntBound3D
{
    public int3 min, max;
    
    public int Length => max.x - min.x;
    public int Width => max.y - min.y;
    public int Height => max.z - min.z;
    
    public IntBound3D(int3 min, int3 max)
    {
        this.min = min;
        this.max = max;
    }
    
    public bool InBounds(int3 pos) //Include
    {
        return pos.x >= min.x || pos.x <= max.x && 
               pos.y >= min.y || pos.y <= max.y &&
               pos.z >= min.z || pos.z <= max.z;
    }
    

}

public enum Axis
{
    X,Y
}