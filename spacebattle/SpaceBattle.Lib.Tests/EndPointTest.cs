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
        var IdServersAndThreads = new Dictionary<Guid, Guid>();

        IoC.Resolve<ICommand>("IoC.Register", "Server.Commands.GetThreadQueue", (object[] args) =>
        {
            return queueCollection[(Guid)args[0]];
        }).Execute();

        IoC.Resolve<ICommand>("IoC.Register", "Server.Commands.TryGetServerIdByGameId", (object[] args) =>
        {
            return (object)IdServersAndThreads[(Guid)args[0]];
        }).Execute();

        IoC.Resolve<ICommand>("IoC.Register", "Server.Commands.AddThread", (object[] args) =>
        {
            return new ActionCommand(() =>
            {
                IdServersAndThreads.Add((Guid)args[0], (Guid)args[1]);
                queueCollection.Add((Guid)args[1], new BlockingCollection<ICommand>(10));
            });
        }).Execute();

        IoC.Resolve<ICommand>("IoC.Register", "Server.Commands.SendCommand", (object[] args) =>
            {
                var id = (Guid)args[0];
                var cmd = (ICommand)args[1];
                var q = IoC.Resolve<BlockingCollection<ICommand>>("Server.Commands.GetThreadQueue", id);
                return new ActionCommand(() =>
                    {
                        q.Add(cmd);
                        q.Take().Execute();
                    });
            }).Execute();

        IoC.Resolve<ICommand>("IoC.Register", "GetQueueCollection", (object[] args) => { return queueCollection; }).Execute();
        IoC.Resolve<ICommand>("IoC.Register", "GetThreadsId", (object[] args) => { return IdServersAndThreads; }).Execute();
    }


    [Fact]

    public void EndPoint_correct_work()
    {
        var ServerId = Guid.NewGuid();
        var ThreadId = Guid.NewGuid();

        IoC.Resolve<ICommand>("Server.Commands.AddThread", ServerId, ThreadId).Execute();

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

        IoC.Resolve<ICommand>("IoC.Register", "CreateOrderCmd", (object[] args) =>
        {
            return CreatOrderCmd.Object;
        }).Execute();

        var webApi = new WebApi();

        var response = webApi.PostOrder(OrdersList[0]);
        OrdersList.ForEach(order => webApi.PostOrder(order));

        Assert.Equal("Code 202 - Accepted", response);
        CreatOrderCmd.Verify(cmd => cmd.Execute(), Times.Exactly(5));
    }

    [Fact]

    public void Absence_of_Id_dont_drop_exception()
    {
        var ServerId = Guid.NewGuid();
        var ThreadId = Guid.NewGuid();

        IoC.Resolve<ICommand>("Server.Commands.AddThread", ServerId, ThreadId).Execute();

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

        IoC.Resolve<ICommand>("IoC.Register", "CreateOrderCmd", (object[] args) =>
         {
             return CreatOrderCmd.Object;
         }).Execute();

        var webApi = new WebApi();

        var response1 = webApi.PostOrder(OrdersList[0]);
        var response2 = webApi.PostOrder(OrdersList[1]);

        Assert.Equal("Code 400 - Bad input", response1);
        Assert.Equal("Code 202 - Accepted", response2);

        CreatOrderCmd.Verify(cmd => cmd.Execute(), Times.Once());
    }

    [Fact]

    public void Absence_of_OrderType_dont_drop_exception()
    {
        var ServerId = Guid.NewGuid();
        var ThreadId = Guid.NewGuid();

        IoC.Resolve<ICommand>("Server.Commands.AddThread", ServerId, ThreadId).Execute();

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
        IoC.Resolve<ICommand>("IoC.Register", "CreateOrderCmd", (object[] args) =>
        {
            return CreatOrderCmd.Object;
        }).Execute();

        var webApi = new WebApi();

        var response1 = webApi.PostOrder(OrdersList[0]);
        var response2 = webApi.PostOrder(OrdersList[1]);

        Assert.Equal("Code 400 - Bad input", response1);
        Assert.Equal("Code 202 - Accepted", response2);

        CreatOrderCmd.Verify(cmd => cmd.Execute(), Times.Once());
    }

    [Fact]

    public void Absence_of_ObjectId_dont_drop_exception()
    {
        var ServerId = Guid.NewGuid();
        var ThreadId = Guid.NewGuid();

        IoC.Resolve<ICommand>("Server.Commands.AddThread", ServerId, ThreadId).Execute();

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
        IoC.Resolve<ICommand>("IoC.Register", "CreateOrderCmd", (object[] args) =>
        {
            return CreatOrderCmd.Object;
        }).Execute();

        var webApi = new WebApi();

        var response1 = webApi.PostOrder(OrdersList[0]);
        var response2 = webApi.PostOrder(OrdersList[1]);

        Assert.Equal("Code 400 - Bad input", response1);
        Assert.Equal("Code 202 - Accepted", response2);

        CreatOrderCmd.Verify(cmd => cmd.Execute(), Times.Once);
    }


    [Fact]

    public void EndPoint_works_in_currient_thread()
    {
        var ServerId1 = Guid.NewGuid();
        var ThreadId1 = Guid.NewGuid();
        var ServerId2 = Guid.NewGuid();
        var ThreadId2 = Guid.NewGuid();

        IoC.Resolve<ICommand>("Server.Commands.AddThread", ServerId1, ThreadId1).Execute();
        IoC.Resolve<ICommand>("Server.Commands.AddThread", ServerId2, ThreadId2).Execute();

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

        IoC.Resolve<ICommand>("IoC.Register", "CreateOrderCmd", (object[] args) =>
        {
            return CreatOrderCmd.Object;
        }).Execute();

        var webApi = new WebApi();

        var response1 = webApi.PostOrder(OrdersList[0]);
        var response2 = webApi.PostOrder(OrdersList[1]);
        OrdersList.ForEach(order => webApi.PostOrder(order));

        Assert.Equal("Code 202 - Accepted", response1);
        Assert.Equal("Code 202 - Accepted", response2);

        Assert.True(IoC.Resolve<Dictionary<Guid, BlockingCollection<ICommand>>>("GetQueueCollection").Count() == 2);
        CreatOrderCmd.Verify(cmd => cmd.Execute(), Times.Exactly(6));
    }
}
