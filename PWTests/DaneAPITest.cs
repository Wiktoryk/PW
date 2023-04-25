using Dane;
using NUnit.Framework;
using System.Numerics;

namespace DaneTests
{
    public class DaneApiTest
    {
        DaneApiBase? api;

        [SetUp]
        public void Setup()
        {
            api = new DaneApi();
        }

        [Test]
        public void StworzKuleTest()
        {
            Assert.NotNull(api);

            IKula ball = api.StworzKule(new Pozycja { X = 0d, Y = 0d }, new Pozycja { X = 10d, Y = 20d }, 0d, 100d);

            Assert.NotNull(ball);

            Assert.Multiple(() =>
            {
                Assert.LessOrEqual(ball.GetPromien(), 100d);
                Assert.GreaterOrEqual(ball.GetPromien(), 0.01d);

                Assert.LessOrEqual(ball.GetSzybkosc().X, 100d);
                Assert.GreaterOrEqual(ball.GetSzybkosc().X, 0d);
                Assert.LessOrEqual(ball.GetSzybkosc().Y, 100d);
                Assert.GreaterOrEqual(ball.GetSzybkosc().Y, 0d);

                Assert.LessOrEqual(ball.GetPoz().X, 100d);
                Assert.GreaterOrEqual(ball.GetPoz().X, -90d);
                Assert.LessOrEqual(ball.GetPoz().Y, 100d);
                Assert.GreaterOrEqual(ball.GetPoz().Y, -80d);
            });
        }

        [Test]
        public void StworzSceneTest()
        {
            Assert.NotNull(api);

            Scena scena = api.StworzScene(100d, 0.01d);

            Assert.NotNull(scena);

            Assert.Multiple(() =>
            {
                Assert.AreEqual(100d, scena.GetWidth(), 0.01d);
                Assert.AreEqual(0.01d, scena.GetHeight(), 0.01d);
            });
        }

        [Test]
        public void GetApiTest()
        {
            Assert.NotNull(api);

            DaneApiBase api2 = api ?? DaneApiBase.GetApi();

            Assert.NotNull(api2);
            Assert.AreSame(api, api2);

            api2 = DaneApiBase.GetApi();

            Assert.NotNull(api2);
            Assert.AreNotSame(api, api2);
        }
    }
}