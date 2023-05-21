using Hwdtech.Ioc;
using Hwdtech;
using ShipGame.Server;
using SpaceBattle.ServerStrategies;
using System.Collections.Concurrent;
using Moq;
using ShipGame.Move;
using ICommand = Hwdtech.ICommand;

namespace Tests.TestServerThread
{
    public class TestThread
    {
        public TestThread()
        {
            new InitScopeBasedIoCImplementationCommand().Execute();
            IoC.Resolve<ICommand>("Scopes.Current.Set", IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))).Execute();

            var threadDict = new ConcurrentDictionary<string, ServerThread>();
            var senderDict = new ConcurrentDictionary<string, ISender>();
            IoC.Resolve<ICommand>("IoC.Register", "ThreadDictionary", (object[] _) => threadDict).Execute();
            IoC.Resolve<ICommand>("IoC.Register", "SenderDictionary", (object[] _) => senderDict).Execute();
            IoC.Resolve<ICommand>("IoC.Register", "SenderAdapterGetByID", (object[] id) => senderDict[(string)id[0]]).Execute();
            IoC.Resolve<ICommand>("IoC.Register", "ServerThreadGetByID", (object[] id) => threadDict[(string)id[0]]).Execute();

            var createWithStartThreadStrategy = new CreateAndStartThreadStrategy();
            IoC.Resolve<ICommand>("IoC.Register", "CreateAndStartThread", (object[] args) => createWithStartThreadStrategy.RunStrategy(args)).Execute();
            var hardStopCommandStrategy = new HardStopCommandStrategy();
            IoC.Resolve<ICommand>("IoC.Register", "HardStop", (object[] args) => hardStopCommandStrategy.RunStrategy(args)).Execute();
            var softStopCommandStrategy = new SoftStopCommandStrategy();
            IoC.Resolve<ICommand>("IoC.Register", "SoftStop", (object[] args) => softStopCommandStrategy.RunStrategy(args)).Execute();
            var sendCommandStrategy = new SendCommandStrategy();
            IoC.Resolve<ICommand>("IoC.Register", "SendCommand", (object[] args) => sendCommandStrategy.RunStrategy(args)).Execute();

            var command1 = new Mock<ShipGame.Move.ICommand>();
            var regStrategy1 = new Mock<IStrategy>();
            command1.Setup(_command => _command.Execute()).Verifiable();
            regStrategy1.Setup(_strategy => _strategy.RunStrategy(It.IsAny<object[]>())).Returns(command1.Object).Verifiable();
            IoC.Resolve<ICommand>("IoC.Register", "HandleException", (object[] args) => regStrategy1.Object.RunStrategy(args)).Execute();
        }
        [Test]
        public void CreateServerThreadTest()
        {
            BlockingCollection<ShipGame.Move.ICommand> que = new BlockingCollection<ShipGame.Move.ICommand>(100);
            var sender = new Mock<ISender>();
            var receiver = new Mock<IReceiver>();
            var ST = IoC.Resolve<ServerThread>("CreateAndStartThread", "1", sender.Object, receiver.Object);
            Assert.False(ST.GetStop());
            Assert.NotNull(IoC.Resolve<ServerThread>("ServerThreadGetByID", "1"));
            Assert.NotNull(IoC.Resolve<ISender>("SenderAdapterGetByID", "1"));
        }
        [Test]
        public void HardStopThreadTest()
        {
            BlockingCollection<ShipGame.Move.ICommand> que = new BlockingCollection<ShipGame.Move.ICommand>(100);
            var sender = new Mock<ISender>();
            var receiver = new Mock<IReceiver>();
            receiver.Setup(x => x.Receive()).Returns(() => que.Take());
            receiver.Setup(x => x.IsEmpty()).Returns(() => que.Count() == 0);
            sender.Setup(x => x.Send(It.IsAny<ShipGame.Move.ICommand>())).Callback(
                (ShipGame.Move.ICommand cmd) =>
                {
                    que.Add(cmd);
                });

            var th1 = IoC.Resolve<ServerThread>("CreateAndStartThread", "2", sender.Object, receiver.Object);
            var mre1 = new ManualResetEvent(false);
            var hardStopCommand = IoC.Resolve<ShipGame.Move.ICommand>("HardStop", "2", () => { mre1.Set(); });
            var send = IoC.Resolve<ISender>("SenderAdapterGetByID", "2");
            var sendCommand = IoC.Resolve<ShipGame.Move.ICommand>("SendCommand", send, hardStopCommand);

            sendCommand.Execute();
            mre1.WaitOne(200);
            Assert.True(th1.GetStop());
            Assert.True(th1.QueueIsEmpty());
        }
    }
}
