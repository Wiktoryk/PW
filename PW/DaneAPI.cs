using System;
using System.Numerics;

namespace Dane
{
    public abstract class DaneApiBase
    {
        public abstract Kula StworzKule(Pozycja minPoz, Pozycja maxPoz, double minSzybkosc, double maxSzybkosc);
        public abstract Scena StworzScene(double szerokosc, double wysokosc);

        public static DaneApiBase GetApi()
        {
            return new DaneApi();
        }
    }
    internal class DaneApi : DaneApiBase
    {
        public override Kula StworzKule(Pozycja minPoz, Pozycja maxPoz, double minSzybkosc, double maxSzybkosc)
        {
            Random rnd = new();
            double radius = 20;

            double minX = minPoz.X + radius;
            double maxX = maxPoz.X - radius;

            double minY = minPoz.Y + radius;
            double maxY = maxPoz.Y - radius;

            double temp;
            if (minX > maxX)
            {
                temp = minX;
                minX = maxX;
                maxX = temp;
            }

            if (minY > maxY)
            {
                temp = minY;
                minY = maxY;
                maxY = temp;
            }

            Pozycja poz = new(rnd.NextDouble() * (maxX - minX) + minX, rnd.NextDouble() * (maxY - minY) + minY);
            Pozycja szybkosc = new(rnd.NextDouble() * (maxSzybkosc - minSzybkosc) + minSzybkosc, rnd.NextDouble() * (maxSzybkosc - minSzybkosc) + minSzybkosc);

            return new Kula(rnd.NextInt64(), radius, poz, szybkosc);
        }

        public override Scena StworzScene(double szerokosc, double wysokosc)
        {
            return new Scena(szerokosc, wysokosc);
        }
    }
}
