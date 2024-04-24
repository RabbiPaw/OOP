namespace SpaceBattle.Lib.Tests;

using Moq;
using Hwdtech;
using Hwdtech.Ioc;
using WebHttp;
using System.Collections.Concurrent;

public class EndPointTest
{
    public EndPointTest()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();
        IoC.Resolve<ICommand>("Scopes.Current.Set",
            IoC.Resolve<object>("Scopes.New",
                IoC.Resolve<object>("Scopes.Root"))).Execute();

        var queueCollection = new Dictionary<Guid, BlockingCollection<ICommand>>();
        var threadCollection = new Dictionary<Guid, ServerThread>();
        var IdServersAndThreads = new Dictionary<Guid, List<Guid>>();

        IoC.Resolve<ICommand>("IoC.Register", "Server.Commands.RegisterThread", (object[] args) =>
        {
            return new ActionCommand(() =>
            {
                queueCollection.Add((Guid)args[1], (BlockingCollection<ICommand>)args[2]);
                threadCollection.Add((Guid)args[1], (ServerThread)args[3]);
                IdServersAndThreads.Add((Guid)args[0], [(Guid)args[1]]);
            });
        }).Execute();

        IoC.Resolve<ICommand>("IoC.Register", "Server.Command.GetThreadQueue", (object[] args) =>
        {
            return queueCollection[(Guid)args[0]];
        }).Execute();

        IoC.Resolve<ICommand>("IoC.Register", "Server.Command.GetThreadId", (object[] args) =>
        {
            return IdServersAndThreads[(Guid)args[0]];
        }).Execute();

        IoC.Resolve<ICommand>("IoC.Register", "Server.Commands.CreateStartThread", (object[] args) =>
        {
            var server_id = (Guid)args[0];
            var thread_id = (Guid)args[1];
            Action action;
            if (args.Count() == 1)
            {
                action = (() => { });
            }
            else
            {
                action = (Action)args[2];
            }

            return new ActionCommand(() =>
            {
                var q = new BlockingCollection<ICommand>(100);
                var t = new ServerThread(q);
                t.Start();
                IoC.Resolve<ICommand>("Server.Commands.RegisterThread", server_id, thread_id, q, t).Execute();
                action();
            });
        }).Execute();

        IoC.Resolve<ICommand>("IoC.Register", "Server.Commands.SendCommand", (object[] args) =>
            {
                var id = (Guid)args[0];
                var cmd = (ICommand)args[1];
                var q = IoC.Resolve<BlockingCollection<ICommand>>("Server.Command.GetThreadQueue", id);
                return new ActionCommand(() =>
                    {
                        q.Add(cmd);
                    });
            }).Execute();
        IoC.Resolve<ICommand>("IoC.Register", "GetQueueCollection", (object[] args) => { return queueCollection; }).Execute();
        IoC.Resolve<ICommand>("IoC.Register", "GetThreadCollection", (object[] args) => { return threadCollection; }).Execute();
        IoC.Resolve<ICommand>("IoC.Register", "GetThreadsId", (object[] args) => { return IdServersAndThreads; }).Execute();

        IoC.Resolve<ICommand>("IoC.Register", "Server.Command.CreatResponse",(object[] args) =>
        {

         if (args[1] == null){
            throw new ArgumentException();
            
         }
         else{
             if (args[2] == null){
                throw new IndexOutOfRangeException();
             }
             else{
                return "Code 202 - Accepted " + args[0];
             }
         }
        }).Execute();
    }


    [Fact]

    public void EndPoint_correct_work()
    {
        var ServerId = Guid.NewGuid();
        var ThreadId = Guid.NewGuid();

        var OrdersList = new List<OrderContract>()
            {
                new()
                {
                  OrderType = "start movement",
                  GameId = ServerId,
                  ObjectId = "1",
                  Properties = new(){{"Velocity", 1}}
                },

                new()
                {
                  OrderType = "start rotatement",
                  GameId = ServerId,
                  ObjectId = "1",
                  Properties = new(){{"Angle_Velocity", 1}}
                },

                new()
                {
                  OrderType = "stop",
                  GameId = ServerId,
                  ObjectId= "1",
                },

                new()
                {
                  OrderType = "fire",
                  GameId = ServerId,
                  ObjectId= "1",
                }
            };
        var CreatOrderCmd = new Mock<ICommand>();
        CreatOrderCmd.Setup(cmd => cmd.Execute()).Verifiable();
        var StartThreadCMD = new Mock<ICommand>();
        StartThreadCMD.Setup(cmd => cmd.Execute()).Verifiable();
        IoC.Resolve<ICommand>("Server.Commands.CreateStartThread", ServerId, ThreadId, () => { StartThreadCMD.Object.Execute(); }).Execute();
        IoC.Resolve<ICommand>("IoC.Register", "CreatOrderCmd", (object[] args) =>
        {
            return CreatOrderCmd.Object;
        }).Execute();

        var webApi = new WebApi();

        var response = webApi.PostOrder(OrdersList[0]);
        OrdersList.ForEach(order => webApi.PostOrder(order));

        Assert.Equal("Code 202 - Accepted " + ServerId, response);

        StartThreadCMD.Verify(cmd => cmd.Execute(), Times.Once());
        CreatOrderCmd.Verify(cmd => cmd.Execute(), Times.Exactly(5));
    }

    [Fact]

    public void Absence_of_Id_dont_drop_exception()
    {
        var ServerId = Guid.NewGuid();
        var ThreadId = Guid.NewGuid();

        var OrdersList = new List<OrderContract>()
            {
                new()
                {
                  OrderType = "start movement",
                  GameId = Guid.NewGuid(),
                  ObjectId = "1",
                  Properties = new(){{"Velocity", 1}}
                },

                new()
                {
                  OrderType = "start rotatement",
                  GameId = ServerId,
                  ObjectId = "1",
                  Properties = new(){{"Angle_Velocity", 1}}
                }
            };
        var CreatOrderCmd = new Mock<ICommand>();
        CreatOrderCmd.Setup(cmd => cmd.Execute()).Verifiable();
        var StartThreadCMD = new Mock<ICommand>();
        StartThreadCMD.Setup(cmd => cmd.Execute()).Verifiable();
        IoC.Resolve<ICommand>("Server.Commands.CreateStartThread", ServerId, ThreadId, () => { StartThreadCMD.Object.Execute(); }).Execute();
        IoC.Resolve<ICommand>("IoC.Register", "CreatOrderCmd", (object[] args) =>
        {
            return CreatOrderCmd.Object;
        }).Execute();

        var webApi = new WebApi();

        var response1 = webApi.PostOrder(OrdersList[0]);
        var response2 = webApi.PostOrder(OrdersList[1]);

        Assert.Equal("Code 400 - Entered GameId don't exist", response1);
        Assert.Equal("Code 202 - Accepted " + ServerId, response2);

        StartThreadCMD.Verify(cmd => cmd.Execute(), Times.Once());
        CreatOrderCmd.Verify(cmd => cmd.Execute(), Times.Once());
    }

    [Fact]

    public void Absence_of_OrderType_dont_drop_exception()
    {
        var ServerId = Guid.NewGuid();
        var ThreadId = Guid.NewGuid();

        var OrdersList = new List<OrderContract>()
            {
                new()
                {
                  OrderType = null,
                  GameId = ServerId,
                  ObjectId = "1",
                  Properties = new(){{"Velocity", 1}}
                },

                new()
                {
                  OrderType = "start rotatement",
                  GameId = ServerId,
                  ObjectId = "1",
                  Properties = new(){{"Angle_Velocity", 1}}
                }
            };

        var CreatOrderCmd = new Mock<ICommand>();
        CreatOrderCmd.Setup(cmd => cmd.Execute()).Verifiable();
        var StartThreadCMD = new Mock<ICommand>();
        StartThreadCMD.Setup(cmd => cmd.Execute()).Verifiable();
        IoC.Resolve<ICommand>("Server.Commands.CreateStartThread", ServerId, ThreadId, () => { StartThreadCMD.Object.Execute(); }).Execute();
        IoC.Resolve<ICommand>("IoC.Register", "CreatOrderCmd", (object[] args) =>
        {
            return CreatOrderCmd.Object;
        }).Execute();

        var webApi = new WebApi();

        var response1 = webApi.PostOrder(OrdersList[0]);
        var response2 = webApi.PostOrder(OrdersList[1]);

        Assert.Equal("Code 400 - Don't have OrderType", response1);
        Assert.Equal("Code 202 - Accepted " + ServerId, response2);

        StartThreadCMD.Verify(cmd => cmd.Execute(), Times.Once());
        CreatOrderCmd.Verify(cmd => cmd.Execute(), Times.Once());
    }

    [Fact]

    public void Absence_of_ObjectId_dont_drop_exception()
    {
        var ServerId = Guid.NewGuid();
        var ThreadId = Guid.NewGuid();

        var OrdersList = new List<OrderContract>()
            {
                new()
                {
                  OrderType = "start move",
                  GameId = ServerId,
                  ObjectId = null,
                  Properties = new(){{"Velocity", 1}}
                },

                new()
                {
                  OrderType = "start rotatement",
                  GameId = ServerId,
                  ObjectId = "1",
                  Properties = new(){{"Angle_Velocity", 1}}
                }
            };

        var CreatOrderCmd = new Mock<ICommand>();
        CreatOrderCmd.Setup(cmd => cmd.Execute()).Verifiable();
        var StartThreadCMD = new Mock<ICommand>();
        StartThreadCMD.Setup(cmd => cmd.Execute()).Verifiable();
        IoC.Resolve<ICommand>("Server.Commands.CreateStartThread", ServerId, ThreadId, () => { StartThreadCMD.Object.Execute(); }).Execute();
        IoC.Resolve<ICommand>("IoC.Register", "CreatOrderCmd", (object[] args) =>
        {
            return CreatOrderCmd.Object;
        }).Execute();

        var webApi = new WebApi();

        var response1 = webApi.PostOrder(OrdersList[0]);
        var response2 = webApi.PostOrder(OrdersList[1]);

        Assert.Equal("Code 400 - Entered ObjectId don't exist", response1);
        Assert.Equal("Code 202 - Accepted " + ServerId, response2);

        StartThreadCMD.Verify(cmd => cmd.Execute(), Times.Once());
        CreatOrderCmd.Verify(cmd => cmd.Execute(), Times.Once);
    }


    [Fact]

    public void EndPoint_works_in_currient_thread()
    {
        var ServerId1 = Guid.NewGuid();
        var ThreadId1 = Guid.NewGuid();
        var ServerId2 = Guid.NewGuid();
        var ThreadId2 = Guid.NewGuid();

        var OrdersList = new List<OrderContract>()
            {
                new()
                {
                  OrderType = "start movement",
                  GameId = ServerId1,
                  ObjectId = "1",
                  Properties = new(){{"Velocity", 1}}
                },

                new()
                {
                  OrderType = "start rotatement",
                  GameId = ServerId2,
                  ObjectId = "1",
                  Properties = new(){{"Angle_Velocity", 1}}
                },

                new()
                {
                  OrderType = "stop",
                  GameId = ServerId1,
                  ObjectId= "1",
                },

                new()
                {
                  OrderType = "fire",
                  GameId = ServerId2,
                  ObjectId= "1",
                }
            };
        var CreatOrderCmd = new Mock<ICommand>();
        CreatOrderCmd.Setup(cmd => cmd.Execute()).Verifiable();
        var StartThreadCMD1 = new Mock<ICommand>();
        StartThreadCMD1.Setup(cmd => cmd.Execute()).Verifiable();
        var StartThreadCMD2 = new Mock<ICommand>();
        StartThreadCMD2.Setup(cmd => cmd.Execute()).Verifiable();

        IoC.Resolve<ICommand>("Server.Commands.CreateStartThread", ServerId1, ThreadId1, () => { StartThreadCMD1.Object.Execute(); }).Execute();
        IoC.Resolve<ICommand>("Server.Commands.CreateStartThread", ServerId2, ThreadId2, () => { StartThreadCMD2.Object.Execute(); }).Execute();
        IoC.Resolve<ICommand>("IoC.Register", "CreatOrderCmd", (object[] args) =>
        {
            return CreatOrderCmd.Object;
        }).Execute();

        var webApi = new WebApi();

        var response1 = webApi.PostOrder(OrdersList[0]);
        var response2 = webApi.PostOrder(OrdersList[1]);
        OrdersList.ForEach(order => webApi.PostOrder(order));

        Assert.Equal("Code 202 - Accepted " + ServerId1, response1);
        Assert.Equal("Code 202 - Accepted " + ServerId2, response2);

        Assert.True(IoC.Resolve<Dictionary<Guid, BlockingCollection<ICommand>>>("GetQueueCollection").Count() == 2);
        StartThreadCMD1.Verify(cmd => cmd.Execute(), Times.Once());
        StartThreadCMD2.Verify(cmd => cmd.Execute(), Times.Once());
        CreatOrderCmd.Verify(cmd => cmd.Execute(), Times.Exactly(6));
    }
}
