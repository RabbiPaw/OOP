using Moq;
using Hwdtech;
using Hwdtech.Ioc;
using System.Numerics;

namespace SpaceBattle.Lib.Tests;

public class StartCommandTests
{
    private readonly static Mock<IQueue> q;
    static StartCommandTests()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();

        var movement = new Mock<ICommand>();
        IoC.Resolve<Hwdtech.ICommand>(
            "IoC.Register", "Operations.Movement",
            (object[] args) => movement.Object
            ).Execute();

        var injectable = new Mock<ICommand>();
        IoC.Resolve<Hwdtech.ICommand>(
            "IoC.Register", "Commands.Injectable",
            (object[] args) => injectable.Object
            ).Execute();

        var setPropertiesCommand = new Mock<ICommand>();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Properties.Set", (object[] args) => setPropertiesCommand.Object).Execute();

        q = new Mock<IQueue>();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.Queue", (object[] args) => q.Object).Execute();
    }


    [Fact]
    public void MoveCommandStartRegistersPropertiesAndPutsMoveCommandInQueue()
    {
        var uobject = new Mock<IUObject>();

        var startable = new Mock<IMoveCommandStartable>();
        startable.Setup(s => s.Target).Returns(uobject.Object);
        startable.Setup(s => s.Properties).Returns(new Dictionary<string, object>() { { "Velocity", new Vector(1, 1) } });

        var startMoveCommand = new StartMoveCommand(startable.Object);

        startMoveCommand.Execute();

        startable.Verify(s => s.Properties, Times.Once());
        q.Verify(q => q.Put(It.IsAny<ICommand>()), Times.Once());
    }
    [Fact]
    public void NoIUObject()
    {
        var uobject = new Mock<IUObject>();

        var startable = new Mock<IMoveCommandStartable>();
        startable.Setup(s => s.Target).Throws(() => new Exception()).Verifiable();
        startable.Setup(s => s.Properties).Returns(new Dictionary<string, object>() { { "Velocity", new Vector(1, 1) } });

        var startMoveCommand = new StartMoveCommand(startable.Object);

        Assert.Throws<Exception>(startMoveCommand.Execute);
    }

    [Fact]
    public void NoPropertiesIUObject()
    {
        var uobject = new Mock<IUObject>();

        var startable = new Mock<IMoveCommandStartable>();
        startable.Setup(s => s.Target).Throws(() => new Exception()).Verifiable();
        startable.Setup(s => s.Properties).Throws(() => new Exception()).Verifiable();

        var startMoveCommand = new StartMoveCommand(startable.Object);

        Assert.Throws<Exception>(startMoveCommand.Execute);
    }
}
