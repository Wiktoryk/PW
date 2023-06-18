using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dane
{
    public struct Pozycja
    {
        public double X { get; set; }
        public double Y { get; set; }

        public Pozycja(double x, double y)
        {
            X = x;
            Y = y;
        }

        public static Pozycja operator *(Pozycja left, int right)
        {
            return new Pozycja { X = left.X * right, Y = left.Y * right };
        }
        public static Pozycja operator *(Pozycja left, float right)
        {
            return new Pozycja { X = left.X * right, Y = left.Y * right };
        }
        public static Pozycja operator *(Pozycja left, double right)
        {
            return new Pozycja { X = left.X * right, Y = left.Y * right };
        }
        public static Pozycja operator *(double left, Pozycja right)
        {
            return new Pozycja { X = right.X * left, Y = right.Y * left };
        }

        public static Pozycja operator /(Pozycja left, double right)
        {
            return new Pozycja { X = left.X / right, Y = left.Y / right };
        }

        public static Pozycja operator +(Pozycja left, Pozycja right)
        {
            return new Pozycja { X = left.X + right.X, Y = left.Y + right.Y };
        }
        public static Pozycja operator +(Pozycja pos)
        {
            return new Pozycja { X = pos.X, Y = pos.Y };
        }

        public static Pozycja operator -(Pozycja left, Pozycja right)
        {
            return new Pozycja { X = left.X - right.X, Y = left.Y - right.Y };
        }
        public static Pozycja operator -(Pozycja pos)
        {
            return new Pozycja { X = -pos.X, Y = -pos.Y };
        }

        public static bool operator ==(Pozycja left, Pozycja right)
        {
            return left.X == right.X && left.Y == right.Y;
        }
        public static bool operator !=(Pozycja left, Pozycja right)
        {
            return left.X != right.X || left.Y != right.Y;
        }

        public static Pozycja Zero { get { return new Pozycja { X = 0, Y = 0 }; } }
        public static Pozycja One { get { return new Pozycja { X = 1, Y = 1 }; } }
        //public static Pos2D Right { get { return new Pos2D { X = 1, Y = 0 }; } }
        //public static Pos2D Down { get { return new Pos2D { X = 0, Y = 1 }; } }

        public double Length
        {
            get
            {
                return Math.Sqrt(X * X + Y * Y);
            }
        }
        public Pozycja Normalize
        {
            get
            {
                double dist = Length;
                return new Pozycja { X = X / dist, Y = Y / dist };
            }
        }
        public double DotProduct(Pozycja other)
        {
            return this.X * other.X + this.Y * other.Y;
        }

        public override bool Equals(object? obj)
        {
            if (obj is not Pozycja) return false;
            return this == (Pozycja)obj;
        }

        public override int GetHashCode()
        {
            return X.GetHashCode() ^ Y.GetHashCode();
        }
    }
}
