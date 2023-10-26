using Moq;

namespace SpaceBattle.Lib.Tests;

public class TurnCommandTest
{
    [Fact]
    public void TurnCommand1()
    {
        var turnable = new Mock<ITurnable>();

        turnable.SetupGet(m => m.Angle).Returns(new Angles(45,"OX","IsPosition")).Verifiable();
        turnable.SetupGet(m => m.Turn).Returns(new Angles(45,"OX","by")).Verifiable();

        ICommand turnCommand = new TurnCommand(turnable.Object);

        turnCommand.Execute();

        turnable.VerifySet(m => m.Angle = new Angles(90,"OX","IsPosition"), Times.Once);
        turnable.VerifyAll();
    }

    [Fact]
    public void TurnCommand2()
    {
        var turnable = new Mock<ITurnable>();
            
        turnable.SetupGet(m => m.Angle).Returns(new Angles("","OX","IsPosition")).Verifiable();
        turnable.SetupGet(m => m.Turn).Returns(new Angles(45,"OX","by")).Verifiable();

        ICommand turnCommand = new TurnCommand(turnable.Object);
        try
        {
            turnCommand.Check();
            turnCommand.Execute();
        }
        catch(FormatException)
        {
            var result = false;
            Assert.False(result);
        }
    }
    [Fact]
    public void TurnCommand3()
    {
        var turnable = new Mock<ITurnable>();
            
        turnable.SetupGet(m => m.Angle).Returns(new Angles(45,"OX","IsPosition")).Verifiable();
        turnable.SetupGet(m => m.Turn).Returns(new Angles("","OX","by")).Verifiable();

        ICommand turnCommand = new TurnCommand(turnable.Object);
        try{
        turnCommand.Check();
        turnCommand.Execute();
        }
        catch(FormatException)
        {
            var result = false;
            Assert.False(result);
        }
    }
    [Fact]
    public void TurnCommand4()
    {
        var turnable = new Mock<ITurnable>();
            
        turnable.SetupGet(m => m.Angle).Returns(new Angles(45,"OX","IsPosition")).Verifiable();
        turnable.SetupGet(m => m.Turn).Returns(new Angles(45,"OX","by")).Verifiable();
        turnable.SetupGet(m => m.TurnAbility).Returns(new TurnAbility(false)).Verifiable();

        ICommand turnCommand = new TurnCommand(turnable.Object);
        try{
        turnCommand.Check();
        turnCommand.TurnAbilityCheck();
        turnCommand.Execute();
        }
        catch(FormatException)
        {
            var result = false;
            Assert.False(result);
        }
    }
}