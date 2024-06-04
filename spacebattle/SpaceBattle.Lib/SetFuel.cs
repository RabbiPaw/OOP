using Hwdtech;

namespace SpaceBattle.Lib;

public class SetFuel: ICommand
{
    private readonly IEnumerable<IUObject> _spaceships;
    private int _fuel;

    public SetFuel(IEnumerable<IUObject> spaceships, int fuel)
    {
        _spaceships = spaceships;
        _fuel = fuel;
    }

    public void Execute()
    {
        _spaceships.ToList().ForEach(ship => 
        IoC.Resolve<ICommand>("Game.UObject.SetProperty", ship, "Fuel", _fuel).Execute());
    }

}