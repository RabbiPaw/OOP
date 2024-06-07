namespace SpaceBattle.Lib;

using Hwdtech;

public class SetPosition : ICommand
{
    Dictionary<int, UObject> _objects;
    private List<Vector> _PosList;
    public SetPosition(Dictionary<int, UObject> objects, List<Vector> posList)
    {
        this._objects = objects;
        this._PosList = posList;
    }
    public void Execute()
    {
        for (int i = 0; i < _objects.Count; i++)
        {
            _objects[i].SetProperty("Position", _PosList[i]);
        }
    }

}