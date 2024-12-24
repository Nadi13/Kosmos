using System.Collections.Concurrent;
using gRPC.EndPointRouter;
using gRPC.Router;
using gRPC.Services;
using ShipGame.Game;
using ShipGame.Server;
using SpaceBattle.ServerStrategies;
using ICommand = ShipGame.Move.ICommand;

new Hwdtech.Ioc.InitScopeBasedIoCImplementationCommand().Execute();
Hwdtech.IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", Hwdtech.IoC.Resolve<object>("Scopes.New", Hwdtech.IoC.Resolve<object>("Scopes.Root"))).Execute();

var threadDict = new ConcurrentDictionary<string, ServerThread>();
var gamesThreadsDictionary = new ConcurrentDictionary<string, string>();
var senderDict = new ConcurrentDictionary<string, ISender>();
var senderOrderDict = new ConcurrentDictionary<string, ISender>();
var gamesDictionary = new Dictionary<string, Queue<ICommand>>();
Queue<ICommand> gameQueue1 = new Queue<ICommand>();
Queue<ICommand> gameQueue2 = new Queue<ICommand>();
gamesDictionary.TryAdd("1", gameQueue1);
gamesDictionary.TryAdd("2", gameQueue2);
ICommand gameCommand1 = new GameCommand("1", gameQueue1);
ICommand gameCommand2 = new GameCommand("2", gameQueue2);
gamesThreadsDictionary.TryAdd("1", "80");

Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "ThreadDictionary", (object[] _) => threadDict).Execute();
Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "SenderDictionary", (object[] _) => senderDict).Execute();
Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "SenderOrderDictionary", (object[] _) => senderOrderDict).Execute();
var createWithStartThreadStrategy = new CreateWithStartThreadStrategy();
Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "CreateWithStartThread", (object[] args) => createWithStartThreadStrategy.RunStrategy(args)).Execute();
Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "QuantumForGame", (object[] _) => (object)new TimeSpan(0, 0, 0, 40, 0)).Execute();
ICommand emptyCommand = new ActionCommand(()=>{});

Action act1 = () => {
    Hwdtech.IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", Hwdtech.IoC.Resolve<object>("Scopes.New", Hwdtech.IoC.Resolve<object>("Scopes.Root"))).Execute();
    Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "OrderDictionaryToICommand", (object[] args) => emptyCommand).Execute();
    Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "SendCommandToGame", (object[] args) => new ActionCommand(()=>gamesDictionary[(string)args[0]].Append((ICommand)args[1]))).Execute();
};

var th1 = Hwdtech.IoC.Resolve<ServerThread>("CreateWithStartThread", "80", act1);
IEndPointRouter router = new EndPointRouter(gamesThreadsDictionary, senderOrderDict);
var logger = new LoggerFactory().CreateLogger<EndPointService>();
EndPointService endpoint = new EndPointService(logger, router);

var builder = WebApplication.CreateBuilder();
builder.Services.AddSingleton(endpoint);
builder.Services.AddGrpc();

WebApplication app = builder.Build();
app.MapGrpcService<EndPointService>();
app.Run();