using Dane;
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
        public void CreateBallTest()
        {
            Assert.NotNull(api);

            IKula kula = api.StworzKule(0.01d, 100d, 0.01d, 100d, new Pozycja { X = 0d, Y = 0d }, new Pozycja { X = 10d, Y = 20d }, 0d, 100d);

            Assert.NotNull(kula);

            Assert.Multiple(() =>
            {
                Assert.LessOrEqual(kula.GetMasa(), 100d);
                Assert.GreaterOrEqual(kula.GetMasa(), 0.01d);

                Assert.LessOrEqual(kula.GetPromien(), 100d);
                Assert.GreaterOrEqual(kula.GetPromien(), 0.01d);

                Assert.LessOrEqual(kula.GetSzybkosc().X, 100d);
                Assert.GreaterOrEqual(kula.GetSzybkosc().X, 0d);
                Assert.LessOrEqual(kula.GetSzybkosc().Y, 100d);
                Assert.GreaterOrEqual(kula.GetSzybkosc().Y, 0d);

                Assert.LessOrEqual(kula.GetPoz().X, 100d);
                Assert.GreaterOrEqual(kula.GetPoz().X, -90d);
                Assert.LessOrEqual(kula.GetPoz().Y, 100d);
                Assert.GreaterOrEqual(kula.GetPoz().Y, -80d);
            });
        }

        [Test]
        public void StworzSceneTest()
        {
            Assert.NotNull(api);

            Scena scena= api.StworzScene(100d, 0.01d);

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