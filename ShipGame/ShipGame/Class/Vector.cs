namespace ShipGame.Class
{
    public class Vector
    {
        private List<int> components = new List<int>();
        public Vector(params int[] c)
        {
            for (var i = 0; i < c.Length; i++)
            {
                components.Add(c[i]);
            }
        }

        public static Vector Sum(Vector a, Vector b)
        {
            if (!SameSize(a, b))
            {
                throw new ArgumentException();
            }
            var d = new Vector();
            for (var i = 0; i < a.components.Count; i++)
            {
                d.components.Add(a.components[i] + b.components[i]);
            }
            return d;
        }

        public static bool SameSize(Vector a, Vector b)
        {
            return a.components.Count == b.components.Count;
        }

        public static bool AreEquals(Vector a, Vector b)
        {
            return a.components.SequenceEqual(b.components);
        }

    }
}
