global using NUnit.Framework;
using ShipGame.Move;

namespace Tests.TestVector
{
    public class Tests
    {

        [Test]
        public void TestSameSize()
        {
            var a = new Vector(1, 2, 3, 4, 5);
            var b = new Vector(1, 2, 3);
            var c = new Vector(0, 3, 2, 3, 4);
            Assert.That(Vector.SameSize(a, c), Is.EqualTo(true));
            Assert.That(Vector.SameSize(a, b), Is.EqualTo(false));

        }

        [Test]
        public void TestSum()
        {
            var a = new Vector(1, 2, 3, 5);
            var b = new Vector(1, 2, 5, 5);
            var c = new Vector(2, 4, 8, 10);
            var d = new Vector(1, 2, 3);
            Assert.That(Vector.AreEquals(c, a + b), Is.EqualTo(true));
            Assert.Throws<ArgumentException>(() => Vector.Sum(c, d));
        }
    }
}
