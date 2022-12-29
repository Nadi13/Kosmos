using Hwdtech;
using Moq;
using Hwdtech.Ioc;
using ShipGame.Move;

namespace Tests.Move
{
    public class StartMoveCommandTests
    {
        public StartMoveCommandTests()
        {
            new InitScopeBasedIoCImplementationCommand().Execute();
            IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))).Execute();
        }

        private static void MoveCommandStsrtableHasTarget(Mock <IMoveCommandStartable> mock1, Mock<IUObject> mock2)
        {
            mock1.SetupGet(x => x.Target).Returns(mock2.Object).Verifiable();
        }

        private static void MoveCommandStartableHasProperties(Mock<IMoveCommandStartable> mock1)
        {
            mock1.SetupGet(x => x.Properties).Returns(new Dictionary<string, object>() { { "Velocity", new Vector(1, 1) } }).Verifiable();
        }

        private static void LetThrowExceptionToGetUObject(Mock<IMoveCommandStartable> mock1)
        {
            mock1.SetupGet(x => x.Target).Throws<Exception>().Verifiable();
        }

        private static void LetThrowExceptionToGetVelocity(Mock<IMoveCommandStartable> mock1)
        {
            mock1.SetupGet(x => x.Properties).Throws<Exception>().Verifiable();
        }
   
        [Test]
        public void PositiveStartMoveCommand()
        {
            var MockCommand = new Mock<ShipGame.Move.ICommand>();
            MockCommand.Setup(x => x.Execute());
            var MockStrategy = new Mock<IStrategy>();
            MockStrategy.Setup(x => x.RunStrategy(It.IsAny<object[]>())).Returns(MockCommand.Object);
            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Сommon.SetProperty", (object[] args) => MockStrategy.Object.RunStrategy(args)).Execute();
            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Long.Move", (object[] args) => MockStrategy.Object.RunStrategy(args)).Execute();
            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Queue.Push", (object[] args) => MockStrategy.Object.RunStrategy(args)).Execute();
            
            var mock1 = new Mock<IMoveCommandStartable>();
            var mock2 = new Mock<IUObject>();
            MoveCommandStsrtableHasTarget(mock1, mock2);
            MoveCommandStartableHasProperties(mock1);
            ShipGame.Move.ICommand smc = new StartMoveCommand(mock1.Object);
            smc.Execute();
            mock1.Verify();
        }
        [Test]
        public void ExceptionToGetUObject()
        {
            var mock1 = new Mock<IMoveCommandStartable>();
            LetThrowExceptionToGetUObject(mock1);
            MoveCommandStartableHasProperties(mock1);
            ShipGame.Move.ICommand smc = new StartMoveCommand(mock1.Object);
            Assert.Throws<Exception>(() => smc.Execute());
        }
        [Test]
        public void ExceptionToGetVelocity()
        {
            var mock1 = new Mock<IMoveCommandStartable>();
            var mock2 = new Mock<IUObject>();
            MoveCommandStsrtableHasTarget(mock1, mock2);
            LetThrowExceptionToGetVelocity(mock1);
            ShipGame.Move.ICommand smc = new StartMoveCommand(mock1.Object);
            Assert.Throws<Exception>(() => smc.Execute());
        }
    }
}
