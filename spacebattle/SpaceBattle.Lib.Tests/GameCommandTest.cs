namespace SpaceBattle.Lib.Tests;

using Moq;
using Hwdtech;
using Hwdtech.Ioc;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Diagnostics;

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

        IoC.Resolve<ICommand>("IoC.Register", "Server.Commands.GameCommand", (object[] args) =>
        {
            return new ActionCommand(() =>
            {
                new GameCommand((Queue<ICommand>)args[0], (object)args[1], (Dictionary<ICommand, Exception>)args[2]).Execute();
            });
        }).Execute();
        var pill = new ActionCommand(() =>
           {
               IoC.Resolve<ICommand>("IoC.Register", "Game.TimeQuant", (object[] args) => { return (object)50; }).Execute();
               IoC.Resolve<ICommand>("IoC.Register", "ExceptionHandler.Handle", (object[] args) => { return (string?)ED[(ICommand)args[0]]?.TargetSite?.Name; }).Execute();
               IoC.Resolve<ICommand>("IoC.Register", "ExceptionHandler.DefaultHandle", (object[] args) =>
               {
                   return (Exception)args[0];
               }).Execute();
               IoC.Resolve<ICommand>("IoC.Register", "ExceptionHandler.Checker", (object[] args) =>
               {
                   return new ActionCommand(() =>
                  {
                      if (((Dictionary<ICommand, Exception>)args[2]).ContainsKey((ICommand)args[0]) && ((Dictionary<ICommand, Exception>)args[2])[(ICommand)args[0]] != null)
                      {
                          IoC.Resolve<string>("ExceptionHandler.Handle", (ICommand)args[0], (Exception)args[1]);
                      }
                      else
                      {
                          throw IoC.Resolve<Exception>("ExceptionHandler.DefaultHandle", (Exception)args[1]); ;
                      }
                  });
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
        q.Enqueue(IoC.Resolve<ICommand>("IoC.Register", "Game.TimeQuant", (object[] args) => { return (object)50; }));
        q.Enqueue(cmd1.Object);
        q.Enqueue(cmd2.Object);

        var gameCommand = new GameCommand(q, scope, ExceptionDictionary);
        gameCommand.Execute();

        cmd1.Verify(x => x.Execute(), Times.Once);
        cmd2.Verify(x => x.Execute(), Times.Once);
    }

    [Fact]

    public void ExceptionHandel_Check()
    {
        var ExceptionDictionary = IoC.Resolve<Dictionary<ICommand, Exception>>("GetExceptionDict");
        var scope = IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"));
        var q = new Queue<ICommand>();
        var cmd = new Mock<ICommand>();
        var exceptionCMD = new Mock<ICommand>();
        cmd.Setup(x => x.Execute()).Verifiable();
        exceptionCMD.Setup(x => x.Execute()).Throws<Exception>().Verifiable();
        ExceptionDictionary.Add(exceptionCMD.Object, new Exception());

        q.Enqueue(IoC.Resolve<ICommand>("pill"));
        q.Enqueue(exceptionCMD.Object);
        q.Enqueue(cmd.Object);

        var gameCommand = new GameCommand(q, scope, ExceptionDictionary);
        gameCommand.Execute();

        exceptionCMD.Verify(x => x.Execute(), Times.Once());
        cmd.Verify(x => x.Execute(), Times.Once);

    }
    [Fact]
    public void DefaultHandle_Check()
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
        q.Enqueue(cmd.Object);

        var gameCommand = new GameCommand(q, scope, ExceptionDictionary);
        Assert.Throws<Exception>(() => { gameCommand.Execute(); });
        gameCommand.Execute();
        cmd.Verify(x => x.Execute(), Times.Once);
    }
    [Fact]
    public void Time_quant_test()
    {
        var ExceptionDictionary = IoC.Resolve<Dictionary<ICommand, Exception>>("GetExceptionDict");
        var scope = IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"));
        var q = new Queue<ICommand>();
        var cmd = new Mock<ICommand>();
        cmd.Setup(x => x.Execute()).Verifiable();

        q.Enqueue(IoC.Resolve<ICommand>("IoC.Register", "Game.TimeQuant", (object[] args) => { return (object)50; }));
        q.Enqueue(new ActionCommand(() => { Thread.Sleep(100); }));
        q.Enqueue(cmd.Object);

        var gameCommand = new GameCommand(q, scope, ExceptionDictionary);
        gameCommand.Execute();

        cmd.Verify(x => x.Execute(), Times.Never);
    }
}
