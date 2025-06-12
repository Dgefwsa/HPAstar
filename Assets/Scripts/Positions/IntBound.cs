public class IntBound
{
    public int3 min, max;
    
    public IntBound(int3 min, int3 max)
    {
        this.min = min;
        this.max = max;
    }
    
    public bool InBounds(int3 pos)
    {
        //TODO: override equality operators;
        return  true;
    }
    
}