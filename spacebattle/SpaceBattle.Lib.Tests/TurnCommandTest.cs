using Moq;

namespace SpaceBattle.Lib.Tests;

public class TurnCommandTest
{
    [Fact]
    public void MoveCommand1()
    {
        var turnable = new Mock<ITurnable>();

        turnable.SetupGet(m => m.Anglle).Returns(45).Verifiable();
        turnable.SetupGet(m => m.Turn).Returns(45).Verifiable();

        ICommand turnCommand = new TurnCommand(turnable.Object);

        turnCommand.Execute();

        turnable.VerifySet(m => m.Anglle = 90, Times.Once);
        turnable.VerifyAll();
    }
}
