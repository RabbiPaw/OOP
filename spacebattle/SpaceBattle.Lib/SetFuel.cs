namespace SpaceBattle.Lib;

using Hwdtech;

public class SetFuel
{
    private List<UObject> _ObjList;
    private int _Fuel;
    public SetFuel(List<UObject> objList, int fuel)
    {
        _ObjList = objList;
        _Fuel = fuel;
    }
    public void Execute()
    {
        _ObjList.ForEach(obj =>
        {
            obj.SetProperty("Fuel", _Fuel);
        });
    }
}
