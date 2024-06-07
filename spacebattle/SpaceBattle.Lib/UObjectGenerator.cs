using Hwdtech;
namespace SpaceBattle.Lib;

public class UObjectGenerator : ICommand{
    private readonly int _objectsCount;
    private readonly string? _objectType;
    private Dictionary<int,UObject> _objects;

    public UObjectGenerator(int ObjectsCount, string? ObjectType, Dictionary<int,UObject> objects)
    {
        this._objectsCount = ObjectsCount;
        this._objectType = ObjectType;
        this._objects = objects;
    }

    public void Execute()
    {
        if (_objectType == null || _objectsCount == 0){throw new ArgumentNullException();}
        for (int i = 0; i < _objectsCount*2;i++){
            _objects.Add(i, IoC.Resolve<UObject>("Game.CreatUObject",_objectType));
        }
    }

}