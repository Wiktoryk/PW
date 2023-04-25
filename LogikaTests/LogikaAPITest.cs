using NUnit.Framework;
using Logika;
using Dane;

namespace LogikaTests
{
    public class LogikaApiTest
    {
        LogikaApiBase? api;

        [SetUp]
        public void Setup()
        {
            api = new LogikaApi(null);
        }

        [Test]
        public void ConstructorAndGettersTest()
        {
            LogikaApiBase testApi = new LogikaApi(null);
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

            api.StworzKule(count, 1d, 6d);

            Assert.AreEqual(count, api.Balls.Count());

            Array balls = api.Balls.ToArray();

            for (uint i = 0; i < count; ++i)
            {
                Assert.LessOrEqual(((IKula)balls.GetValue((int)i)).GetPromien(), 20d);
                Assert.GreaterOrEqual(((IKula)balls.GetValue((int)i)).GetPromien(), 0.1d);

                Assert.LessOrEqual(((IKula)balls.GetValue((int)i)).GetPoz().X, 100d);
                Assert.GreaterOrEqual(((IKula)balls.GetValue((int)i)).GetPoz().X, 0d);
                Assert.LessOrEqual(((IKula)balls.GetValue((int)i)).GetPoz().Y, 90d);
                Assert.GreaterOrEqual(((IKula)balls.GetValue((int)i)).GetPoz().Y, 0d);

                Assert.LessOrEqual(((IKula)balls.GetValue((int)i)).GetSzybkosc().X, 6d);
                Assert.GreaterOrEqual(((IKula)balls.GetValue((int)i)).GetSzybkosc().X, 1d);
                Assert.LessOrEqual(((IKula)balls.GetValue((int)i)).GetSzybkosc().Y, 6d);
                Assert.GreaterOrEqual(((IKula)balls.GetValue((int)i)).GetSzybkosc().Y, 1d);
            }
        }

        [Test]
        public void StartSimulationTest()
        {
            Assert.IsNotNull(api);
            api.ScenaHeight = 90d;
            api.ScenaWidth = 100d;
            Assert.AreEqual(90d, api.ScenaHeight, 0.01d);
            Assert.AreEqual(100d, api.ScenaWidth, 0.01d);

            Assert.IsNotNull(api.Balls);
            Assert.AreEqual(0, api.Balls.Count());

            api.StworzKule(1, 1d, 6d);

            Assert.IsNotNull(api.Balls);
            Assert.AreEqual(1, api.Balls.Count());

            Pozycja startPos = ((IKula)api.Balls.ToArray().GetValue(0)).GetPoz();

            api.StartSimulation();

            Thread.Sleep(10);

            Pozycja pos = ((IKula)api.Balls.ToArray().GetValue(0)).GetPoz();

            Assert.Multiple(() =>
            {
                Assert.AreNotEqual(startPos.X, pos.X);
                Assert.AreNotEqual(startPos.Y, pos.Y);
            });

            api.StopSimulation();
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