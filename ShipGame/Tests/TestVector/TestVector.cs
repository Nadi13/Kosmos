global using NUnit.Framework;
using ShipGame.Class;

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
            Assert.AreEqual(true, Vector.SameSize(a, c));
            Assert.AreEqual(false, Vector.SameSize(a, b));

        }

        [Test]
        public void TestSum()
        {
            var a = new Vector(1, 2, 3, 5);
            var b = new Vector(1, 2, 5, 5);
            var c = new Vector(2, 4, 8, 10);
            var d = new Vector(1, 2, 3);
            Assert.AreEqual(true, Vector.AreEquals(c, Vector.Sum(a, b)));
            Assert.Throws<ArgumentException>(() => Vector.Sum(c, d));
        }
    }
}
