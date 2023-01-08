using Hwdtech;
using Moq;
using Hwdtech.Ioc;
using ShipGame.Move;

namespace Tests.Move
{
    public class StopMoveCommandTests
    {
        public StopMoveCommandTests()
        {
            new InitScopeBasedIoCImplementationCommand().Execute();
            IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))).Execute();
            
            var MockStrategy = new Mock<IStrategy>();
            var Mock_move = new Mock<ShipGame.Move.ICommand>();
            MockStrategy.Setup(c => c.RunStrategy(It.IsAny<object[]>())).Returns(Mock_move.Object);

            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Clear.Command", (object[] args) => MockStrategy.Object.RunStrategy(args)).Execute();
            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Delete.Property", (object[] args) => MockStrategy.Object.RunStrategy(args)).Execute();
            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Insert.Command", (object[] args) => MockStrategy.Object.RunStrategy(args)).Execute();
        }
        [Test]
        public void StopMoveCommandTest()
        {
            var move_endable = new Mock<IMoveCommandEndable>();
            move_endable.SetupGet(c => c.MoveCommand).Returns(new Mock<ShipGame.Move.ICommand>().Object);
            move_endable.SetupGet(c => c.ObjMove).Returns(new Mock<IUObject>().Object);
            move_endable.SetupGet(c => c.Obj).Returns(new Mock<IQueue<ShipGame.Move.ICommand>>().Object);

            ShipGame.Move.ICommand StopMove = new StopMoveCommand(move_endable.Object);
            StopMove.Execute();
            move_endable.VerifyAll();
        }
    }
}
