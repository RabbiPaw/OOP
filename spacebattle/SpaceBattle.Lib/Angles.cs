namespace SpaceBattle.Lib;

public class Angles
{
    private Int32 angle;
    private string axis;
    private string turning_direction;
    public  Angles(Int32 angle, string axis, string turning_direction){
        this.angle = angle;
        this.axis = axis;
        this.turning_direction = turning_direction;
    }

    public static Angles operator +(Angles a, Angles b){
        Angles c = new Angles(1,"1","1");
        if (a.axis == b.axis){
            if (b.turning_direction == "by"){
                c = new Angles(a.angle+b.angle,a.axis,"IsPosition");
            }
            else{
                if (b.turning_direction == "against"){
                    c = new Angles(a.angle-b.angle,a.axis,"IsPosition");
                }
                else{
                    c = a;
                }
            }
        }
        else{
            c = a;
        }
        return c;
    }
   public override bool Equals(object? obj)
    {
    if (obj == null || obj is not Angles)
    return false;
    else
    return angle.Equals( ( (Angles)obj ).angle) && axis.Equals(((Angles)obj).axis) && turning_direction.Equals(((Angles)obj).turning_direction);
    }

    public override int GetHashCode()
    {
    return GetHashCode();
    }
}
