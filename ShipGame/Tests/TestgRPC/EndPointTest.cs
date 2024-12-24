using Hwdtech;
using Moq;
using System.Collections.Concurrent;
using ICommand = ShipGame.Move.ICommand;
using gRPC.EndPointRouter;
using ShipGame.Server;
using ShipGame.Game;
using ShipGame.Move;
using gRPC.Router;
using gRPC.Services;
using Microsoft.Extensions.Logging;
using gRPC;
using Grpc.Core;
using SpaceBattle.ServerStrategies;

namespace SpaceBattle.Lib.Test;

public class EndpointTest
{
    [Test]
    public void PositiveRoutingTest()
    {
        new Hwdtech.Ioc.InitScopeBasedIoCImplementationCommand().Execute();
        Hwdtech.IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", Hwdtech.IoC.Resolve<object>("Scopes.New", Hwdtech.IoC.Resolve<object>("Scopes.Root"))).Execute();

        var threadDict = new ConcurrentDictionary<string, ServerThread>();
        var gamesThreadsDictionary = new ConcurrentDictionary<string, string>();
        var senderDict = new ConcurrentDictionary<string, ISender>();
        var senderOrderDict = new ConcurrentDictionary<string, ISender>();
        var gamesDictionary = new Dictionary<string, Queue<ICommand>>();
        Queue<ICommand> gameQueue1 = new Queue<ICommand>();
        gamesDictionary.TryAdd("1", gameQueue1);
        Queue<ICommand> gameQueue2 = new Queue<ICommand>();
        gamesDictionary.TryAdd("2", gameQueue2);
        ICommand gameCommand1 = new GameCommand("1", gameQueue1);
        ICommand gameCommand2 = new GameCommand("2", gameQueue2);
        gamesThreadsDictionary.TryAdd("1", "80");
        gamesThreadsDictionary.TryAdd("2", "80");

        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "ThreadDictionary", (object[] _) => threadDict).Execute();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "SenderDictionary", (object[] _) => senderDict).Execute();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "SenderOrderDictionary", (object[] _) => senderOrderDict).Execute();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "ServerThreadGetByID", (object[] id) => threadDict[(string)id[0]]).Execute();

        var createWithStartThreadStrategy = new CreateWithStartThreadStrategy();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "CreateWithStartThread", (object[] args) => createWithStartThreadStrategy.RunStrategy(args)).Execute();
        Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "QuantumForGame", (object[] _) => (object)new TimeSpan(0, 0, 0, 0, 150)).Execute();
        var command1 = new Mock<ShipGame.Move.ICommand>();
            var regStrategy1 = new Mock<IStrategy>();
            command1.Setup(_command => _command.Execute());
            regStrategy1.Setup(_strategy => _strategy.RunStrategy(It.IsAny<object[]>())).Returns(command1.Object);

        BlockingCollection<ICommand> queue = new();
        BlockingCollection<ICommand> orderQueue = new();
        ISender orderSender = new SenderAdapter(orderQueue);
        IReceiver orderReceiver = new ReceiverAdapter(orderQueue);
        ISender sender = new SenderAdapter(queue);
        IReceiver receiver = new ReceiverAdapter(queue);

        var mre0 = new ManualResetEvent(false);
        var mre1 = new ManualResetEvent(false);

        var valueMap = new Google.Protobuf.Collections.MapField<string, string>(){{"type", "Move"},{"objid", "uobj1"},{"velocity", "5"}};
        OrderRequest orderRequest = new()
        {
            GameId = "2"
        };
        orderRequest.Map.Add(valueMap);
        
        var MockCommand = new Mock<ICommand>();
        MockCommand.Setup(x => x.Execute());
                            ICommand commandForGame = new ActionCommand(()=>{
                        MockCommand.Object.Execute();
        });
        IEndPointRouter router = new EndPointRouter(gamesThreadsDictionary, senderOrderDict);
        Action act1 = () => {
                Hwdtech.IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", Hwdtech.IoC.Resolve<object>("Scopes.New", Hwdtech.IoC.Resolve<object>("Scopes.Root"))).Execute();
        Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "OrderDictionaryToICommand", (object[] args) => commandForGame).Execute();
                        Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "SendCommandToGame", (object[] args) => new ActionCommand(()=>gameQueue2.Append((ICommand)args[1]))).Execute();
                                    IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "HandleException", (object[] args) => regStrategy1.Object.RunStrategy(args)).Execute();
                mre0.Set();
            };
        var ST = new ServerThread(receiver, orderReceiver);
        senderDict.TryAdd("80", sender);
        threadDict.TryAdd("80", ST);
        senderOrderDict.TryAdd("80", orderSender);
        sender.Send(new ActionCommand(act1));
        ST.Start();
        
        mre0.WaitOne();
        router.route(orderRequest);
        sender.Send(gameCommand1);
        sender.Send(gameCommand2);
        sender.Send((ICommand)new HardStopCommandStrategy().RunStrategy("80", ()=>{mre1.Set();}));
        mre1.WaitOne();
        Assert.IsEmpty(orderQueue);
        Assert.IsEmpty(gameQueue2);
        MockCommand.Verify();
    }

    public void NegativeRoutingTest()
    {
        new Hwdtech.Ioc.InitScopeBasedIoCImplementationCommand().Execute();
        Hwdtech.IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", Hwdtech.IoC.Resolve<object>("Scopes.New", Hwdtech.IoC.Resolve<object>("Scopes.Root"))).Execute();

        var threadDict = new ConcurrentDictionary<string, ServerThread>();
        var gamesThreadsDictionary = new ConcurrentDictionary<string, string>();
        var senderDict = new ConcurrentDictionary<string, ISender>();
        var senderOrderDict = new ConcurrentDictionary<string, ISender>();
        var gamesDictionary = new Dictionary<string, Queue<ICommand>>();
        Queue<ICommand> gameQueue1 = new Queue<ICommand>();
        gamesDictionary.TryAdd("1", gameQueue1);
        Queue<ICommand> gameQueue2 = new Queue<ICommand>();
        gamesDictionary.TryAdd("2", gameQueue2);
        ICommand gameCommand1 = new GameCommand("1", gameQueue1);
        ICommand gameCommand2 = new GameCommand("2", gameQueue2);
        gamesThreadsDictionary.TryAdd("1", "80");
        gamesThreadsDictionary.TryAdd("2", "80");

        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "ThreadDictionary", (object[] _) => threadDict).Execute();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "SenderDictionary", (object[] _) => senderDict).Execute();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "SenderOrderDictionary", (object[] _) => senderOrderDict).Execute();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "ServerThreadGetByID", (object[] id) => threadDict[(string)id[0]]).Execute();

        Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "QuantumForGame", (object[] _) => (object)new TimeSpan(0, 0, 0, 0, 150)).Execute();
        var command1 = new Mock<ShipGame.Move.ICommand>();
        var regStrategy1 = new Mock<IStrategy>();
        command1.Setup(_command => _command.Execute());
        regStrategy1.Setup(_strategy => _strategy.RunStrategy(It.IsAny<object[]>())).Returns(command1.Object);

        var mre0 = new ManualResetEvent(false);
        var mre1 = new ManualResetEvent(false);

        BlockingCollection<ICommand> queue = new();
        BlockingCollection<ICommand> orderQueue = new();
        ISender orderSender = new SenderAdapter(orderQueue);
        IReceiver orderReceiver = new ReceiverAdapter(orderQueue);
        ISender sender = new SenderAdapter(queue);
        IReceiver receiver = new ReceiverAdapter(queue);

        var valueMap = new Google.Protobuf.Collections.MapField<string, string>(){{"type", "Move"},{"objid", "uobj1"},{"velocity", "5"}};
        OrderRequest orderRequest = new()
        {
            GameId = "2"
        };
        orderRequest.Map.Add(valueMap);
        
        var MockCommand = new Mock<ICommand>();
        MockCommand.Setup(x => x.Execute());
        ICommand commandForGame = new ActionCommand(()=>{
            MockCommand.Object.Execute();
        });
        IEndPointRouter router = new EndPointRouter(gamesThreadsDictionary, senderOrderDict);
        Action act1 = () => {
            IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))).Execute();
            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "OrderDictionaryToICommand", (object[] args) => commandForGame).Execute();
            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "SendCommandToGame", (object[] args) => new ActionCommand(()=>gameQueue2.Append((ICommand)args[1]))).Execute();
            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "HandleException", (object[] args) => regStrategy1.Object.RunStrategy(args)).Execute();
            mre0.Set();
        };
        var ST = new ServerThread(receiver, orderReceiver);
        senderDict.TryAdd("80", sender);
        threadDict.TryAdd("80", ST);
        senderOrderDict.TryAdd("80", orderSender);
        sender.Send(new ActionCommand(act1));
        ST.Start();
        
        mre0.WaitOne();
        router.route(orderRequest);
        sender.Send(gameCommand1);
        sender.Send(gameCommand2);
        sender.Send((ICommand)new HardStopCommandStrategy().RunStrategy("80", ()=>{mre1.Set();}));
        mre1.WaitOne();
        Assert.IsEmpty(orderQueue);
        Assert.IsEmpty(gameQueue2);
        MockCommand.Verify();
    }

}