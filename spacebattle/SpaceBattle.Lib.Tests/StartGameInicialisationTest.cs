namespace SpaceBattle.Lib.Tests;

using Hwdtech;
using Moq;
using Hwdtech.Ioc;

public class StartGameInicialisationTest
{
    public StartGameInicialisationTest()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();
        IoC.Resolve<ICommand>("Scopes.Current.Set",
            IoC.Resolve<object>("Scopes.New",
                IoC.Resolve<object>("Scopes.Root")
                )).Execute();
        var pill = new ActionCommand(() =>
        {
            var ObjectsList = new List<UObject>();
            var SpaceShips = new List<UObject>();
            var StartPos = new List<Vector>();

            IoC.Resolve<ICommand>("IoC.Register", "Game.GetObjectList", (object[] args) =>
            {
                return ObjectsList;
            }).Execute();

            IoC.Resolve<ICommand>("IoC.Register", "Game.GetSpaceShipsList", (object[] args) =>
            {
                return SpaceShips;
            }).Execute();

            IoC.Resolve<ICommand>("IoC.Register", "Game.CalculateStartPosition", (object[] args) =>
            {
                Vector Center = new Vector(0, 0);
                int defx = 40;
                int defy = -40;
                for (int j = 0; j < 2; j++)
                {
                    for (int i = 0; i < (int)args[1]; i++)
                    {
                        ((List<Vector>)args[0]).Add(Center + new Vector(defx, defy));
                        defy += 80 / (int)args[1];
                    }
                    defy = -40;
                    defx = -40;
                }
                return (List<Vector>)args[0];
            }).Execute();

            IoC.Resolve<ICommand>("IoC.Register", "Game.CreatUObject", (object[] args) =>
                {
                    UObject uObject = new UObject();
                    uObject.SetProperty("Type", (string)args[0]);
                    uObject.SetProperty("Id", (int)args[1]);
                    return uObject;
                }).Execute();
        });
        IoC.Resolve<ICommand>("IoC.Register", "pill", (object[] args) => { return pill; }).Execute();
    }

    [Fact]

    public void ObjectGenerator_works_correct()
    {
        var p = IoC.Resolve<ICommand>("pill");
        IoC.Resolve<ICommand>("Scopes.Current.Set",
            IoC.Resolve<object>("Scopes.New",
                IoC.Resolve<object>("Scopes.Root")
                )).Execute();
        p.Execute();

        var uObject = new Mock<UObject>();
        string objectType = "spaceship";
        int objectsCount = 3;
        var SpaceShipsList = IoC.Resolve<List<UObject>>("Game.GetSpaceShipsList");
        var objectGenerator = new UObjectGenerator(objectsCount, objectType, SpaceShipsList);
        objectGenerator.Execute();

        var ObjectList = IoC.Resolve<List<UObject>>("Game.GetObjectList");

        Assert.Equal(objectsCount * 2, SpaceShipsList.Count);
        Assert.Equal(objectType, SpaceShipsList[0].GetProperty("Type"));
        Assert.Empty(ObjectList);
    }
    [Fact]
    public void ObjectGenerator_throws_exception_for_incorrect_input_of_objectType()
    {
        var p = IoC.Resolve<ICommand>("pill");
        IoC.Resolve<ICommand>("Scopes.Current.Set",
            IoC.Resolve<object>("Scopes.New",
                IoC.Resolve<object>("Scopes.Root")
                )).Execute();
        p.Execute();

        var uObject = new Mock<UObject>();
        string? objectType = null;
        int objectsCount = 3;
        var SpaceShipsList = IoC.Resolve<List<UObject>>("Game.GetSpaceShipsList");
        var objectGenerator = new UObjectGenerator(objectsCount, objectType, SpaceShipsList);
        Assert.Throws<ArgumentNullException>(() => objectGenerator.Execute());
    }
    [Fact]
    public void ObjectGenerator_throws_exception_for_incorrect_input_of_objectsCount()
    {
        var p = IoC.Resolve<ICommand>("pill");
        IoC.Resolve<ICommand>("Scopes.Current.Set",
            IoC.Resolve<object>("Scopes.New",
                IoC.Resolve<object>("Scopes.Root")
                )).Execute();
        p.Execute();

        var uObject = new Mock<UObject>();
        string? objectType = "spaceship";
        int objectsCount = 0;
        var SpaceShipsList = IoC.Resolve<List<UObject>>("Game.GetSpaceShipsList");
        var objectGenerator = new UObjectGenerator(objectsCount, objectType, SpaceShipsList);
        Assert.Throws<ArgumentNullException>(() => objectGenerator.Execute());
    }
    [Fact]
    public void Succesfully_generate_startPosition_and_set_this_property_to_spaceship()
    {
        var p = IoC.Resolve<ICommand>("pill");
        IoC.Resolve<ICommand>("Scopes.Current.Set",
            IoC.Resolve<object>("Scopes.New",
                IoC.Resolve<object>("Scopes.Root")
                )).Execute();
        p.Execute();
        string? objectType = "spaceship";
        int objectsCount = 3;
        var SpaceShipsList = IoC.Resolve<List<UObject>>("Game.GetSpaceShipsList");
        var StartPos = new List<Vector>();
        StartPos = IoC.Resolve<List<Vector>>("Game.CalculateStartPosition", StartPos, objectsCount);

        Assert.Equal(objectsCount * 2, StartPos.Count);
        var objectGenerator = new UObjectGenerator(objectsCount, objectType, SpaceShipsList);
        objectGenerator.Execute();
        var SetStartPosition = new SetPosition(SpaceShipsList, StartPos);
        SetStartPosition.Execute();
        for (int i = 0; i < objectsCount * 2; i++)
        {
            Assert.Equal(((Vector)SpaceShipsList[i].GetProperty("Position")), StartPos[i]);
        }
    }
    [Fact]
    public void Succesfully_set_Fuel_property_to_spaceship()
    {
        var p = IoC.Resolve<ICommand>("pill");
        IoC.Resolve<ICommand>("Scopes.Current.Set",
            IoC.Resolve<object>("Scopes.New",
                IoC.Resolve<object>("Scopes.Root")
                )).Execute();
        p.Execute();

        string? objectType = "spaceship";
        int objectsCount = 3;
        var SpaceShipsList = IoC.Resolve<List<UObject>>("Game.GetSpaceShipsList");
        int Fuel = 40;
        var objectGenerator = new UObjectGenerator(objectsCount, objectType, SpaceShipsList);
        objectGenerator.Execute();
        var SetStartFuel = new SetFuel(SpaceShipsList, Fuel);
        SetStartFuel.Execute();
        for (int i = 0; i < objectsCount * 2; i++)
        {
            Assert.Equal(SpaceShipsList[i].GetProperty("Fuel"), Fuel);
        }
    }
}
