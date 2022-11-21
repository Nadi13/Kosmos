namespace ShipGame.Move
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

        public int this[int index]
        {
            get
            {
                return components[index];
            }
            set
            {
                this.components[index] = value;
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
        public static Vector operator +(Vector a, Vector b)
        {
            return Sum(a, b);
        }

        public static bool SameSize(Vector a, Vector b)
        {
            return a.components.Count == b.components.Count;
        }

        public static bool operator ==(Vector a, Vector b)
        {
            return a.components.SequenceEqual(b.components);
        }
        public static bool operator !=(Vector a, Vector b)
        {
            return (a == b) ? false : true;
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(components);
        }
        public override bool Equals(object ? obj)
        {
            return obj is Vector;
        }
    }
}
