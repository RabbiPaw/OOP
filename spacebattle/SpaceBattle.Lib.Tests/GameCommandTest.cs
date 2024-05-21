namespace SpaceBattle.Lib.Tests;

using Moq;
using Hwdtech;
using Hwdtech.Ioc;
using System.ComponentModel.DataAnnotations;

public class GameCommandTest
{
    public GameCommandTest()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();

        IoC.Resolve<ICommand>("Scopes.Current.Set",
                IoC.Resolve<object>("Scopes.New",
                    IoC.Resolve<object>("Scopes.Root"))).Execute();

        var ED = new Dictionary<ICommand, Exception?>();

        IoC.Resolve<ICommand>("IoC.Register", "GetExceptionDict", (object[] args) => { return ED; }).Execute();

        IoC.Resolve<ICommand>("IoC.Register", "ExceptionHandler.Checker", (object[] args) =>
        {
            return new ActionCommand(() =>
            {
                new ExceptionChecker((ICommand)args[0], (Exception)args[1], (Dictionary<ICommand, Exception>)args[2]).Execute();
            });
        }).Execute();

        IoC.Resolve<ICommand>("IoC.Register", "Server.Commands.GameCommand", (object[] args) =>
        {
            return new ActionCommand(() =>
            {
                new GameCommand((Queue<ICommand>)args[0], (object)args[1], (Dictionary<ICommand, Exception>)args[2]).Execute();
            });
        }).Execute();
        var pill = new ActionCommand(() =>
           {
               IoC.Resolve<ICommand>("IoC.Register", "Game.TimeQuant", (object[] args) => { return (object)2000; }).Execute();
               IoC.Resolve<ICommand>("IoC.Register", "ExceptionHandler.Handle", (object[] args) => { return (object)ED[(ICommand)args[0]]; }).Execute();
               IoC.Resolve<ICommand>("IoC.Register", "ExceptionHandler.Checker", (object[] args) =>
               {
                   return new ExceptionChecker((ICommand)args[0], (Exception)args[1], (Dictionary<ICommand, Exception>)args[2]);
               }).Execute();
           });

        IoC.Resolve<ICommand>("IoC.Register", "pill", (object[] args) => { return pill; }).Execute();
    }
    [Fact]
    public void GameCommand_Correct_Work()
    {
        var ExceptionDictionary = IoC.Resolve<Dictionary<ICommand, Exception>>("GetExceptionDict");
        var scope = IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"));
        var q = new Queue<ICommand>();
        var cmd1 = new Mock<ICommand>();
        var cmd2 = new Mock<ICommand>();
        cmd1.Setup(x => x.Execute()).Verifiable();
        cmd2.Setup(x => x.Execute()).Verifiable();

        q.Enqueue(IoC.Resolve<ICommand>("pill"));
        q.Enqueue(cmd1.Object);
        q.Enqueue(cmd2.Object);

        var gameCommand = new GameCommand(q, scope, ExceptionDictionary);
        gameCommand.Execute();

        cmd1.Verify(x => x.Execute(), Times.Once);
        cmd2.Verify(x => x.Execute(), Times.Once);
    }

    [Fact]

    public void There_is_no_exception_in_the_dictionary_by_key()
    {
        var ExceptionDictionary = IoC.Resolve<Dictionary<ICommand, Exception>>("GetExceptionDict");
        var scope = IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"));
        var q = new Queue<ICommand>();
        var cmd = new Mock<ICommand>();
        var exceptionCMD = new Mock<ICommand>();
        cmd.Setup(x => x.Execute()).Verifiable();
        exceptionCMD.Setup(x => x.Execute()).Throws<Exception>().Verifiable();

        q.Enqueue(IoC.Resolve<ICommand>("pill"));
        q.Enqueue(exceptionCMD.Object);
        q.Enqueue(exceptionCMD.Object);
        q.Enqueue(cmd.Object);

        var gameCommand = new GameCommand(q, scope, ExceptionDictionary);
        gameCommand.Execute();

        cmd.Verify(x => x.Execute(), Times.Once);

        Assert.Single(ExceptionDictionary);
    }
    [Fact]
    public void There_is_no_exception_in_the_dictionary_by_value()
    {
        var ExceptionDictionary = IoC.Resolve<Dictionary<ICommand, Exception>>("GetExceptionDict");
        var scope = IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"));
        var q = new Queue<ICommand>();
        var cmd = new Mock<ICommand>();
        var exceptionCMD = new Mock<ICommand>();
        cmd.Setup(x => x.Execute()).Verifiable();
        exceptionCMD.Setup(x => x.Execute()).Throws<Exception>().Verifiable();
        ExceptionDictionary[exceptionCMD.Object] = null;

        q.Enqueue(IoC.Resolve<ICommand>("pill"));
        q.Enqueue(exceptionCMD.Object);
        q.Enqueue(exceptionCMD.Object);
        q.Enqueue(cmd.Object);

        var gameCommand = new GameCommand(q, scope, ExceptionDictionary);
        gameCommand.Execute();

        cmd.Verify(x => x.Execute(), Times.Once);

        Assert.Single(ExceptionDictionary);
    }
    [Fact]
    public void There_is_no_exception_in_the_dictionary_by_another_value()
    {
        var ExceptionDictionary = IoC.Resolve<Dictionary<ICommand, Exception>>("GetExceptionDict");
        var scope = IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"));
        var q = new Queue<ICommand>();
        var cmd = new Mock<ICommand>();
        var exceptionCMD = new Mock<ICommand>();
        cmd.Setup(x => x.Execute()).Verifiable();
        exceptionCMD.Setup(x => x.Execute()).Throws<Exception>().Verifiable();
        ExceptionDictionary[exceptionCMD.Object] = null;

        q.Enqueue(IoC.Resolve<ICommand>("pill"));
        q.Enqueue(exceptionCMD.Object);
        q.Enqueue(exceptionCMD.Object);
        q.Enqueue(cmd.Object);

        var gameCommand = new GameCommand(q, scope, ExceptionDictionary);
        gameCommand.Execute();

        cmd.Verify(x => x.Execute(), Times.Once);

        Assert.Single(ExceptionDictionary);
    }
}
