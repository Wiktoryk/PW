using NUnit.Framework;
using Logika;
using Dane;

namespace LogikaTests
{
    internal class DaneApi2 : DaneApiBase
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
    public class LogikaApiTest
    {
        LogikaApiBase? api;

        [SetUp]
        public void Setup()
        {
            api = new LogikaApi(new DaneApi2());
        }

        [Test]
        public void ConstructorAndGettersTest()
        {
            LogikaApiBase testApi = new LogikaApi(new DaneApi2());
            Assert.IsNotNull(testApi);
            Assert.IsNotNull(testApi.Balls);
            Assert.AreEqual(0, testApi.Balls.Count());
            Assert.AreEqual(0d, testApi.ScenaWidth, 0.01d);
            Assert.AreEqual(0d, testApi.ScenaHeight, 0.01d);
        }

        [Test]
        public void SettersTest()
        {
            Assert.IsNotNull(api);

            Assert.AreEqual(0d, api.ScenaHeight, 0.01d);
            Assert.AreEqual(0d, api.ScenaWidth, 0.01d);

            api.ScenaHeight = 90d;
            api.ScenaWidth = 100d;

            Assert.AreEqual(90d, api.ScenaHeight, 0.01d);
            Assert.AreEqual(100d, api.ScenaWidth, 0.01d);
        }

        [Test]
        public void StworzKuleTest()
        {
            Assert.IsNotNull(api);
            api.ScenaHeight = 90d;
            api.ScenaWidth = 100d;
            Assert.AreEqual(90d, api.ScenaHeight, 0.01d);
            Assert.AreEqual(100d, api.ScenaWidth, 0.01d);

            Assert.IsNotNull(api.Balls);
            Assert.AreEqual(0, api.Balls.Count());

            uint count = 10;

            api.StworzKule(count, 0.1d, 10d, 0.1d, 10d, 1d, 6d);

            Assert.AreEqual(count, api.Balls.Count());

            Array balls = api.Balls.ToArray();

            for (uint i = 0; i < count; ++i)
            {
                // Mass
                Assert.LessOrEqual(((IKula)balls.GetValue((int)i)).GetMasa(), 10d);
                Assert.GreaterOrEqual(((IKula)balls.GetValue((int)i)).GetMasa(), 0.1d);

                // Radius
                Assert.LessOrEqual(((IKula)balls.GetValue((int)i)).GetPromien(), 10d);
                Assert.GreaterOrEqual(((IKula)balls.GetValue((int)i)).GetPromien(), 0.1d);

                // Pos
                Assert.LessOrEqual(((IKula)balls.GetValue((int)i)).GetPoz().X, 100d);
                Assert.GreaterOrEqual(((IKula)balls.GetValue((int)i)).GetPoz().X, 0d);
                Assert.LessOrEqual(((IKula)balls.GetValue((int)i)).GetPoz().Y, 90d);
                Assert.GreaterOrEqual(((IKula)balls.GetValue((int)i)).GetPoz().Y, 0d);

                // Vel
                Assert.LessOrEqual(((IKula)balls.GetValue((int)i)).GetSzybkosc().X, 6d);
                Assert.GreaterOrEqual(((IKula)balls.GetValue((int)i)).GetSzybkosc().X, 1d);
                Assert.LessOrEqual(((IKula)balls.GetValue((int)i)).GetSzybkosc().Y, 6d);
                Assert.GreaterOrEqual(((IKula)balls.GetValue((int)i)).GetSzybkosc().Y, 1d);
            }
        }


        [Test]
        public void GetApiTest()
        {
            Assert.NotNull(api);

            LogikaApiBase api2 = api ?? LogikaApiBase.GetApi();

            Assert.NotNull(api2);
            Assert.AreSame(api, api2);

            api2 = LogikaApiBase.GetApi();

            Assert.NotNull(api2);
            Assert.AreNotSame(api, api2);
        }
    }
}