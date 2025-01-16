using System.Collections.Concurrent;
using gRPC.Services;
using GRpc.Server;
using Hwdtech;
using Hwdtech.Ioc;
using ShipGame.Game;
using ShipGame.Server;

new InitScopeBasedIoCImplementationCommand().Execute();
IoC.Resolve<ICommand>("Scopes.Current.Set", IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))).Execute();

var threadDict = new ConcurrentDictionary<string, ServerThread>();
var senderDict = new ConcurrentDictionary<string, ISender>();
var externalSenderDict = new ConcurrentDictionary<string, ISender>(); 
IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "ExternalSenderDictionary", (object[] _) => externalSenderDict ).Execute();
IoC.Resolve<ICommand>("IoC.Register", "ThreadDictionary", (object[] _) => threadDict).Execute();
IoC.Resolve<ICommand>("IoC.Register", "SenderDictionary", (object[] _) => senderDict).Execute();

var emptyCommand = new ActionCommand(() => {});
var gamesDictionary = new ConcurrentDictionary<string, Queue<ShipGame.Move.ICommand>>();

Action action = ()=>{
    Hwdtech.IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", Hwdtech.IoC.Resolve<object>("Scopes.New", Hwdtech.IoC.Resolve<object>("Scopes.Root"))).Execute();
    Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "messageToICommand", (object[] args) => emptyCommand).Execute();
    Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "GameQueueReceiveCommand", (object[] args) => new ActionCommand(()=>gamesDictionary[(string)args[0]].Append((ShipGame.Move.ICommand)args[1]))).Execute();
};

var gameQueue = new Queue<ShipGame.Move.ICommand>();
var gameCommand = new GameCommand("game1", gameQueue);
gamesDictionary.TryAdd("game1", gameQueue);

var gameThread = new ConcurrentDictionary<string, string>();
gameThread.TryAdd("game1","108");

BlockingCollection<ShipGame.Move.ICommand> queue = new BlockingCollection<ShipGame.Move.ICommand>(100);
BlockingCollection<ShipGame.Move.ICommand> queue2 = new BlockingCollection<ShipGame.Move.ICommand>();
var sender = new SenderAdapter(queue);
var sender2 = new SenderAdapter(queue2);
string thredId = "108";
sender.Send(new ActionCommand(action));
var receiver = new ReceiverAdapter(queue);
var receiver2 = new ReceiverAdapter(queue2);
var ST = new ServerThread(receiver, receiver2);
ST.Start();
senderDict.TryAdd(thredId, sender);
threadDict.TryAdd(thredId, ST);
externalSenderDict.TryAdd(thredId, sender2);

var router = new RouterMessage(gameThread, externalSenderDict);
var endpoint = new EndPointService(router);

var builder = WebApplication.CreateBuilder();
builder.Services.AddSingleton(endpoint);
builder.Services.AddGrpc();

WebApplication app = builder.Build();
app.MapGrpcService<EndPointService>();
app.Run();