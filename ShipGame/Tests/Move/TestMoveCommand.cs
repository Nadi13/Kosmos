global using NUnit.Framework;
using Moq;
using ShipGame.Move;

namespace Tests.Move
{
    public class Tests
    {
        [Test]
        public void PositiveMove()
        {
            Mock<IMovable> movable = new Mock<IMovable>();
            movable.SetupProperty<Vector>(m => m.Position, new Vector(12, 5));
            movable.SetupGet<Vector>(m => m.Velocity).Returns(new Vector(-7, 3));
            ICommand mc = new MoveCommand(movable.Object);
            mc.Execute();
            Assert.True(new Vector(5, 8) == movable.Object.Position);
        }
        [Test]
        public void ExceptionGetPosition()
        {
            Mock<IMovable> movable = new Mock<IMovable>();
            movable.SetupGet<Vector>(m => m.Position).Throws<Exception>();
            ICommand mc = new MoveCommand(movable.Object);
            Assert.Throws<Exception>(() => mc.Execute());
        }
        [Test]
        public void ExceptionGetVelocity()
        {
            Mock<IMovable> movable = new Mock<IMovable>();
            movable.SetupGet<Vector>(m => m.Velocity).Throws<Exception>();
            ICommand mc = new MoveCommand(movable.Object);
            Assert.Throws<Exception>(() => mc.Execute());
        }
        [Test]
        public void ExceptionSetPosition()
        {
            Mock<IMovable> movable = new Mock<IMovable>();
            movable.SetupGet<Vector>(m => m.Position).Returns(new Vector(12, 5));
            movable.SetupGet<Vector>(m => m.Velocity).Returns(new Vector(-7, 3));
            movable.SetupSet<Vector>(m => m.Position = It.IsAny<Vector>()).Throws<Exception>();
            ICommand mc = new MoveCommand(movable.Object);
            Assert.Throws<Exception>(() => mc.Execute());
        }

    }
}
