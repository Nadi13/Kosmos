using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShipGame.Class
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
            Fraction.Form(c);
            return c;
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
            Fraction.Form(c);
            return c;
        }
        public static Fraction Multi(int a, Fraction b)
        {
            var c = new Fraction(1, 1);
            c.numerator = a * b.numerator;
            c.denominator = b.denominator;
            Fraction.Form(c);
            return c;
        }

        public static Fraction Form(Fraction a)
        {
            int max = 0;
            if (a.numerator > a.denominator)
            {
                max = Math.Abs(a.denominator);
            }
            else
            {
                max = Math.Abs(a.numerator);
            }
            for (int i = max; i >= 2; i--)
            {
                if ((a.numerator % i == 0) & (a.denominator % i == 0))
                {
                    a.numerator = a.numerator / i;
                    a.denominator = a.denominator / i;
                }
            }
            if ((a.denominator < 0))
            {
                a.numerator = -1 * (a.numerator);
                a.denominator = Math.Abs(a.denominator);
            }
            return (a);
        }
        public static bool AreEquals(Fraction a, Fraction b)
        {
            if (a.numerator == b.numerator && a.denominator == b.denominator)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

    }
}
