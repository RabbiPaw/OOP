using Moq;

namespace SpaceBattle.Lib.Tests;

public class TurnCommandTest
{
    [Fact]
    public void TurnCommand1()
    {
        var turnable = new Mock<ITurnable>();

        turnable.SetupGet(m => m.Angle).Returns(45).Verifiable();
        turnable.SetupGet(m => m.Turn).Returns(45).Verifiable();

        ICommand turnCommand = new TurnCommand(turnable.Object);

        turnCommand.Execute();

        turnable.VerifySet(m => m.Angle = 90, Times.Once);
        turnable.VerifyAll();
    }

    [Fact]
    public void TurnCommand2()
    {
        var turnable = new Mock<ITurnable>();
            
        turnable.SetupGet(m => m.Angle).Throws(() => new Exception()).Verifiable();;
        turnable.SetupGet(m => m.Turn).Returns(45).Verifiable();

        ICommand turnCommand = new TurnCommand(turnable.Object);
        Assert.Throws<Exception>(turnCommand.Execute);
        
    }
    [Fact]
    public void TurnCommand3()
    {
        var turnable = new Mock<ITurnable>();
            
        turnable.SetupGet(m => m.Angle).Returns(45).Verifiable();
        turnable.SetupGet(m => m.Turn).Throws(() => new Exception()).Verifiable();
        ICommand turnCommand = new TurnCommand(turnable.Object);
        Assert.Throws<Exception>(turnCommand.Execute);
        

    }
    [Fact]
    public void TurnCommand4()
    {
        var turnable = new Mock<ITurnable>();
            
        turnable.SetupGet(m => m.Angle).Returns(45).Verifiable();
        turnable.SetupGet(m => m.Turn).Returns(45).Verifiable();

        ICommand turnCommand = new TurnCommand(turnable.Object);

        turnable.SetupSet(m => m.Angle = 90).Throws(() => new Exception()).Verifiable();

        Assert.Throws<Exception>(turnCommand.Execute);
        turnable.VerifyAll();
    }
}