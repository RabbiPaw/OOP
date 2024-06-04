using Hwdtech;
namespace SpaceBattle.Lib;

public class UObjectGenerator : ICommand{
    private readonly int _ObjectsCount;

    public UObjectGenerator(int ObjectsCount)
    {
        this._ObjectsCount = ObjectsCount;
    }

    public void Execute()
    {
        var ObjectList = IoC.Resolve<IDictionary<int,IUObject>>("Game.GetObjectList");
        Enumerable.Range(0, _ObjectsCount).ToList().ForEach(
            id => ObjectList.Add(id, IoC.Resolve<IUObject>("Game.CreatUObject")));
    }

}