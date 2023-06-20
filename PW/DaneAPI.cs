using System.Numerics;
using System;

namespace Dane
{
    public abstract class DaneApiBase
    {
        public abstract Kula StworzKule(double minMass, double maxMass, double minRadius, double maxRadius, Pozycja minPos, Pozycja maxPos, double minVel, double maxVel);
        public abstract Scena StworzScene(double width, double height);

        public static DaneApiBase GetApi()
        {
            return new DaneApi();
        }
    }
    internal class DaneApi : DaneApiBase
    {
        public override Kula StworzKule(double minMass, double maxMass, double minRadius, double maxRadius, Pozycja minPos, Pozycja maxPos, double minVel, double maxVel)
        {
            Random rnd = new();
            double randVal = rnd.NextDouble();
            double radius = randVal * (maxRadius - minRadius) + minRadius;

            double mass = randVal * (maxMass - minMass) + minMass;

            double minX = minPos.X + radius;
            double maxX = maxPos.X - radius;

            double minY = minPos.Y + radius;
            double maxY = maxPos.Y - radius;

            if (minX > maxX)
            {
                (minX, maxX) = (maxX, minX);
            }

            if (minY > maxY)
            {
                (minY, maxY) = (maxY, minY);
            }

            Pozycja pos = new(rnd.NextDouble() * (maxX - minX) + minX, rnd.NextDouble() * (maxY - minY) + minY);
            Pozycja vel = new(rnd.NextDouble() * (maxVel - minVel) + minVel, rnd.NextDouble() * (maxVel - minVel) + minVel);

            return new Kula(rnd.NextInt64(), mass, radius, pos, vel);
        }

        public override Scena StworzScene(double width, double height)
        {
            return new Scena(width, height);
        }
    }
}
