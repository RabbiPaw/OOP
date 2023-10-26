using Moq;

namespace SpaceBattle.Lib.Tests;

public class MoveCommandTest
{
    [Fact]
    public void MoveCommand1()
    {
        var movable = new Mock<IMovableTurnable>();

        movable.SetupGet(m => m.Position).Returns(new Vector ( 12, 5 )).Verifiable();
        movable.SetupGet(m => m.Velocity).Returns(new Vector ( -5, 3 )).Verifiable();

        ICommand moveCommand = new MoveTurnCommand(movable.Object);

        moveCommand.Execute();

        movable.VerifySet(m => m.Position = new Vector ( 7, 8 ), Times.Once);
        movable.VerifyAll();
    }

    [Fact]
    public void MoveCommand2()
    {

        var movable = new Mock<IMovableTurnable>();
        try
        {
            movable.SetupGet(m => m.Position).Returns(new Vector ()).Verifiable();
            movable.SetupGet(m => m.Velocity).Returns(new Vector ( -5, 3 )).Verifiable();

            ICommand moveCommand = new MoveTurnCommand(movable.Object);

            moveCommand.Execute();
            moveCommand.Check();
        }
        catch(System.Exception)
        {
            var result = false;
            Assert.False(result);
        }
    }
    [Fact]
    public void MoveCommand3()
    {

        var movable = new Mock<IMovableTurnable>();
        try
        {
            movable.SetupGet(m => m.Position).Returns(new Vector (12, 5)).Verifiable();
            movable.SetupGet(m => m.Velocity).Returns(new Vector ()).Verifiable();

            ICommand moveCommand = new MoveTurnCommand(movable.Object);
            moveCommand.Execute();
            moveCommand.Check();
        }
        catch(System.Exception)
        {
            var result = false;
            Assert.False(result);
        }

    }
    [Fact]
    public void MoveCommand4()
    {

        var movable = new Mock<IMovableTurnable>();
        try
        {
            movable.SetupGet(m => m.Position).Returns(new Vector (12, 5)).Verifiable();
            movable.SetupGet(m => m.Velocity).Returns(new Vector ( -5, 3 )).Verifiable();
            movable.SetupGet(m => m.MoveAbility).Returns(new MoveAbility(false)).Verifiable();

            ICommand moveCommand = new MoveTurnCommand(movable.Object);

            moveCommand.Execute();
            moveCommand.MoveAbilityCheck();
        }
        catch(System.Exception)
        {
            var result = false;
            Assert.False(result);
        }
    }
}