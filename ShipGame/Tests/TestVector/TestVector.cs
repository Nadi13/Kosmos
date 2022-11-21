global using NUnit.Framework;
using ShipGame.Move;

namespace Tests.TestVector
{
    public class Tests
    {
        [Test]
        public void TestNewVector()
        {
            Vector a = new Vector(1, 2);
            Assert.NotNull(a);
        }

        [Test]
        public void TestSameSize()
        {
            var a = new Vector(1, 2, 3, 4, 5);
            var b = new Vector(1, 2, 3);
            var c = new Vector(0, 3, 2, 3, 4);
            Assert.True(Vector.SameSize(a, c));
            Assert.False(Vector.SameSize(a, b));

        }

        [Test]
        public void TestSum()
        {
            var a = new Vector(1, 2, 3, 5);
            var b = new Vector(1, 2, 5, 5);
            var c = new Vector(2, 4, 8, 10);
            var d = new Vector(1, 2, 3);
            Assert.True(c == a + b);
            Assert.Throws<ArgumentException>(() => Vector.Sum(c, d));
        }

        [Test]
        public void TestEquality()
        {
            var a = new Vector(1, 2, 3);
            var b = new Vector(1, 2, 3);
            var c = new Vector(1, 2, 2);
            Assert.True(a == b);
            Assert.False(a == c);
        }

        [Test]
        public void TestNoEquality()
        {
            var a = new Vector(2, 6);
            var b = new Vector(2, 6);
            var c = new Vector(2, 1, 6);
            Assert.True(a != c);
            Assert.False(a != b);
        }
        [Test]
        public void TestGetHashCode()
        {
            var a = new Vector(1, 2);
            Assert.IsInstanceOf<int>(a.GetHashCode());
        }
    }

}
