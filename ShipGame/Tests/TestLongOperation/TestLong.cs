using Hwdtech;
using Hwdtech.Ioc;
using Moq;
using ShipGame.longOperation;
using ShipGame.Move;

namespace Tests.TestLong
{
    public class TestsMacroCommand
    {
        Mock<IStrategy> Strategy1 = new Mock<IStrategy>();
        Mock<IStrategy> Strategy2 = new Mock<IStrategy>();
        Mock<IStrategy> Strategy3 = new Mock<IStrategy>();
        public TestsMacroCommand()
        { 
            new InitScopeBasedIoCImplementationCommand().Execute();
            IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))).Execute();
            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Create.Command", (object[] args) => Strategy2.Object.RunStrategy(args)).Execute();
            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Commands.Repeat", (object[] args) => Strategy3.Object.RunStrategy(args)).Execute();
            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.Queue", (object[] args) => Strategy1.Object.RunStrategy(args)).Execute();
        }
        private static void CreateStrategy(Mock<IStrategy> mock1, Mock<ShipGame.Move.ICommand> mock2)
        {
            mock1.Setup(x => x.RunStrategy(It.IsAny<object[]>())).Returns(mock2.Object).Verifiable();
        }

        private static void QueueStrategy(Mock<IStrategy> mock1, Mock<IQueue<ShipGame.Move.ICommand>> mock2)
        {
            mock1.Setup(x => x.RunStrategy(It.IsAny<object[]>())).Returns(mock2.Object).Verifiable();
        }
        [Test]
        public void CreateLongtermCommandStrategyTest()
        {
            var mock1 = new Mock<ShipGame.Move.ICommand>();
            mock1.Setup(x => x.Execute());
            var mock2 = new Mock<IQueue<ShipGame.Move.ICommand>>();
            QueueStrategy(Strategy1, mock2);
            CreateStrategy(Strategy2, mock1);
            CreateStrategy(Strategy3, mock1);

            var LongCommand = new LongCommandStrategy();
            var UObj = new Mock<IUObject>();
            LongCommand.RunStrategy(It.IsAny<string>(), UObj.Object);

            Strategy1.Verify();
            Strategy2.Verify();
            Strategy3.Verify();

        }
    }
}
