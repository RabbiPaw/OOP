namespace SpaceBattle.Lib;

using Hwdtech;

public class SetFuel{
    private Dictionary<int,UObject> _ObjList;
    private int _Fuel;
    public SetFuel(Dictionary<int,UObject> objList, int fuel){
        _ObjList = objList;
        _Fuel = fuel;
    }
    public void Execute()
    {
        for (int i = 0; i < _ObjList.Count; i++){
            _ObjList[i].SetProperty("Fuel", _Fuel);
        }
    }
}