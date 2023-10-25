using Moq;

namespace SpaceBattle.Lib.Tests;

public class TurnCommandTest
{
    [Fact]
    public void MoveCommand1()
    {
        var turnable = new Mock<ITurnable>();

        turnable.SetupGet(m => m.Anglle).Returns(new Angles(45,"OX","IsPosition")).Verifiable();
        turnable.SetupGet(m => m.Turn).Returns(new Angles(45,"OX","by")).Verifiable();

        ICommand turnCommand = new TurnCommand(turnable.Object);

        turnCommand.Execute();

        turnable.VerifySet(m => m.Anglle = new Angles(90,"OX","IsPosition"), Times.Once);
        turnable.VerifyAll();
    }
}
