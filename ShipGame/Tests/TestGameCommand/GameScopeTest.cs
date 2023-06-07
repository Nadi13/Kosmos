using Hwdtech.Ioc;
using Hwdtech;
using System.Collections.Concurrent;
using Moq;
using ShipGame.Game;
using ICommand = ShipGame.Move.ICommand;
using System.Diagnostics;
using ShipGame.Move;
using ShipGame.Server;
using SpaceBattle.ServerStrategies;

namespace Tests.TestGameCommand
{
    public class GameScopeTest
    {
        public GameScopeTest()
        {
            new InitScopeBasedIoCImplementationCommand().Execute();
            IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))).Execute();

            var threadDict = new ConcurrentDictionary<string, ServerThread>();
            var senderDict = new ConcurrentDictionary<string, ISender>();
            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "SenderDictionary", (object[] _) => { return senderDict; }).Execute();
            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "ThreadDictionary", (object[] _) => { return threadDict; }).Execute();
            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "ThreadIDSenderMapping", (object[] _) => senderDict).Execute();
            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "SenderAdapterGetByID", (object[] id) => senderDict[(string)id[0]]).Execute();
            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "ServerThreadGetByID", (object[] id) => threadDict[(string)id[0]]).Execute();
            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "SenderGetByID", (object[] id) => senderDict[(string)id[0]]).Execute();


            var sendCommandStrategy = new SendCommandStrategy();
            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "SendCommand", (object[] args) => sendCommandStrategy.RunStrategy(args)).Execute();

            var threadgamedict = new ConcurrentDictionary<string, string>();
            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Storage.ThreadByGameID", (object[] args) => threadgamedict).Execute();
            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Storage.GetThreadByGameID", (object[] args) =>
            {
                var dict = IoC.Resolve<ConcurrentDictionary<string, string>>("Storage.ThreadByGameID");
                return dict[(string)args[0]];
            }
            ).Execute();
            var exceptionCommandStrategyDictionary = new Dictionary<Type, Dictionary<ICommand, IStrategy>>();
            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Dictionary.Handler.Exception", (object[] args) => { return exceptionCommandStrategyDictionary; }).Execute();
        }
        [Test]
        public void GameCommandWithExceptionTest()
        {
            var handleExceptionStrategy = new HandlStrategy();
            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "HandleException", (object[] args) => handleExceptionStrategy.RunStrategy(args)).Execute();
            var exceptCommandStrategyDictionary = IoC.Resolve<Dictionary<Type, Dictionary<ICommand, IStrategy>>>("Dictionary.Handler.Exception");
            var commandStrategyDictionary = new Dictionary<ICommand, IStrategy>();

            var argException = new ArgumentException();
            var mockCommand = new Mock<ICommand>();
            mockCommand.Setup(_command => _command.Execute()).Throws(argException);

            var verifyCommand = new ActionCommand(() => { Assert.Throws<ArgumentException>(() => mockCommand.Object.Execute()); });

            var mockStrategy = new Mock<IStrategy>();
            mockStrategy.Setup(_strategy => _strategy.RunStrategy(It.IsAny<object[]>())).Returns(verifyCommand).Verifiable();

            commandStrategyDictionary.TryAdd(mockCommand.Object, mockStrategy.Object);
            exceptCommandStrategyDictionary.TryAdd(argException.GetType(), commandStrategyDictionary);

            var quantum = new TimeSpan(0, 0, 0, 0, 250);
            var quantumStrategy = new Mock<IStrategy>();
            quantumStrategy.Setup(_strategy => _strategy.RunStrategy(It.IsAny<object[]>())).Returns(quantum);
            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "QuantumForGame", (object[] args) => quantumStrategy.Object.RunStrategy()).Execute();
            var queue = new Queue<ICommand>();
            queue.Enqueue(mockCommand.Object);
            var gameCommand = new GameCommand("game1", queue);
            gameCommand.Execute();
            mockStrategy.Verify();
        }
        [Test]
        public void GameCommandWithExceptionWithoutFindTest()
        {
            var exceptCommandStrategyDictionary = IoC.Resolve<Dictionary<Type, Dictionary<ICommand, IStrategy>>>("Dictionary.Handler.Exception");
            var commandStrategyDictionary = new Dictionary<ICommand, IStrategy>();

            var argException = new ArgumentException();
            var mockCommand = new Mock<ICommand>();
            mockCommand.Setup(_command => _command.Execute()).Throws(argException);

            var verifyCommand = new ActionCommand(() => { Assert.Throws<ArgumentException>(() => mockCommand.Object.Execute()); });

            var mockStrategy = new Mock<IStrategy>();
            mockStrategy.Setup(_strategy => _strategy.RunStrategy(It.IsAny<object[]>())).Returns(verifyCommand).Verifiable();

            var queue = new Queue<ICommand>();
            queue.Enqueue(mockCommand.Object);
            var gameCommand = new GameCommand("game1", queue);
            Assert.Throws<Exception>(()=>gameCommand.Execute());
        }
        [Test]
        public void GameCommandWithExceptionWithoutFindAnotherTest()
        {
            var exceptCommandStrategyDictionary = IoC.Resolve<Dictionary<Type, Dictionary<ICommand, IStrategy>>>("Dictionary.Handler.Exception");
            var commandStrategyDictionary = new Dictionary<ICommand, IStrategy>();

            var Exception1 = new Exception();
            var mockCommand = new Mock<ICommand>();
            mockCommand.Setup(_command => _command.Execute()).Throws(Exception1);

            var verifyCommand = new ActionCommand(() => { Assert.Throws<Exception>(() => mockCommand.Object.Execute()); });

            var mockStrategy = new Mock<IStrategy>();
            mockStrategy.Setup(_strategy => _strategy.RunStrategy(It.IsAny<object[]>())).Returns(verifyCommand).Verifiable();

            exceptCommandStrategyDictionary.TryAdd(Exception1.GetType(), commandStrategyDictionary);

            var queue = new Queue<ICommand>();
            queue.Enqueue(mockCommand.Object);
            var gameCommand = new GameCommand("game1", queue);
            Assert.Throws<Exception>(() => gameCommand.Execute());
        }
    }
}
