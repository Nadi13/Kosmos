using Hwdtech;
using Hwdtech.Ioc;
using Moq;
using ShipGame.MacroCommands;
using ShipGame.Move;

namespace Tests.TestMacroCommand
{
    public class TestsMacroCommand
    {
        Mock<IStrategy> Strategy1 = new Mock<IStrategy>();
        Mock<IStrategy> Strategy2 = new Mock<IStrategy>();
        Mock<IStrategy> Strategy3 = new Mock<IStrategy>();
        Mock<IStrategy> result = new Mock<IStrategy>();

        [Test]
        public void PositiveMacroCommand()
        {
            var mock1 = new Mock<ShipGame.Move.ICommand>();
            var mock2 = new Mock<ShipGame.Move.ICommand>();
            var commands = new List<ShipGame.Move.ICommand> { mock1.Object, mock2.Object };
            mock1.Setup(command => command.Execute()).Verifiable();
            mock2.Setup(command => command.Execute()).Verifiable();
            var mCommand = new MacroCommand(commands);
            mCommand.Execute();
        }
        [Test]
        public void ExceptionMacroCommand()
        {
            var mock1 = new Mock<ShipGame.Move.ICommand>();
            var mock2 = new Mock<ShipGame.Move.ICommand>();
            var commands = new List<ShipGame.Move.ICommand> { mock1.Object, mock2.Object };
            mock1.Setup(command => command.Execute()).Throws<Exception>();
            mock2.Setup(command => command.Execute()).Verifiable();
            var mCommand = new MacroCommand(commands);
            Assert.Throws<Exception>(() => mCommand.Execute());
        }
        public TestsMacroCommand()
        {
            new InitScopeBasedIoCImplementationCommand().Execute();
            IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))).Execute();

            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Config.MacroCommand.Move", (object[] args) => Strategy1.Object.RunStrategy(args)).Execute();
            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Command1", (object[] args) => Strategy2.Object.RunStrategy(args)).Execute();
            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Command2", (object[] args) => Strategy3.Object.RunStrategy(args)).Execute();
            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "SimpleMacroCommand", (object[] args) => result.Object.RunStrategy(args)).Execute();
        }
        private static void SetupStrategy(Mock<IStrategy> mock1, List<string> list)
        {
            mock1.Setup(strategy => strategy.RunStrategy(It.IsAny<object[]>())).Returns(list).Verifiable();
        }
        private static void SetupStategyObject(Mock<IStrategy> mock1, Mock<ShipGame.Move.ICommand> mock2)
        {
            mock1.Setup(strategy => strategy.RunStrategy(It.IsAny<object[]>())).Returns(mock2.Object).Verifiable();

        }
        private static void SetupStrategyResult(Mock<IStrategy> mock1, MacroCommand mock2)
        {
            mock1.Setup(strategy => strategy.RunStrategy(It.IsAny<object>())).Returns(mock2).Verifiable();
        }

        [Test]
        public void PositiveMacroCommandStrategy()
        {
            var UObject1 = new Mock<IUObject>();
            var mock1 = new Mock<ShipGame.Move.ICommand>();
            var mock2 = new Mock<ShipGame.Move.ICommand>();
            var StrategyMacroCommand = new MacroCommandStrategy();
            SetupStrategy(Strategy1, new List<string> { "Command1", "Command2" });
            SetupStategyObject(Strategy2, mock1);
            SetupStategyObject(Strategy3, mock2);
            var commands = new List<ShipGame.Move.ICommand> {mock1.Object, mock2.Object };
            var macroCommand = new MacroCommand(commands);
            SetupStrategyResult(result, macroCommand);
            StrategyMacroCommand.RunStrategy(UObject1.Object, "Move");
            Strategy1.Verify();
            Strategy2.Verify();
            Strategy3.Verify();
            result.Verify();
        }

    }
}

