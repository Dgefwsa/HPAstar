using System;

[Serializable]
public struct int3 : System.IEquatable<int3>
{
    public int x;
    public int y;
    public int z;

    public int3(int x, int y, int z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }
    public bool Equals(int3 other)
    {
        return x == other.x && y == other.y && z == other.z;
    }

    public override bool Equals(object obj)
    {
        return obj is int3 other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(x, y, z);
    }
}