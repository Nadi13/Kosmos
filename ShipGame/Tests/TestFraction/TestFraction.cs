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
            var c = new Fraction(5, 6);
            var d = new Fraction(2, 6);
            Assert.True(c == a + b);
            Assert.False(d == a + c);

        }

        [Test]
        public void TestSub()
        {
            var a = new Fraction(-5, 10);
            var b = new Fraction(1, 6);
            var c = new Fraction(-8, 6);
            var d = new Fraction(-2, 3);
            Assert.True(d == a - b);
            Assert.False(a == b - c);
        }

        [Test]
        public void TestMulti()
        {
            var a = new Fraction(1, -2);
            var b = new Fraction(-2, 1);

            Assert.True(b == 4 * a);
            Assert.False(b == -5 * a);
        }

        [Test]
        public void TestReduction()
        {
            var a = new Fraction(2, -4);
            var b = new Fraction(-1, 2);
            var c = new Fraction(2, 4);
            Assert.True(b == Fraction.Reduction(a));
            Assert.False(c == Fraction.Reduction(a));
        }

        [Test]
        public void TestEquality()
        {
            var a = new Fraction(1, 2);
            var b = new Fraction(1, 2);
            var c = new Fraction(1, 5);
            Assert.True(a == b);
            Assert.False(a == c);
        }

        [Test]
        public void TestNoEquality()
        {
            var a = new Fraction(1, 2);
            var b = new Fraction(1, 2);
            var c = new Fraction(1, 5);
            Assert.True(a != c);
            Assert.False(a != b);
        }
        [Test]
        public void TestGetHashCode()
        {
            var a = new Fraction(1, 2);
            Assert.IsInstanceOf<int>(a.GetHashCode());
        }
        [Test]
        public void TestEquals()
        {
            var a = new Fraction(2, 3);
            var b = new Fraction(1, 2);
            var c = 3;
            Assert.True(a.Equals(b));
            Assert.False(a.Equals(c));
        }
    }
}
