namespace SpaceBattle.Lib;

using Hwdtech;

public class SetPosition : ICommand
{
    List<UObject> _objects;
    private List<Vector> _PosList;
    public SetPosition(List<UObject> objects, List<Vector> posList)
    {
        this._objects = objects;
        this._PosList = posList;
    }
    public void Execute()
    {
        IEnumerator<Vector> posEnumeratior = _PosList.GetEnumerator();
        posEnumeratior.MoveNext();
        _objects.ForEach(obj =>
        {
            obj.SetProperty("Position", posEnumeratior.Current);
            posEnumeratior.MoveNext();
        });
        posEnumeratior.Reset();
    }
}
