using System.Net;
using System.Reflection.Metadata;
using System.Security.Cryptography.X509Certificates;

namespace SpaceBattle.Lib;

public class Vector
{
    private int[] coordinates;
    private int coord_cont;
    public Vector (params int[] coordinates){
        this.coordinates = coordinates;
        this.coord_cont = coordinates.Length;
    }
    public static Vector IsNotNull(Vector a){
        if (a.coordinates.Length == 0){
            throw new System.Exception();
        }
        return a;
    }
    public static Vector operator +(Vector a, Vector b){
        Vector c = new(new int[a.coord_cont]);
        for (int i = 0;i <a.coord_cont;i++){
            c.coordinates[i] = a.coordinates[i]+b.coordinates[i];
        }
        return c;
    }
    public override bool Equals(object? obj)
    {
    if (obj == null || obj is not Vector)
    return false;
    else
    return coordinates.SequenceEqual( ( (Vector)obj ).coordinates);
    }

    public override int GetHashCode()
    {
    return coordinates.GetHashCode();
    }
}