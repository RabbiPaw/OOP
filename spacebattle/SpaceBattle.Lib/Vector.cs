namespace SpaceBattle.Lib;

public class Vector
{
    private int[] coordinates;
    private int coord_cont;
    public Vector (params int[] coordinates){
        this.coordinates = coordinates;
        coord_cont = coordinates.Length;
    }
    public static Vector operator +(Vector a, Vector b){
        Vector c = new(new int[a.coord_cont]);
        for (int i = 0;i <a.coord_cont;i++){
            c.coordinates[i] = a.coordinates[i]+b.coordinates[i];
        }
        return c;
    }
    public override bool Equals(object obj)
    {
        return coordinates.SequenceEqual( ( (Vector)obj ).coordinates);
    }
    

    public override int GetHashCode()
    {
        return coordinates.GetHashCode();
    }
}