using Hwdtech;
namespace SpaceBattle.Lib;

public class UObjectGenerator : ICommand
{
    private readonly int _objectsCount;
    private readonly string? _objectType;
    private List<UObject> _objects;

    public UObjectGenerator(int ObjectsCount, string? ObjectType, List<UObject> objects)
    {
        this._objectsCount = ObjectsCount;
        this._objectType = ObjectType;
        this._objects = objects;
    }

    public void Execute()
    {
        if (_objectType == null || _objectsCount == 0) { throw new ArgumentNullException(); }
        Enumerable.Range(0, _objectsCount * 2).ToList().ForEach(id =>
        {
            _objects.Add(IoC.Resolve<UObject>("Game.CreatUObject", _objectType, id));
        });
    }
}
