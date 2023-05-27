using Hwdtech;
using Hwdtech.Ioc;
using Moq;
using ShipGame.MessageProcessing;
using ShipGame.Move;
using ICommand = Hwdtech.ICommand;

namespace Tests.TestMessageProcessing
{
    public class TestInterpretCommand
    {

        [Test]
        public void TestPush()
        {
            new InitScopeBasedIoCImplementationCommand().Execute();

            IoC.Resolve<ICommand>("Scopes.Current.Set", IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))).Execute();
            IoC.Resolve<ICommand>("IoC.Register", "GetQueue", (object[] args) => new GetQueue().RunStrategy(args)).Execute();
            IoC.Resolve<ICommand>("IoC.Register", "GetUObject", (object[] args) => new GetUObject().RunStrategy(args)).Execute();
            IoC.Resolve<ICommand>("IoC.Register", "CreateCommand", (object[] args) => new CreateCommand().RunStrategy(args)).Execute();
            IoC.Resolve<ICommand>("IoC.Register", "PushCommand", (object[] args) => new PushCommand().RunStrategy(args)).Execute();

            Dictionary<string, Queue<ShipGame.Move.ICommand>> gameDictionary = new Dictionary<string, Queue<ShipGame.Move.ICommand>>();
            Dictionary<string, IUObject> uobjectDictionary = new Dictionary<string, IUObject>();

            IoC.Resolve<ICommand>("IoC.Register", "GameDictionary", (object[] args) => gameDictionary).Execute();
            IoC.Resolve<ICommand>("IoC.Register", "UObjectDictionary", (object[] args) => uobjectDictionary).Execute();

            Mock<ShipGame.Move.ICommand> mockCommand = new Mock<ShipGame.Move.ICommand>();
            Mock<IUObject> mockUObject = new Mock<IUObject>();

            mockUObject.Setup(x => x.SetProperty(It.IsAny<string>(), It.IsAny<object>()));

            gameDictionary.Add("1", new Queue<ShipGame.Move.ICommand>());

            uobjectDictionary.Add("1", mockUObject.Object);

            Mock<IMessage> mockMessage = new Mock<IMessage>();
            mockMessage.SetupGet(x => x.Gameid).Returns("1");
            mockMessage.SetupGet(x => x.Cmd).Returns("Test");
            mockMessage.SetupGet(x => x.Args).Returns(new Dictionary<string, object> { { "Test", 1 } });
            mockMessage.SetupGet(x => x.UObjectid).Returns("1");

            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "CommandTest", (object[] args) => mockCommand.Object).Execute();

            var intepretcmd = new InterpretCommand(mockMessage.Object);
            intepretcmd.Execute();

            Assert.True(gameDictionary["1"].Count == 1);
        }

        [Test]
        public void TestGetGameException()
        {
            new InitScopeBasedIoCImplementationCommand().Execute();

            IoC.Resolve<ICommand>("Scopes.Current.Set", IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))).Execute();
            IoC.Resolve<ICommand>("IoC.Register", "GetQueue", (object[] args) => new GetQueue().RunStrategy(args)).Execute();
            IoC.Resolve<ICommand>("IoC.Register", "GetUObject", (object[] args) => new GetUObject().RunStrategy(args)).Execute();
            IoC.Resolve<ICommand>("IoC.Register", "CreateCommand", (object[] args) => new CreateCommand().RunStrategy(args)).Execute();
            IoC.Resolve<ICommand>("IoC.Register", "PushCommand", (object[] args) => new PushCommand().RunStrategy(args)).Execute();

            Dictionary<string, Queue<ShipGame.Move.ICommand>> gameDictionary = new Dictionary<string, Queue<ShipGame.Move.ICommand>>();
            Dictionary<string, IUObject> uobjectDictionary = new Dictionary<string, IUObject>();

            IoC.Resolve<ICommand>("IoC.Register", "GameDictionary", (object[] args) => gameDictionary).Execute();
            IoC.Resolve<ICommand>("IoC.Register", "UObjectDictionary", (object[] args) => uobjectDictionary).Execute();

            Mock<ShipGame.Move.ICommand> mockCommand = new Mock<ShipGame.Move.ICommand>();

            Mock<IUObject> mockUObject = new Mock<IUObject>();
            mockUObject.Setup(x => x.SetProperty(It.IsAny<string>(), It.IsAny<object>())).Verifiable();

            gameDictionary.Add("1", new Queue<ShipGame.Move.ICommand>());

            uobjectDictionary.Add("1", mockUObject.Object);

            Mock<IMessage> mockMessage = new Mock<IMessage>();
            mockMessage.SetupGet(x => x.Gameid).Returns("14");
            mockMessage.SetupGet(x => x.Cmd).Returns("Test");
            mockMessage.SetupGet(x => x.Args).Returns(new Dictionary<string, object> { { "Test", 1 } });
            mockMessage.SetupGet(x => x.UObjectid).Returns("1");

            IoC.Resolve<ICommand>("IoC.Register", "CommandTest", (object[] args) => mockCommand.Object).Execute();

            var intepretcmd = new InterpretCommand(mockMessage.Object);
            Assert.Throws<Exception>(() => { intepretcmd.Execute(); });
        }

        [Test]
        public void TestGetUObjectException()
        {
            new InitScopeBasedIoCImplementationCommand().Execute();

            IoC.Resolve<ICommand>("Scopes.Current.Set", IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))).Execute();
            IoC.Resolve<ICommand>("IoC.Register", "GetQueue", (object[] args) => new GetQueue().RunStrategy(args)).Execute();
            IoC.Resolve<ICommand>("IoC.Register", "GetUObject", (object[] args) => new GetUObject().RunStrategy(args)).Execute();
            IoC.Resolve<ICommand>("IoC.Register", "CreateCommand", (object[] args) => new CreateCommand().RunStrategy(args)).Execute();
            IoC.Resolve<ICommand>("IoC.Register", "PushCommand", (object[] args) => new PushCommand().RunStrategy(args)).Execute();

            Dictionary<string, Queue<ShipGame.Move.ICommand>> gameDictionary = new Dictionary<string, Queue<ShipGame.Move.ICommand>>();
            Dictionary<string, IUObject> uobjectDictionary = new Dictionary<string, IUObject>();

            IoC.Resolve<ICommand>("IoC.Register", "GameDictionary", (object[] args) => gameDictionary).Execute();
            IoC.Resolve<ICommand>("IoC.Register", "UObjectDictionary", (object[] args) => uobjectDictionary).Execute();

            Mock<ShipGame.Move.ICommand> mockCommand = new Mock<ShipGame.Move.ICommand>();

            Mock<IUObject> mockUObject = new Mock<IUObject>();
            mockUObject.Setup(x => x.SetProperty(It.IsAny<string>(), It.IsAny<object>())).Verifiable();

            gameDictionary.Add("1", new Queue<ShipGame.Move.ICommand>());

            uobjectDictionary.Add("1", mockUObject.Object);

            Mock<IMessage> mockMessage = new Mock<IMessage>();
            mockMessage.SetupGet(x => x.Gameid).Returns("1");
            mockMessage.SetupGet(x => x.Cmd).Returns("Test");
            mockMessage.SetupGet(x => x.Args).Returns(new Dictionary<string, object> { { "Test", 1 } });
            mockMessage.SetupGet(x => x.UObjectid).Returns("14");

            IoC.Resolve<ICommand>("IoC.Register", "CommandTest", (object[] args) => mockCommand.Object).Execute();

            var intepretcmd = new InterpretCommand(mockMessage.Object);
            Assert.Throws<Exception>(() => { intepretcmd.Execute(); });
        }

        [Test]
        public void TestMessageParamException()
        {
            new InitScopeBasedIoCImplementationCommand().Execute();

            IoC.Resolve<ICommand>("Scopes.Current.Set", IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))).Execute();
            IoC.Resolve<ICommand>("IoC.Register", "GetQueue", (object[] args) => new GetQueue().RunStrategy(args)).Execute();
            IoC.Resolve<ICommand>("IoC.Register", "GetUObject", (object[] args) => new GetUObject().RunStrategy(args)).Execute();
            IoC.Resolve<ICommand>("IoC.Register", "CreateCommand", (object[] args) => new CreateCommand().RunStrategy(args)).Execute();
            IoC.Resolve<ICommand>("IoC.Register", "PushCommand", (object[] args) => new PushCommand().RunStrategy(args)).Execute();

            Dictionary<string, Queue<ShipGame.Move.ICommand>> gameDictionary = new Dictionary<string, Queue<ShipGame.Move.ICommand>>();
            Dictionary<string, IUObject> uobjectDictionary = new Dictionary<string, IUObject>();
            IoC.Resolve<ICommand>("IoC.Register", "UObjectDictionary", (object[] args) => uobjectDictionary).Execute();
            IoC.Resolve<ICommand>("IoC.Register", "GameDictionary", (object[] args) => gameDictionary).Execute();
            
            Mock<ShipGame.Move.ICommand> mockCommand = new Mock<ShipGame.Move.ICommand>();

            Mock<IUObject> mockUObject = new Mock<IUObject>();
            mockUObject.Setup(x => x.SetProperty(It.IsAny<string>(), It.IsAny<object>())).Verifiable();

            Mock<IMessage> mockMessage = new Mock<IMessage>();
            mockMessage.SetupGet(x => x.Gameid).Throws(new Exception());
            mockMessage.SetupGet(x => x.Cmd).Returns("Test");
            mockMessage.SetupGet(x => x.Args).Returns(new Dictionary<string, object> { { "Test", 1 } });
            mockMessage.SetupGet(x => x.UObjectid).Returns("1");

            IoC.Resolve<ICommand>("IoC.Register", "CommandTest", (object[] args) => mockCommand.Object).Execute();

            var intepretcmd = new InterpretCommand(mockMessage.Object);
            Assert.Throws<Exception>(() => { intepretcmd.Execute(); });
        }
    }
}
