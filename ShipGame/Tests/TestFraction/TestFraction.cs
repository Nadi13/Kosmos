global using NUnit.Framework;
using ShipGame.Rotate;

namespace Tests.TestFraction
{
    public class TestFraction
    {

        [Test]
        public void TestSum()
        {
            var a = new Fraction(1, 2);
            var b = new Fraction(1, 3);
            var c = new Fraction(2, 2);
            var d = new Fraction(1, 6);
            Assert.AreEqual(true, Fraction.AreEquals(new Fraction(5, 6), Fraction.Sum(a, b)));
            Assert.AreEqual(true, Fraction.AreEquals(new Fraction(2, 3), Fraction.Sum(a, d)));
            Assert.AreEqual(false, Fraction.AreEquals(new Fraction(4, 2), Fraction.Sum(a, c)));

        }

        [Test]
        public void TestSub()
        {
            var a = new Fraction(-5, 10);
            var b = new Fraction(1, 6);
            var c = new Fraction(-8, 6);
            Assert.AreEqual(true, Fraction.AreEquals(new Fraction(-2, 3), Fraction.Sub(a, b)));
            Assert.AreEqual(false, Fraction.AreEquals(new Fraction(-7, 6), Fraction.Sub(b, c)));
        }

        [Test]
        public void TestMulti()
        {
            var a = new Fraction(1, -2);
            Assert.AreEqual(true, Fraction.AreEquals(new Fraction(-2, 1), Fraction.Multi(4, a)));
            Assert.AreEqual(false, Fraction.AreEquals(new Fraction(5, -2), Fraction.Multi(-5, a)));
        }

        [Test]
        public void TestForm()
        {
            var a = new Fraction(2, 4);
            var b = new Fraction(3, 4);
            Assert.AreEqual(true, Fraction.AreEquals(new Fraction(1, 2), Fraction.Form(a)));
            Assert.AreEqual(true, Fraction.AreEquals(new Fraction(3, 4), Fraction.Form(b)));
            Assert.AreEqual(false, Fraction.AreEquals(new Fraction(2, 4), Fraction.Form(a)));
        }
    }
}
