namespace SpaceBattle.Lib;

public class Vector
{
    private int[] coordinates;
    private int coord_cont;
    public Vector(params int[] coordinates)
    {
        this.coordinates = coordinates;
        coord_cont = coordinates.Length;
    }
    public static Vector operator +(Vector a, Vector b)
    {
        Vector c = new(new int[a.coord_cont]);
        c.coordinates = (a.coordinates.Select((x, index) => x + b.coordinates[index]).ToArray());
        return c;
    }
#pragma warning disable CS8765 // Nullability of type of parameter doesn't match overridden member (possibly because of nullability attributes).
    public override bool Equals(object obj)
#pragma warning restore CS8765 // Nullability of type of parameter doesn't match overridden member (possibly because of nullability attributes).
    {
        return coordinates.SequenceEqual(((Vector)obj).coordinates);
    }


    public override int GetHashCode()
    {
        return coordinates.GetHashCode();
    }
}
