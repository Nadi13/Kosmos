using System.Collections.Concurrent;
using gRPC;
using gRPC.EndPointRouter;
using gRPC.Router;
using Moq;
using ShipGame.Game;
using ShipGame.Move;
using ShipGame.Server;
using SpaceBattle.ServerStrategies;

namespace TestgRPC;

public class MigrateTest
{
    [Test]
    public void PositiveSerializeGame()
    {
        new Hwdtech.Ioc.InitScopeBasedIoCImplementationCommand().Execute();
        Hwdtech.IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", Hwdtech.IoC.Resolve<object>("Scopes.New", Hwdtech.IoC.Resolve<object>("Scopes.Root"))).Execute();

        var threadDict = new ConcurrentDictionary<string, ServerThread>();
        var gamesThreadsDictionary = new ConcurrentDictionary<string, string>();
        var senderDict = new ConcurrentDictionary<string, ISender>();
        var orderSenderDict = new ConcurrentDictionary<string, ISender>();
        var gamesDictionary = new Dictionary<string, Queue<ICommand>>();
        IEndPointRouter router = new EndPointRouter(gamesThreadsDictionary, orderSenderDict);

        Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "ThreadDictionary", (object[] _) => threadDict).Execute();
        Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "SenderDictionary", (object[] _) => senderDict).Execute();
        Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "SenderOrderDictionary", (object[] _) => orderSenderDict).Execute();
        Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "ServerThreadGetByID", (object[] id) => threadDict[(string)id[0]]).Execute();

        List<string> gameOptions = new()
        {
            "free_for_all",
            "happy_new_year_event"
        };
        Dictionary<string, object> gameObjects = new()
        {
            {"helmet-01", new Mock<IUObject>()},
            {"space_station", new Mock<IUObject>()},
            {"helmet-02", new Mock<IUObject>()},
            {"IOC-shark", new Mock<IUObject>()},
            {"IOC-bee", new Mock<IUObject>()},
        };

        Queue<ICommand> gameQueue1 = new Queue<ICommand>();
        gameQueue1.Enqueue(new ActionCommand(()=>{}));
        gamesDictionary.TryAdd("1", gameQueue1);
        ICommand gameCommand1 = new GameCommand("1", gameQueue1);
        gamesThreadsDictionary.TryAdd("1", "21");

        var stopGameCommandExecutingStrategy = new Mock<IStrategy>();
        stopGameCommandExecutingStrategy.Setup(_strategy => _strategy.RunStrategy()).Returns(new ActionCommand(()=>{})).Verifiable();


        var gameOptionsGetAllStrategy = new Mock<IStrategy>();
        gameOptionsGetAllStrategy.Setup(_strategy => _strategy.RunStrategy(It.IsAny<object[]>())).Returns(gameOptions).Verifiable();


        var gameObjectsGetAllStrategy = new Mock<IStrategy>();
        gameObjectsGetAllStrategy.Setup(_strategy => _strategy.RunStrategy(It.IsAny<object[]>())).Returns(gameObjects).Verifiable();


        var gameQueueGetStrategy = new Mock<IStrategy>();
        gameQueueGetStrategy.Setup(_strategy => _strategy.RunStrategy(It.IsAny<object[]>())).Returns(gameQueue1).Verifiable();


        var stringfyOptionStrategy = new Mock<IStrategy>();
        stringfyOptionStrategy.Setup(_strategy => _strategy.RunStrategy(It.IsAny<object[]>())).Returns("GameOption_").Verifiable();


        var stringfyObjectStrategy = new Mock<IStrategy>();
        stringfyOptionStrategy.Setup(_strategy => _strategy.RunStrategy(It.IsAny<object[]>())).Returns("Object_").Verifiable();


        var serializeCommandStrategy = new Mock<IStrategy>();
        serializeCommandStrategy.Setup(_strategy => _strategy.RunStrategy(It.IsAny<object[]>())).Returns("ICommand_type_").Verifiable();

        var command1 = new Mock<ICommand>();
        var command2 = new Mock<ICommand>();
        var regStrategy1 = new Mock<IStrategy>();
        command1.Setup(_command => _command.Execute());
        command2.Setup(_command => _command.Execute());
        regStrategy1.Setup(_strategy => _strategy.RunStrategy(It.IsAny<object[]>())).Returns(command1.Object);

        var createAndStartThreadStrategy = new CreateWithStartThreadStrategy();
        Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "CreateWithStartThread", (object[] args) => createAndStartThreadStrategy.RunStrategy(args)).Execute();
        Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "QuantumForGame", (object[] _) => (object)new TimeSpan(0, 0, 0, 0, 200)).Execute();

        BlockingCollection<ICommand> queue = new();
        BlockingCollection<ICommand> orderQueue = new();
        ISender orderSender = new SenderAdapter(orderQueue);
        IReceiver orderReceiver = new ReceiverAdapter(orderQueue);
        ISender sender = new SenderAdapter(queue);
        IReceiver receiver = new ReceiverAdapter(queue);
        var mre0 = new ManualResetEvent(false);
        var th1 = new ServerThread(receiver, orderReceiver);
        senderDict.TryAdd("21", sender);
        threadDict.TryAdd("21", th1);
        orderSenderDict.TryAdd("21", orderSender);
        var act1 = new ActionCommand(()=>{
                    Hwdtech.IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", Hwdtech.IoC.Resolve<object>("Scopes.New", Hwdtech.IoC.Resolve<object>("Scopes.Root"))).Execute();
        Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "StopGameCommandExecuting", (object[] args) => stopGameCommandExecutingStrategy.Object.RunStrategy(args)).Execute();
        Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.Options.GetAll", (object[] args) => gameOptionsGetAllStrategy.Object.RunStrategy(args)).Execute();
        Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.Objects.GetAll", (object[] args) => gameObjectsGetAllStrategy.Object.RunStrategy(args)).Execute();
        Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.Queue.Get", (object[] args) => gameQueueGetStrategy.Object.RunStrategy(args)).Execute();
        Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.Get.Timespan", (object[] _) => (object)new TimeSpan(0, 0, 0, 0, 100)).Execute();
        Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "StringfyOption", (object[] args) => stringfyOptionStrategy.Object.RunStrategy(args)).Execute();
            Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "StringifyObject", (object[] args) => stringfyObjectStrategy.Object.RunStrategy(args)).Execute();
            Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "SerializeCommand", (object[] args) => serializeCommandStrategy.Object.RunStrategy(args)).Execute();
            Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "EndPointClientCall", (object[] _)=>command2.Object).Execute();
        Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "HandleException", (object[] args) => regStrategy1.Object.RunStrategy(args)).Execute();
            mre0.Set();
        });
        orderSender.Send(act1);
        th1.Start();
        var mre1 = new ManualResetEvent(false);
        GameStatus status = new GameStatus
        {
            GameId = "1",
            NewServerId = "174.168.50.10"
        };
        mre0.WaitOne();
        var newStatus = router.routeMigrateCommand(status.NewServerId, status.GameId);
        Assert.IsInstanceOf<bool>(newStatus);
        Assert.True(newStatus);
        sender.Send(new ActionCommand(()=>mre1.Set()));
        mre1.WaitOne();
        stopGameCommandExecutingStrategy.Verify();
        gameOptionsGetAllStrategy.Verify();
        gameObjectsGetAllStrategy.Verify();
        gameQueueGetStrategy.Verify();
        stringfyOptionStrategy.Verify();
        stringfyObjectStrategy.Verify();
        serializeCommandStrategy.Verify();
    }
    [Test]
    public void PositiveDeserializeGame()
    {

        new Hwdtech.Ioc.InitScopeBasedIoCImplementationCommand().Execute();
        var initialScope = Hwdtech.IoC.Resolve<object>("Scopes.New", Hwdtech.IoC.Resolve<object>("Scopes.Root"));
        Hwdtech.IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", initialScope).Execute();

        var threadDict = new ConcurrentDictionary<string, ServerThread>();
        var gamesThreadsDictionary = new ConcurrentDictionary<string, string>();
        var senderDict = new ConcurrentDictionary<string, ISender>();
        var orderSenderDict = new ConcurrentDictionary<string, ISender>();
        IEndPointRouter router = new EndPointRouter(gamesThreadsDictionary, orderSenderDict);
        gamesThreadsDictionary.TryAdd("1", "21");


        Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "ThreadIDServerThreadMapping", (object[] _) => threadDict).Execute();
        Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "ThreadIDSenderMapping", (object[] _) => senderDict).Execute();
        Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "ThreadIDOrdersSenderMapping", (object[] _) => orderSenderDict).Execute();

        ConcurrentDictionary<string, object> threadScopes = new(){};
        threadScopes.TryAdd("21", Hwdtech.IoC.Resolve<object>("Scopes.New", Hwdtech.IoC.Resolve<object>("Scopes.Root")));
        BlockingCollection<ICommand> queue = new();
        BlockingCollection<ICommand> orderQueue = new();
        ISender orderSender = new SenderAdapter(orderQueue);
        IReceiver orderReceiver = new ReceiverAdapter(orderQueue);
        ISender sender = new SenderAdapter(queue);
        IReceiver receiver = new ReceiverAdapter(queue);

        string serilaizedString = "Scope1,Scope2,Option3|Key1 : Value1;Key2 : Value2;Key3 : Value3|type: Command1,type: Command2|00:30:00";

        var deserializeStrategy = new Mock<IStrategy>();
        deserializeStrategy.Setup(_strategy => _strategy.RunStrategy(It.IsAny<object[]>())).Returns(new Mock<IUObject>().Object).Verifiable();

        var emptyCommand = new Mock<ICommand>();
        emptyCommand.Setup(_command => _command.Execute());
        var deserializeCommandStrategy = new Mock<IStrategy>();
        deserializeCommandStrategy.Setup(_strategy => _strategy.RunStrategy(It.IsAny<object[]>())).Returns(emptyCommand.Object).Verifiable();

        var gameScope = Hwdtech.IoC.Resolve<object>("Scopes.New", Hwdtech.IoC.Resolve<object>("Scopes.Root"));

        var gameScopeIoCInicializationStrategy = new Mock<IStrategy>();
        gameScopeIoCInicializationStrategy.Setup(_strategy => _strategy.RunStrategy(It.IsAny<object[]>())).Returns(gameScope).Verifiable();

        var threadScopeGameIdNewStrategy = new Mock<IStrategy>();
        threadScopeGameIdNewStrategy.Setup(_strategy => _strategy.RunStrategy(It.IsAny<object[]>())).Returns("9090").Verifiable();

        var mySenderStrategy = new Mock<IStrategy>();
        mySenderStrategy.Setup(_strategy => _strategy.RunStrategy()).Returns(sender).Verifiable();

        var command1 = new Mock<ICommand>();
        var regStrategy1 = new Mock<IStrategy>();
        command1.Setup(_command => _command.Execute());
        regStrategy1.Setup(_strategy => _strategy.RunStrategy(It.IsAny<object[]>())).Returns(command1.Object);

        var createAndStartThreadStrategy = new CreateWithStartThreadStrategy();
        Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "CreateWithStartThread", (object[] args) => createAndStartThreadStrategy.RunStrategy(args)).Execute();
        Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "QuantumForGame", (object[] _) => (object)new TimeSpan(0, 0, 0, 0, 200)).Execute();

        var mre0 = new ManualResetEvent(false);
        var th1 = new ServerThread(receiver, orderReceiver);
        senderDict.TryAdd("21", sender);
        threadDict.TryAdd("21", th1);
        orderSenderDict.TryAdd("21", orderSender);
        var act1 = new ActionCommand(()=>{
            Hwdtech.IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", threadScopes["21"]).Execute();
            Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "DeserializeValue", (object[] args) => deserializeStrategy.Object.RunStrategy(args)).Execute();
            Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "DeserializeCommand", (object[] args) => deserializeCommandStrategy.Object.RunStrategy(args)).Execute();
            Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "DeserializeTimespan", (object[] _) => (object)new TimeSpan(0, 0, 0, 0, 100)).Execute();
            Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.Scope.Create", (object[] args) => gameScopeIoCInicializationStrategy.Object.RunStrategy(args)).Execute();
            Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "ThreadScope.GameId.New", (object[] args) => threadScopeGameIdNewStrategy.Object.RunStrategy(args)).Execute();
            Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "MySender", (object[] args) => mySenderStrategy.Object.RunStrategy(args)).Execute();
            Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "HandleException", (object[] args) => regStrategy1.Object.RunStrategy(args)).Execute();
            Hwdtech.IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", gameScope).Execute();
            Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "ThreadScope.Current", (object[] _) => threadScopes["21"]).Execute();
            Hwdtech.IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", threadScopes["21"]).Execute();
            mre0.Set();
        });
        orderSender.Send(act1);
        th1.Start();
        
        var mre1 = new ManualResetEvent(false);
        mre0.WaitOne();

        var newStatus = router.routeAcceptCommand(serilaizedString);
        sender.Send(new ActionCommand(()=>mre1.Set()));
        mre1.WaitOne(200);
        Assert.IsInstanceOf<bool>(newStatus);
        Assert.True(newStatus);
        deserializeStrategy.Verify();
        gameScopeIoCInicializationStrategy.Verify();
        threadScopeGameIdNewStrategy.Verify();
        mySenderStrategy.Verify();
    }

}