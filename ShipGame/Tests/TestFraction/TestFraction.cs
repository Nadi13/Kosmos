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
            Assert.AreEqual(true, Fraction.AreEquals(new Fraction(5, 6), a + b));
            Assert.AreEqual(true, Fraction.AreEquals(new Fraction(2, 3), a + d));
            Assert.AreEqual(false, Fraction.AreEquals(new Fraction(4, 2), a + c));

        }

        [Test]
        public void TestSub()
        {
            var a = new Fraction(-5, 10);
            var b = new Fraction(1, 6);
            var c = new Fraction(-8, 6);
            Assert.AreEqual(true, Fraction.AreEquals(new Fraction(-2, 3), a - b));
            Assert.AreEqual(false, Fraction.AreEquals(new Fraction(-7, 6), b - c));
        }

        [Test]
        public void TestMulti()
        {
            var a = new Fraction(1, -2);
            Assert.AreEqual(true, Fraction.AreEquals(new Fraction(-2, 1), 4 * a));
            Assert.AreEqual(false, Fraction.AreEquals(new Fraction(5, -2), -5 * a));
        }

        [Test]
        public void TestReduction()
        {
            var a = new Fraction(2, -4);
            var b = new Fraction(3, 4);
            Assert.AreEqual(true, Fraction.AreEquals(new Fraction(-1, 2), Fraction.Reduction(a)));
            Assert.AreEqual(true, Fraction.AreEquals(new Fraction(3, 4), Fraction.Reduction(b)));
            Assert.AreEqual(false, Fraction.AreEquals(new Fraction(2, 4), Fraction.Reduction(a)));
        }
    }
}
