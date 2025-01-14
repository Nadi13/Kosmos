using System.Collections.Concurrent;
using ShipGame.Game;
using ICommand = ShipGame.Move.ICommand;
using ShipGame.Server;
using Hwdtech;
using SpaceBattle.ServerStrategies;
using Moq;
using ShipGame.Move;
using gRPC;
using GRpc.Server;

namespace SpaceBattle.Lib.Test;

public class EndpointTest
{
    [Test]
    public void PositiveRoutingTest()
    {
        new Hwdtech.Ioc.InitScopeBasedIoCImplementationCommand().Execute();
        Hwdtech.IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", Hwdtech.IoC.Resolve<object>("Scopes.New", Hwdtech.IoC.Resolve<object>("Scopes.Root"))).Execute();

        var threadDict = new ConcurrentDictionary<string, ServerThread>();
        var gamesThreadDict = new ConcurrentDictionary<string, string>();
        var senderDict = new ConcurrentDictionary<string, ISender>();
        var externalSenderDict = new ConcurrentDictionary<string, ISender>();
        var gamesDict = new Dictionary<string, Queue<ICommand>>();
        Queue<ICommand> gameQueue1 = new Queue<ICommand>();
        gamesDict.TryAdd("game1", gameQueue1);
        Queue<ICommand> gameQueue2 = new Queue<ICommand>();
        gamesDict.TryAdd("game2", gameQueue2);
        ICommand gameCommand1 = new GameCommand("game1", gameQueue1);
        ICommand gameCommand2 = new GameCommand("game2", gameQueue2);
        gamesThreadDict.TryAdd("game2", "80");

        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "ServerThreadGetByID", (object[] id) => threadDict[(string)id[0]]).Execute();

        var command1 = new Mock<ShipGame.Move.ICommand>();
        var regStrategy1 = new Mock<IStrategy>();
        command1.Setup(_command => _command.Execute());
        regStrategy1.Setup(_strategy => _strategy.RunStrategy(It.IsAny<object[]>())).Returns(command1.Object);

        BlockingCollection<ICommand> queue = new();
        BlockingCollection<ICommand> externalQueue = new();
        ISender externalSender = new SenderAdapter(externalQueue);
        IReceiver externalReceiver = new ReceiverAdapter(externalQueue);
        ISender sender = new SenderAdapter(queue);
        IReceiver receiver = new ReceiverAdapter(queue);

        var mre0 = new ManualResetEvent(false);
        var mre1 = new ManualResetEvent(false);

        var valueMap = new Google.Protobuf.Collections.MapField<string, string>(){{"type", "Move"},{"objid", "uobj1"},{"velocity", "5"}};
        ExternalCommandRequest externalCommandRequest = new()
        {
            GameId = "game2"
        };
        externalCommandRequest.Map.Add(valueMap);

        var MockCommand = new Mock<ICommand>();
        MockCommand.Setup(x => x.Execute());
        ICommand commandForGame = new ActionCommand(()=>{
            MockCommand.Object.Execute();
        });
        IEndPointRouter router = new RouterMessage(gamesThreadDict, externalSenderDict);
        Action act1 = () => {
                Hwdtech.IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", Hwdtech.IoC.Resolve<object>("Scopes.New", Hwdtech.IoC.Resolve<object>("Scopes.Root"))).Execute();
                Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "messageToICommand", (object[] args) => commandForGame).Execute();
                Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "GameQueueReceiveCommand", (object[] args) => new ActionCommand(()=>gamesDict[(string)args[0]].Append((ShipGame.Move.ICommand)args[1]))).Execute();
                Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "QuantumForGame", (object[] _) => (object)new TimeSpan(0, 0, 0, 0, 150)).Execute();
                mre0.Set();
            };
        var ST = new ServerThread(receiver, externalReceiver);
        senderDict.TryAdd("80", sender);
        threadDict.TryAdd("80", ST);
        externalSenderDict.TryAdd("80", externalSender);
        sender.Send(new ActionCommand(act1));
        ST.Start();

        mre0.WaitOne();
        var result = router.isSent(externalCommandRequest);
        Assert.IsTrue(result);
        sender.Send(gameCommand1);
        sender.Send(gameCommand2);
        sender.Send((ICommand)new HardStopCommandStrategy().RunStrategy("80", ()=>{mre1.Set();}));
        mre1.WaitOne();
        Assert.IsEmpty(externalQueue);
        Assert.IsEmpty(gameQueue2);
        MockCommand.Verify();
    }

    [Test]
    public void NegativeRoutingTest()
    {
        new Hwdtech.Ioc.InitScopeBasedIoCImplementationCommand().Execute();
        Hwdtech.IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", Hwdtech.IoC.Resolve<object>("Scopes.New", Hwdtech.IoC.Resolve<object>("Scopes.Root"))).Execute();

        var threadDict = new ConcurrentDictionary<string, ServerThread>();
        var gamesThreadDict = new ConcurrentDictionary<string, string>();
        var senderDict = new ConcurrentDictionary<string, ISender>();
        var externalSenderDict = new ConcurrentDictionary<string, ISender>();
        var gamesDict = new Dictionary<string, Queue<ICommand>>();
        Queue<ICommand> gameQueue1 = new Queue<ICommand>();
        gamesDict.TryAdd("game1", gameQueue1);
        Queue<ICommand> gameQueue2 = new Queue<ICommand>();
        gamesDict.TryAdd("game2", gameQueue2);
        ICommand gameCommand1 = new GameCommand("game1", gameQueue1);
        ICommand gameCommand2 = new GameCommand("game2", gameQueue2);
        gamesThreadDict.TryAdd("game1", "80");
        gamesThreadDict.TryAdd("game2", "80");

        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "ServerThreadGetByID", (object[] id) => threadDict[(string)id[0]]).Execute();

        var command1 = new Mock<ShipGame.Move.ICommand>();
        var regStrategy1 = new Mock<IStrategy>();
        command1.Setup(_command => _command.Execute());
        regStrategy1.Setup(_strategy => _strategy.RunStrategy(It.IsAny<object[]>())).Returns(command1.Object);

        var mre0 = new ManualResetEvent(false);
        var mre1 = new ManualResetEvent(false);

        BlockingCollection<ICommand> queue = new();
        BlockingCollection<ICommand> externalQueue = new();
        ISender externalSender = new SenderAdapter(externalQueue);
        IReceiver externalReceiver = new ReceiverAdapter(externalQueue);
        ISender sender = new SenderAdapter(queue);
        IReceiver receiver = new ReceiverAdapter(queue);

        var valueMap = new Google.Protobuf.Collections.MapField<string, string>(){{"type", "Move"},{"objid", "uobj1"},{"velocity", "5"}};
        ExternalCommandRequest externalCommandRequest = new()
        {
            GameId = "game3"
        };
        externalCommandRequest.Map.Add(valueMap);

        var MockCommand = new Mock<ICommand>();
        MockCommand.Setup(x => x.Execute());
        ICommand commandForGame = new ActionCommand(()=>{
            MockCommand.Object.Execute();
        });
        IEndPointRouter router = new RouterMessage(gamesThreadDict, externalSenderDict);
        Action act1 = () => {
            IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))).Execute();
            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "messageToICommand", (object[] args) => commandForGame).Execute();
            Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "GameQueueReceiveCommand", (object[] args) => new ActionCommand(()=>gamesDict[(string)args[0]].Append((ShipGame.Move.ICommand)args[1]))).Execute();
            Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "QuantumForGame", (object[] _) => (object)new TimeSpan(0, 0, 0, 0, 150)).Execute();
            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "HandleException", (object[] args) => regStrategy1.Object.RunStrategy(args)).Execute();
            mre0.Set();
        };
        var ST = new ServerThread(receiver, externalReceiver);
        senderDict.TryAdd("80", sender);
        threadDict.TryAdd("80", ST);
        externalSenderDict.TryAdd("80", externalSender);
        sender.Send(new ActionCommand(act1));
        ST.Start();

        mre0.WaitOne();
        var result = router.isSent(externalCommandRequest);
        Assert.IsFalse(result);
        Assert.IsEmpty(externalQueue);
        Assert.IsEmpty(gameQueue1);
        Assert.IsEmpty(gameQueue2);
        sender.Send(gameCommand1);
        sender.Send(gameCommand2);
        sender.Send((ICommand)new HardStopCommandStrategy().RunStrategy("80", ()=>{mre1.Set();}));
        mre1.WaitOne();
    }

}