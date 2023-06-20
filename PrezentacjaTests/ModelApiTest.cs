using Dane;
using Model;

namespace PrezentacjaTests
{
    public class ModelApiTest
    {
        ModelApiBase? api;

        [SetUp]
        public void Setup()
        {
            api = new ModelApi(null);
        }

        [Test]
        public void ConstructorAndGettersTest()
        {
            ModelApiBase testApi = new ModelApi(null);
            Assert.IsNotNull(testApi);
            Assert.IsNotNull(testApi.Balls);
            Assert.AreEqual(0, testApi.Balls.Count);
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
        public void GenerateBallsTest()
        {
            Assert.IsNotNull(api);
            api.ScenaHeight = 90d;
            api.ScenaWidth = 100d;
            Assert.AreEqual(90d, api.ScenaHeight, 0.01d);
            Assert.AreEqual(100d, api.ScenaWidth, 0.01d);

            Assert.IsNotNull(api.Balls);
            Assert.AreEqual(0, api.Balls.Count);

            uint count = 10;

            api.StworzKule(count, 0.1d, 10d, 0.1d, 10d, 1d, 6d);

            Assert.AreEqual(count, api.Balls.Count);

            Array balls = api.Balls.ToArray();

            for (uint i = 0; i < count; ++i)
            {
                Assert.LessOrEqual(((IKulaModel)balls.GetValue((int)i)).ScenaPoz.X, 100d);
                Assert.GreaterOrEqual(((IKulaModel)balls.GetValue((int)i)).ScenaPoz.X, 0d);
                Assert.LessOrEqual(((IKulaModel)balls.GetValue((int)i)).ScenaPoz.Y, 90d);
                Assert.GreaterOrEqual(((IKulaModel)balls.GetValue((int)i)).ScenaPoz.Y, 0d);
            }
        }

        [Test]
        public void GetApiTest()
        {
            Assert.NotNull(api);

            ModelApiBase api2 = api ?? ModelApiBase.GetApi();

            Assert.NotNull(api2);
            Assert.AreSame(api, api2);

            api2 = ModelApiBase.GetApi();

            Assert.NotNull(api2);
            Assert.AreNotSame(api, api2);
        }
    }
}