namespace ShipGame.Rotate
{
    public class Fraction
    {
        private int numerator;
        private int denominator;
        public Fraction(int numerator, int denominator)
        {
            this.numerator = numerator;
            this.denominator = denominator;

        }

        public static Fraction Sum(Fraction a, Fraction b)
        {
            var c = new Fraction(1, 1);
            if (a.denominator == b.denominator)
            {
                c.numerator = a.numerator + b.numerator;
                c.denominator = a.denominator;
            }
            else
            {
                c.numerator = a.numerator * b.denominator + b.numerator * a.denominator;
                c.denominator = a.denominator * b.denominator;
            }
            Fraction.Reduction(c);
            return c;
        }
        public static Fraction operator +(Fraction a, Fraction b)
        {
            return Sum(a, b);
        }

        public static Fraction Sub(Fraction a, Fraction b)
        {
            var c = new Fraction(1, 1);
            if (a.denominator == b.denominator)
            {
                c.numerator = a.numerator - b.numerator;
                c.denominator = a.denominator;
            }
            else
            {
                c.numerator = a.numerator * b.denominator - b.numerator * a.denominator;
                c.denominator = a.denominator * b.denominator;
            }
            Fraction.Reduction(c);
            return c;
        }
        public static Fraction operator -(Fraction a, Fraction b)
        {
            return Sub(a, b);
        }
        public static Fraction Multi(int a, Fraction b)
        {
            var c = new Fraction(1, 1);
            c.numerator = a * b.numerator;
            c.denominator = b.denominator;
            Fraction.Reduction(c);
            return c;
        }
        public static Fraction operator *(int a, Fraction b)
        {
            return Multi(a, b);
        }

        public static Fraction Reduction(Fraction a)
        {
            int n = Math.Abs(a.numerator);
            int d = Math.Abs(a.denominator);
            while(n != d)
            {
                if(n > d)
                {
                    n = n - d;
                }
                else
                {
                    d = d - n;
                }
            }
            int nod = d;
            a.numerator = a.numerator / nod;
            a.denominator = a.denominator / nod;
            if ((a.denominator < 0))
            {
                a.numerator = -1 * (a.numerator);
                a.denominator = Math.Abs(a.denominator);
            }
            return (a);
        }

        public static bool operator ==(Fraction a, Fraction b)
        {
            return a.numerator == b.numerator && a.denominator == b.denominator;
        }

        public static bool operator !=(Fraction a, Fraction b)
        {
            return (a == b) ? false : true;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(numerator, denominator);
        }

        public override bool Equals(object? obj)
        {
            return obj is Fraction;
        }

    }
}
