using Moq;

namespace SpaceBattle.Lib.Tests;

public class MoveCommandTest
{

    [Fact]
    public void MoveCommandPositive()
    {
        var movable = new Mock<IMovable>();

        movable.SetupGet(m => m.Position).Returns(new Vector (12, 5)).Verifiable();
        movable.SetupGet(m => m.Velocity).Returns(new Vector ( -7, 3 )).Verifiable();

        ICommand moveCommand = new MoveCommand(movable.Object);

        moveCommand.Execute();

        movable.VerifySet(m => m.Position = new Vector (5, 8), Times.Once);
        movable.VerifyAll();
    }
}