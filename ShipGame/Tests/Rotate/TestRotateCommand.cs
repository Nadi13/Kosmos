global using NUnit.Framework;
using Moq;
using ShipGame.Rotate;


namespace Tests.Rotate
{
    public class RotateTests
    {
        [Test]
        public void PositiveRotate()
        {

            Mock<IRotatable> rotatable = new Mock<IRotatable>();
            rotatable.SetupProperty<Fraction>(r => r.Angle, new Fraction(90, 2));
            rotatable.SetupGet<Fraction>(r => r.AngleVelocity).Returns(new Fraction(90, 1));
            ICommand rc = new RotateCommand(rotatable.Object);
            rc.Execute();
            Assert.True(new Fraction(135, 1) == rotatable.Object.Angle);

        }
        [Test]
        public void ExceptionGetAngle()
        {
            Mock<IRotatable> rotatable = new Mock<IRotatable>();
            rotatable.SetupGet<Fraction>(r => r.Angle).Throws<Exception>();
            ICommand rc = new RotateCommand(rotatable.Object);
            Assert.Throws<Exception>(() => rc.Execute());
        }
        [Test]
        public void ExceptionGetAngleVelocity()
        {
            Mock<IRotatable> rotatable = new Mock<IRotatable>();
            rotatable.SetupGet<Fraction>(r => r.AngleVelocity).Throws<Exception>();
            ICommand rc = new RotateCommand(rotatable.Object);
            Assert.Throws<Exception>(() => rc.Execute());
        }
        [Test]
        public void ExceptionSetAngle()
        {
            Mock<IRotatable> rotatable = new Mock<IRotatable>();
            rotatable.SetupGet<Fraction>(r => r.Angle).Returns(new Fraction(90, 2));
            rotatable.SetupGet<Fraction>(r => r.AngleVelocity).Returns(new Fraction(90, 1));
            rotatable.SetupSet<Fraction>(r => r.Angle = It.IsAny<Fraction>()).Throws<Exception>();
            ICommand rc = new RotateCommand(rotatable.Object);
            Assert.Throws<Exception>(() => rc.Execute());
        }

    }
}
