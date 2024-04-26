namespace SpaceBattle.Lib.Tests;

using System.Collections.Concurrent;
using System.Reflection.Metadata;
using Hwdtech;
using Hwdtech.Ioc;
using Moq;

public class ServerThreadTest
{
    public ServerThreadTest()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();
<<<<<<< HEAD

        IoC.Resolve<ICommand>("Scopes.Current.Set",
            IoC.Resolve<object>("Scopes.New",
                IoC.Resolve<object>("Scopes.Root")
            )
        ).Execute();
=======
        
>>>>>>> 6edfca0 (Реализация тестов для SoftStop)

        IoC.Resolve<ICommand>("IoC.Register","Server.Commands.HardStop",(object[] args) =>
        {
            if (args.Count() == 2)
            {
                return new ActionCommand(()=>
                {
                    if (((ServerThread)args[0]).Equals(Thread.CurrentThread))
                    {
                        new HardStopCommand((ServerThread)args[0]).Execute();
                        new ActionCommand((Action)args[1]).Execute();
                    }
                });
            }
            return new ActionCommand(()=>
            {
                if (((ServerThread)args[0]).Equals(Thread.CurrentThread))
                {
                    new HardStopCommand((ServerThread)args[0]).Execute();
                }
            });
        }).Execute();

                IoC.Resolve<ICommand>("Scopes.Current.Set",
            IoC.Resolve<object>("Scopes.New",
                IoC.Resolve<object>("Scopes.Root")
            )
        ).Execute();

        IoC.Resolve<ICommand>("IoC.Register", "Server.Commands.SoftStop", (object[] args) =>
        {
            if (args.Count() == 2)
            {
                return new SoftStopCommand((ServerThread)args[0], (Action)args[1]);
            }
            return new SoftStopCommand((ServerThread)args[0], () => { });
        }).Execute();
    }

    [Fact]

    public void AnExceptionShouldNotStopServerThread()
    {
        var mre = new ManualResetEvent(false);
        var q = new BlockingCollection<ICommand>(100);
        var t = new ServerThread(q);

        var usualCommand = new Mock<ICommand>();
        usualCommand.Setup(m => m.Execute()).Verifiable();

        var exceptionCommand = new Mock<ICommand>();
        exceptionCommand.Setup(m => m.Execute()).Throws<Exception>().Verifiable();

        var hardStopCommand = IoC.Resolve<ICommand>("Server.Commands.HardStop", t, () => { mre.Set(); });

        q.Add(IoC.Resolve<ICommand>(
            "Scopes.Current.Set",
            IoC.Resolve<object>("Scopes.New",
            IoC.Resolve<object>("Scopes.Root")
            ))
        );

        var exceptionHandler = new Mock<ICommand>();
        exceptionHandler.Setup(m => m.Execute()).Verifiable();

        q.Add(
            IoC.Resolve<ICommand>("IoC.Register",
            "ExceptionHandler.Handle", (object[] args) => exceptionHandler.Object
            ));

        q.Add(usualCommand.Object);
        q.Add(exceptionCommand.Object);
        q.Add(usualCommand.Object);
        q.Add(hardStopCommand);
        q.Add(usualCommand.Object);

        t.Start();
        mre.WaitOne();

        exceptionHandler.Verify(m => m.Execute(), Times.Once());
        usualCommand.Verify(m => m.Execute(), Times.Exactly(2));
        Assert.Single(q);
    }
    [Fact]

    public void HardStopCommandShouldStopServer()
    {
        var mre = new ManualResetEvent(false);
        var q = new BlockingCollection<ICommand>(100);
        var t = new ServerThread(q);

        var hardStop = IoC.Resolve<ICommand>("Server.Commands.HardStop", t, () => { mre.Set(); });

        q.Add(new ActionCommand(() => { }));
        q.Add(new ActionCommand(() => { Thread.Sleep(3000); }));
        q.Add(hardStop);
        q.Add(new ActionCommand(() => { }));

        t.Start();
        mre.WaitOne();

        Assert.Single(q);
    }
    [Fact]
    public void HardStopCommandDoesNotStopAnotherThread()
    {
        var mre = new ManualResetEvent(false);
        var q1 = new BlockingCollection<ICommand>(100);
        var t1 = new ServerThread(q1);
        var q2 = new BlockingCollection<ICommand>(100);
        var t2 = new ServerThread(q2);

        var endcmd = new Mock<ICommand>();

        var wrongHardStop = IoC.Resolve<ICommand>("Server.Commands.HardStop", t2, () => { endcmd.Object.Execute(); });
        var hardStop1 = IoC.Resolve<ICommand>("Server.Commands.HardStop", t1);
        var hardStop2 = IoC.Resolve<ICommand>("Server.Commands.HardStop", t2, () => { mre.Set(); });

        q1.Add(new ActionCommand(() => { }));
        q1.Add(new ActionCommand(() => { Thread.Sleep(1000); }));
        q1.Add(wrongHardStop);
        q1.Add(hardStop1);
        q1.Add(new ActionCommand(() => { }));

        q2.Add(new ActionCommand(() => { }));
        q2.Add(new ActionCommand(() => { Thread.Sleep(3000); }));
        q2.Add(hardStop2);

        t1.Start();
        t2.Start();

        mre.WaitOne();

        endcmd.Verify(endcmd => endcmd.Execute(), Times.Never());
        Assert.Single(q1);
    }

    [Fact]
    public void SoftStopCommandShouldStopServer()
    {
        var mre = new ManualResetEvent(false);
        var q = new BlockingCollection<ICommand>(100);
        var t = new ServerThread(q);

        var softStopCommand = new SoftStopCommand(t, () => { mre.Set(); });

        q.Add(new ActionCommand(() => { }));
        q.Add(new ActionCommand(() => { Thread.Sleep(3000); }));
        q.Add(softStopCommand);
        q.Add(new ActionCommand(() => { }));

        t.Start();
        mre.WaitOne();

        Assert.Empty(q);
    }

    [Fact]
    public void SoftStopCommandDoesNotStopAnotherThread()
    {
        var mre = new ManualResetEvent(false);
        var q1 = new BlockingCollection<ICommand>(100);
        var t1 = new ServerThread(q1);
        var q2 = new BlockingCollection<ICommand>(100);
        var t2 = new ServerThread(q2);

        var endcmd = new Mock<ICommand>();

        var wrongSoftStop = IoC.Resolve<ICommand>("Server.Commands.SoftStop", t2, () => { endcmd.Object.Execute(); });
        var softStop1 = IoC.Resolve<ICommand>("Server.Commands.SoftStop", t1);
        var softStop2 = IoC.Resolve<ICommand>("Server.Commands.SoftStop", t2, () => { mre.Set(); });

        q1.Add(new ActionCommand(() => { }));
        q1.Add(wrongSoftStop);

        q2.Add(new ActionCommand(() => { }));
        q2.Add(new ActionCommand(() => { Thread.Sleep(3000); }));
        q2.Add(softStop2);

        t1.Start();
        t2.Start();

        q1.Add(new ActionCommand(() => { Thread.Sleep(1000); }));
        q1.Add(softStop1);
        q1.Add(new ActionCommand(() => { }));

        q2.Add(new ActionCommand(() => { Thread.Sleep(1000); }));
        q2.Add(softStop2);

        mre.WaitOne();

        endcmd.Verify(endcmd => endcmd.Execute(), Times.Never());
        Assert.Empty(q1);
    }

}
