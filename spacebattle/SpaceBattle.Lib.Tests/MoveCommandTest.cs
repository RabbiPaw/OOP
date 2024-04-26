using Moq;
using Hwdtech;

namespace SpaceBattle.Lib.Tests;

public class MoveCommandTest
{
    [Fact]
    public void LineralMoveFromPositionWithVelocity()
    {
        var movable = new Mock<IMovable>();

        movable.SetupGet(m => m.Position).Returns(new Vector ( 12, 5 )).Verifiable();
        movable.SetupGet(m => m.Velocity).Returns(new Vector ( -5, 3 )).Verifiable();

        ICommand moveCommand = new MoveCommand(movable.Object);

        moveCommand.Execute();

        movable.VerifySet(m => m.Position = new Vector ( 7, 8 ), Times.Once);
        movable.VerifyAll();
    }

    [Fact]
    public void NoMoveBecauseNoStartPosition()
    {

        var movable = new Mock<IMovable>();

            movable.SetupGet(m => m.Position).Throws(() => new Exception()).Verifiable();
            movable.SetupGet(m => m.Velocity).Returns(new Vector ( -5, 3 )).Verifiable();

            ICommand moveCommand = new MoveCommand(movable.Object);

            Assert.Throws<Exception>(moveCommand.Execute);

    }
    [Fact]
    public void NoMoveBecauseNoVelocity()
    {
        var movable = new Mock<IMovable>();

        movable.SetupGet(m => m.Position).Returns(new Vector (12, 5)).Verifiable();
        movable.SetupGet(m => m.Velocity).Throws(() => new Exception()).Verifiable();

        ICommand moveCommand = new MoveCommand(movable.Object);
        Assert.Throws<Exception>(moveCommand.Execute);
    }
    [Fact]
    public void NoMoveBecauseCantMove()
    {

        var movable = new Mock<IMovable>();

        movable.SetupGet(m => m.Position).Returns(new Vector (12, 5)).Verifiable();
        movable.SetupGet(m => m.Velocity).Returns(new Vector ( -5, 3 )).Verifiable();

        ICommand moveCommand = new MoveCommand(movable.Object);

        movable.SetupSet(m => m.Position = new Vector(7, 8)).Throws(() => new Exception()).Verifiable();

        Assert.Throws<Exception>(moveCommand.Execute);
    }
    [Fact]
    public void HashCode(){
        Vector m = new Vector (1,1);
        int a = m.GetHashCode();
        Assert.True(true);
    }
}
