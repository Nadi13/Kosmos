global using NUnit.Framework;
using Moq;
using ShipGame.Class;
using ShipGame.Interface;

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
            Assert.AreEqual(true, Vector.AreEquals(new Vector(5, 8), movable.Object.Position));
        }
        [Test]
        public void ExpectionGetPosition()
        {
            Mock<IMovable> movable = new Mock<IMovable>();
            movable.SetupGet<Vector>(m => m.Position).Throws<Exception>();
            ICommand mc = new MoveCommand(movable.Object);
            Assert.Throws<Exception>(() => mc.Execute());
        }
        [Test]
        public void ExpectionGetVelocity()
        {
            Mock<IMovable> movable = new Mock<IMovable>();
            movable.SetupGet<Vector>(m => m.Velocity).Throws<Exception>();
            ICommand mc = new MoveCommand(movable.Object);
            Assert.Throws<Exception>(() => mc.Execute());
        }
        [Test]
        public void ExpectionSetPosition()
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
