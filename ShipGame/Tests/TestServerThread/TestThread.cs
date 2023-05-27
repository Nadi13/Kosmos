using Hwdtech.Ioc;
using Hwdtech;
using ShipGame.Server;
using System.Collections.Concurrent;
using Moq;
using ICommand = Hwdtech.ICommand;
using ShipGame.Move;
using SpaceBattle.ServerStrategies;

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

            var createWithStartThreadStrategy = new CreateWithStartThreadStrategy();
            IoC.Resolve<ICommand>("IoC.Register", "CreateWithStartThread", (object[] args) => createWithStartThreadStrategy.RunStrategy(args)).Execute();
            var hardStopCommandStrategy = new HardStopCommandStrategy();
            IoC.Resolve<ICommand>("IoC.Register", "HardStop", (object[] args) => hardStopCommandStrategy.RunStrategy(args)).Execute();
            var softStopCommandStrategy = new SoftStopCommandStrategy();
            IoC.Resolve<ICommand>("IoC.Register", "SoftStop", (object[] args) => softStopCommandStrategy.RunStrategy(args)).Execute();
            var sendCommandStrategy = new SendCommandStrategy();
            IoC.Resolve<ICommand>("IoC.Register", "SendCommand", (object[] args) => sendCommandStrategy.RunStrategy(args)).Execute();
        }
        [Test]
        public void CreateServerThreadTest()
        {
            var ST = IoC.Resolve<ServerThread>("CreateWithStartThread", "1");
            Assert.False(ST.GetStop());
            Assert.NotNull(IoC.Resolve<ServerThread>("ServerThreadGetByID", "1"));
            Assert.NotNull(IoC.Resolve<ISender>("SenderAdapterGetByID", "1"));
        }
        [Test]
        public void HardStopThreadTest()
        {
            var th1 = IoC.Resolve<ServerThread>("CreateWithStartThread", "2");
            var mre1 = new ManualResetEvent(false);
            var hardStopCommand = IoC.Resolve<ShipGame.Move.ICommand>("HardStop", "2", () => { mre1.Set(); });
            var send = IoC.Resolve<ISender>("SenderAdapterGetByID", "2");
            var sendCommand = IoC.Resolve<ShipGame.Move.ICommand>("SendCommand", send, hardStopCommand);

            sendCommand.Execute();
            mre1.WaitOne(200);
            Assert.True(th1.GetStop());
            Assert.True(th1.QueueIsEmpty());
        }

        [Test]
        public void HardStopCommandThreadWithoutAction()
        {
            var th5 = IoC.Resolve<ServerThread>("CreateWithStartThread", "5");
            var hardStopCommand = IoC.Resolve<ShipGame.Move.ICommand>("HardStop", "5");
            Assert.NotNull(hardStopCommand);
            var senderTrue = IoC.Resolve<ISender>("SenderAdapterGetByID", "5");
            var mre1 = new ManualResetEvent(false);
            IoC.Resolve<ShipGame.Move.ICommand>("SendCommand", senderTrue, new ActionCommand(() => { mre1.Set(); })).Execute();
            var sendCommand = IoC.Resolve<ShipGame.Move.ICommand>("SendCommand", senderTrue, hardStopCommand);
            sendCommand.Execute();
            mre1.WaitOne(200);
            Assert.True(th5.QueueIsEmpty());
            Assert.True(th5.GetStop());
        }
        [Test]
        public void HardStopThreadWithException()
        {
            var command1 = new Mock<ShipGame.Move.ICommand>();
            var regStrategy1 = new Mock<IStrategy>();
            command1.Setup(_command => _command.Execute()).Verifiable();
            regStrategy1.Setup(_strategy => _strategy.RunStrategy(It.IsAny<object[]>())).Returns(command1.Object);
            IoC.Resolve<ICommand>("IoC.Register", "HandleException", (object[] args) => regStrategy1.Object.RunStrategy(args)).Execute();
            Action act1 = () => {
                IoC.Resolve<ICommand>("Scopes.Current.Set", IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))).Execute();
                IoC.Resolve<ICommand>("IoC.Register", "HandleException", (object[] args) => regStrategy1.Object.RunStrategy(args)).Execute();
            };
            var th3 = IoC.Resolve<ServerThread>("CreateWithStartThread", "3", act1);
            var th4 = IoC.Resolve<ServerThread>("CreateWithStartThread", "4", act1);
            var mre1 = new ManualResetEvent(false);
            var hardStopCommand = IoC.Resolve<ShipGame.Move.ICommand>("HardStop", "4", () => { mre1.Set(); });
            var senderFalse = IoC.Resolve<ISender>("SenderAdapterGetByID", "3");
            var sendCommand = IoC.Resolve<ShipGame.Move.ICommand>("SendCommand", senderFalse, hardStopCommand);

            sendCommand.Execute();
            mre1.WaitOne(200);
            Assert.True(th3.QueueIsEmpty());
            Assert.False(th3.GetStop());
            Assert.False(th4.GetStop());
            command1.Verify();
        }
        [Test]
        public void SoftStopThreadTest()
        {
            var mockCommand1 = new Mock<ShipGame.Move.ICommand>();
            var mockCommand2 = new Mock<ShipGame.Move.ICommand>();
            var mockCommand3 = new Mock<ShipGame.Move.ICommand>();
            var mockCommand4 = new Mock<ShipGame.Move.ICommand>();
            var mockCommand5 = new Mock<ShipGame.Move.ICommand>();
            var mockCommand6 = new Mock<ShipGame.Move.ICommand>();
            mockCommand1.Setup(_command => _command.Execute()).Verifiable();
            mockCommand2.Setup(_command => _command.Execute()).Verifiable();
            mockCommand3.Setup(_command => _command.Execute()).Verifiable();
            mockCommand4.Setup(_command => _command.Execute()).Verifiable();
            mockCommand5.Setup(_command => _command.Execute()).Verifiable();
            mockCommand6.Setup(_command => _command.Execute()).Verifiable();

            var mre1 = new ManualResetEvent(false);
            var th6 = IoC.Resolve<ServerThread>("CreateWithStartThread", "6");
            Assert.True(th6.QueueIsEmpty());
            var softStopCommand = IoC.Resolve<ShipGame.Move.ICommand>("SoftStop", "6");
            var sender = IoC.Resolve<ISender>("SenderAdapterGetByID", "6");
            IoC.Resolve<ShipGame.Move.ICommand>("SendCommand", sender, mockCommand1.Object).Execute();
            IoC.Resolve<ShipGame.Move.ICommand>("SendCommand", sender, mockCommand2.Object).Execute();
            IoC.Resolve<ShipGame.Move.ICommand>("SendCommand", sender, mockCommand3.Object).Execute();
            var sendCommand = IoC.Resolve<ShipGame.Move.ICommand>("SendCommand", sender, softStopCommand);
            sendCommand.Execute();
            IoC.Resolve<ShipGame.Move.ICommand>("SendCommand", sender, mockCommand4.Object).Execute();
            IoC.Resolve<ShipGame.Move.ICommand>("SendCommand", sender, mockCommand5.Object).Execute();
            IoC.Resolve<ShipGame.Move.ICommand>("SendCommand", sender, mockCommand6.Object).Execute();
            IoC.Resolve<ShipGame.Move.ICommand>("SendCommand", sender, new ActionCommand(() => { mre1.Set(); })).Execute();
            Assert.False(th6.QueueIsEmpty());
            mre1.WaitOne(200);
            mockCommand1.Verify();
            mockCommand3.Verify();
            mockCommand2.Verify();
            Assert.True(th6.QueueIsEmpty());
            Assert.True(th6.GetStop());
        }
        [Test]
        public void SoftStopCommandThreadWithException()
        {
            var command2 = new Mock<ShipGame.Move.ICommand>();
            var regStrategy2 = new Mock<IStrategy>();
            command2.Setup(_command => _command.Execute());
            regStrategy2.Setup(_strategy => _strategy.RunStrategy(It.IsAny<object[]>())).Returns(command2.Object);
            Action act1 = () => {
                IoC.Resolve<ICommand>("Scopes.Current.Set", IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))).Execute();
                IoC.Resolve<ICommand>("IoC.Register", "HandleException", (object[] args) => regStrategy2.Object.RunStrategy(args)).Execute();
            };

            var th8 = IoC.Resolve<ServerThread>("CreateWithStartThread", "8", act1);
            var th9 = IoC.Resolve<ServerThread>("CreateWithStartThread", "9", act1);
            var mre1 = new ManualResetEvent(false);
            var softStopCommand = IoC.Resolve<ShipGame.Move.ICommand>("SoftStop", "9", () => { mre1.Set(); });
            var sender = IoC.Resolve<ISender>("SenderAdapterGetByID", "8");
            var sendCommand = IoC.Resolve<ShipGame.Move.ICommand>("SendCommand", sender, softStopCommand);

            sendCommand.Execute();
            mre1.WaitOne(200);
            Assert.True(th8.QueueIsEmpty());
            Assert.False(th8.GetStop());
            Assert.False(th8.GetStop());
        }
        [Test]
        public void ThreadSoftStopTestWithAction()
        {
            var mockCommand1 = new Mock<ShipGame.Move.ICommand>();
            var mockCommand2 = new Mock<ShipGame.Move.ICommand>();
            var mockCommand3 = new Mock<ShipGame.Move.ICommand>();
            var mockCommand4 = new Mock<ShipGame.Move.ICommand>();
            var mockCommand5 = new Mock<ShipGame.Move.ICommand>();
            var mockCommand6 = new Mock<ShipGame.Move.ICommand>();
            mockCommand1.Setup(_command => _command.Execute()).Verifiable();
            mockCommand2.Setup(_command => _command.Execute()).Verifiable();
            mockCommand3.Setup(_command => _command.Execute()).Verifiable();
            mockCommand4.Setup(_command => _command.Execute()).Verifiable();
            mockCommand5.Setup(_command => _command.Execute()).Verifiable();
            mockCommand6.Setup(_command => _command.Execute()).Verifiable();

            var mre1 = new ManualResetEvent(false);
            var th11 = IoC.Resolve<ServerThread>("CreateWithStartThread", "11");
            Assert.True(th11.QueueIsEmpty());
            var softStopCommand = IoC.Resolve<ShipGame.Move.ICommand>("SoftStop", "11", () => { mre1.Set(); });
            var sender = IoC.Resolve<ISender>("SenderAdapterGetByID", "11");
            IoC.Resolve<ShipGame.Move.ICommand>("SendCommand", sender, mockCommand1.Object).Execute();
            IoC.Resolve<ShipGame.Move.ICommand>("SendCommand", sender, mockCommand2.Object).Execute();
            IoC.Resolve<ShipGame.Move.ICommand>("SendCommand", sender, mockCommand3.Object).Execute();
            var sendCommand = IoC.Resolve<ShipGame.Move.ICommand>("SendCommand", sender, softStopCommand);
            sendCommand.Execute();
            IoC.Resolve<ShipGame.Move.ICommand>("SendCommand", sender, mockCommand4.Object).Execute();
            IoC.Resolve<ShipGame.Move.ICommand>("SendCommand", sender, mockCommand5.Object).Execute();
            IoC.Resolve<ShipGame.Move.ICommand>("SendCommand", sender, mockCommand6.Object).Execute();
            Assert.False(th11.QueueIsEmpty());
            IoC.Resolve<ShipGame.Move.ICommand>("SendCommand", sender, new ActionCommand(() => { mre1.Set(); })).Execute();
            mre1.WaitOne(200);
            Assert.True(th11.QueueIsEmpty());
            Assert.True(th11.GetStop());
        }
    }
}
