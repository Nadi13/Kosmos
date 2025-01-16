using System.Collections.Concurrent;
using ShipGame.Server;
using ShipGame.Move;
using GRpc.Server;
using Moq;
using ShipGame.Game;
using SpaceBattle.ServerStrategies;
using gRPC;
using Hwdtech;
using ICommand=ShipGame.Move.ICommand;

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
        var externalSenderDict = new ConcurrentDictionary<string, ISender>();
        var gamesDict = new Dictionary<string, Queue<ICommand>>();
        IEndPointRouter router = new RouterMessage(gamesThreadsDictionary, externalSenderDict);

        Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "ThreadDictionary", (object[] _) => threadDict).Execute();
        Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "SenderDictionary", (object[] _) => senderDict).Execute();
        Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "ExternalSenderDictionary", (object[] _) => externalSenderDict).Execute();
        Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "ServerThreadGetByID", (object[] id) => threadDict[(string)id[0]]).Execute();

        List<string> gameOptions = new()
        {
            "team_deathmatch",
            "escort_mission"
        };
        Dictionary<string, object> gameObjects = new()
        {
            {"player_helmet_alpha", new Mock<IUObject>()},
            {"space_station", new Mock<IUObject>()},
            {"orbital_station_zeta", new Mock<IUObject>()},
            {"warp_gate_alpha", new Mock<IUObject>()},
            {"IOC-bee", new Mock<IUObject>()},
        };

        Queue<ICommand> gameQueue1 = new Queue<ICommand>();
        gameQueue1.Enqueue(new ActionCommand(()=>{}));
        gamesDict.TryAdd("game1", gameQueue1);
        ICommand gameCommand1 = new GameCommand("game1", gameQueue1);
        gamesThreadsDictionary.TryAdd("game1", "21");

        var stopGameCommandExecutingStrategy = new Mock<IStrategy>();
        stopGameCommandExecutingStrategy.Setup(_strategy => _strategy.RunStrategy()).Returns(new ActionCommand(()=>{})).Verifiable();


        var gameOptionsGetAllStrategy = new Mock<IStrategy>();
        gameOptionsGetAllStrategy.Setup(_strategy => _strategy.RunStrategy(It.IsAny<object[]>())).Returns(gameOptions).Verifiable();


        var gameObjectsGetAllStrategy = new Mock<IStrategy>();
        gameObjectsGetAllStrategy.Setup(_strategy => _strategy.RunStrategy(It.IsAny<object[]>())).Returns(gameObjects).Verifiable();


        var gameQueueGetStrategy = new Mock<IStrategy>();
        gameQueueGetStrategy.Setup(_strategy => _strategy.RunStrategy(It.IsAny<object[]>())).Returns(gameQueue1).Verifiable();


        var serializeOptionStrategy = new Mock<IStrategy>();
        serializeOptionStrategy.Setup(_strategy => _strategy.RunStrategy(It.IsAny<object[]>())).Returns("GameOption_").Verifiable();


        var serializeObjectStrategy = new Mock<IStrategy>();
        serializeOptionStrategy.Setup(_strategy => _strategy.RunStrategy(It.IsAny<object[]>())).Returns("Object_").Verifiable();


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


        var mre0 = new ManualResetEvent(false);
        var act1 = ()=>{
                    Hwdtech.IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", Hwdtech.IoC.Resolve<object>("Scopes.New", Hwdtech.IoC.Resolve<object>("Scopes.Root"))).Execute();
        Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "TerminateGameCommand", (object[] args) => stopGameCommandExecutingStrategy.Object.RunStrategy(args)).Execute();
        Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.Options.GetAll", (object[] args) => gameOptionsGetAllStrategy.Object.RunStrategy(args)).Execute();
        Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.Objects.GetAll", (object[] args) => gameObjectsGetAllStrategy.Object.RunStrategy(args)).Execute();
        Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.Queue.Get", (object[] args) => gameQueueGetStrategy.Object.RunStrategy(args)).Execute();
        Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.Get.Timespan", (object[] _) => (object)new TimeSpan(0, 0, 0, 0, 100)).Execute();
        Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "SerializeOption", (object[] args) => serializeOptionStrategy.Object.RunStrategy(args)).Execute();
            Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "SerializeObject", (object[] args) => serializeObjectStrategy.Object.RunStrategy(args)).Execute();
            Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "SerializeCommand", (object[] args) => serializeCommandStrategy.Object.RunStrategy(args)).Execute();
            Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "EndPointClientCall", (object[] _)=>command2.Object).Execute();
        Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "HandleException", (object[] args) => regStrategy1.Object.RunStrategy(args)).Execute();
            mre0.Set();
        };
        var th1 = IoC.Resolve<ServerThread>("CreateWithStartThread", "21", act1);
        var mre1 = new ManualResetEvent(false);
        SendGameToAnotherServerRequest request = new SendGameToAnotherServerRequest
        {
            GameId = "game1",
            NewServerId = "174.168.50.10"
        };
        mre0.WaitOne();
        var newStatus = router.isMigrate(request);
        Assert.IsInstanceOf<bool>(newStatus);
        Assert.True(newStatus);
        senderDict["21"].Send(new ActionCommand(()=>mre1.Set()));
        mre1.WaitOne();
        stopGameCommandExecutingStrategy.Verify();
        gameOptionsGetAllStrategy.Verify();
        gameObjectsGetAllStrategy.Verify();
        gameQueueGetStrategy.Verify();
        serializeOptionStrategy.Verify();
        serializeObjectStrategy.Verify();
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
        var externalSenderDict = new ConcurrentDictionary<string, ISender>();
        IEndPointRouter router = new RouterMessage(gamesThreadsDictionary, externalSenderDict);
        gamesThreadsDictionary.TryAdd("game1", "21");


        Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "ThreadDictionary", (object[] _) => threadDict).Execute();
        Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "SenderDictionary", (object[] _) => senderDict).Execute();
        Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "ExternalSenderDictionary", (object[] _) => externalSenderDict).Execute();

        ConcurrentDictionary<string, object> threadScopes = new(){};
        threadScopes.TryAdd("21", Hwdtech.IoC.Resolve<object>("Scopes.New", Hwdtech.IoC.Resolve<object>("Scopes.Root")));

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
        mySenderStrategy.Setup(_strategy => _strategy.RunStrategy()).Returns(senderDict.TakeLast(1)).Verifiable();

        var command1 = new Mock<ICommand>();
        var regStrategy1 = new Mock<IStrategy>();
        command1.Setup(_command => _command.Execute());
        regStrategy1.Setup(_strategy => _strategy.RunStrategy(It.IsAny<object[]>())).Returns(command1.Object);

        var createAndStartThreadStrategy = new CreateWithStartThreadStrategy();
        Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "CreateWithStartThread", (object[] args) => createAndStartThreadStrategy.RunStrategy(args)).Execute();
        Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "QuantumForGame", (object[] _) => (object)new TimeSpan(0, 0, 0, 0, 200)).Execute();

        var mre0 = new ManualResetEvent(false);

        var act1 = ()=>{
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
        };
        var th1 = IoC.Resolve<ServerThread>("CreateWithStartThread", "21", act1);

        var mre1 = new ManualResetEvent(false);
        mre0.WaitOne();

        var newStatus = router.isAccept(serilaizedString);
        senderDict["21"].Send(new ActionCommand(()=>mre1.Set()));
        mre1.WaitOne(200);
        Assert.IsInstanceOf<bool>(newStatus);
        Assert.True(newStatus);
        deserializeStrategy.Verify();
        gameScopeIoCInicializationStrategy.Verify();
        threadScopeGameIdNewStrategy.Verify();
        mySenderStrategy.Verify();
    }
}