using Hwdtech.Ioc;
using Hwdtech;
using ShipGame.Move;
using System.Collections.Concurrent;
using gRPC;
using Moq;
using ICommand = ShipGame.Move.ICommand;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using gRPC.Services;
using gRPC.StartEndPointService;
using ShipGame.Server;
using SpaceBattle.ServerStrategies;

namespace Tests.TestgRPC
{
    public class gRPCTest
    {
        public gRPCTest()
        {
            new InitScopeBasedIoCImplementationCommand().Execute();
            IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))).Execute();

            var threadDict = new ConcurrentDictionary<string, ServerThread>();
            var senderDict = new ConcurrentDictionary<string, ISender>();
            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "ThreadDictionary", (object[] _) => threadDict).Execute();
            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "SenderDictionary", (object[] _) => senderDict).Execute();
            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "SenderGetByID", (object[] id) => senderDict[(string)id[0]]).Execute();
            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "ServerThreadGetByID", (object[] id) => threadDict[(string)id[0]]).Execute();

            var sendCommandStrategy = new SendCommandStrategy();
            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "SendCommand", (object[] args) => sendCommandStrategy.RunStrategy(args)).Execute();

            var createWithStartStrategy = new CreateWithStartThreadStrategy();
            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "CreateWithStartThread", (object[] args) => createWithStartStrategy.RunStrategy(args)).Execute();

            var threadgamedict = new ConcurrentDictionary<string, string>();
            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Storage.ThreadByGameID", (object[] args) => threadgamedict).Execute();
            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Storage.GetThreadByGameID", (object[] args) =>
            {
                var dict = IoC.Resolve<ConcurrentDictionary<string, string>>("Storage.ThreadByGameID");
                return dict[(string)args[0]];
            }
            ).Execute();
            var sendCommandByThreadID = new SendCommandByThreadIDStrategy();
            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "SendCommandByThreadID", (object[] args) => sendCommandByThreadID.RunStrategy(args)).Execute();
        }

        [Test]
        public void EndPointSuccessfulTest()
        {
            var cestrat = new StartEndPointStrategy();
            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "CreateEndPoint", (object[] args) => cestrat.RunStrategy(args)).Execute();

            var thread1 = IoC.Resolve<ServerThread>("CreateWithStartThread", "thread1");

            var games = IoC.Resolve<ConcurrentDictionary<string, string>>("Storage.ThreadByGameID");
            games.TryAdd("game1", "thread1");
            var request = new CommandRequest { GameId = "game1", CommandType = "Check", GameItemId = "1" };
            var d = new Dictionary<string, string>() { { "123", "456" }, { "12", "24" } };
            var commandArgs = d.Select(kv => new CommandForObject { Key = kv.Key, Value = kv.Value }).ToArray();
            request.Args.Add(commandArgs);
            var cmd = new Mock<ICommand>();
            cmd.Setup(_command => _command.Execute()).Verifiable();
            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "CreateCommandByNameForObject", (object[] args) =>
            {
                return cmd.Object;
            }
            ).Execute();
            var mre1 = new ManualResetEvent(false);
            var sender = IoC.Resolve<ISender>("SenderGetByID", "thread1");
            IoC.Resolve<ICommand>("SendCommand", sender, new ActionCommand(() => { mre1.Set(); })).Execute();
            var endp = IoC.Resolve<ICommand>("CreateEndPoint");
            var service = new EndPointService(new Mock<ILogger<EndPointService>>().Object);
            service.Message(request, new Mock<ServerCallContext>().Object);
            Assert.True(thread1.QueueIsEmpty());
            cmd.Verify();
        }
    }
}
