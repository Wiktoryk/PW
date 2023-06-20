using Dane;
namespace DaneTests
{
    public class KulaTest
    {
        [Test]
        public void ConstructorAndGettersTest()
        {
            IKula kula = new Kula(100L, 0.1d, 0.1d, new Pozycja { X = 10.0d, Y = 11.34d }, new Pozycja { X = 9d, Y = 2.2222d });

            Assert.NotNull(kula);

            Assert.Multiple(() =>
            {
                Assert.AreEqual(100L, kula.GetId());
                Assert.AreEqual(0.1d, kula.GetMasa(), 0.01d);
                Assert.AreEqual(0.1d, kula.GetPromien(), 0.01d);
                Assert.AreEqual(10.0d, kula.GetPoz().X, 0.01d);
                Assert.AreEqual(11.34d, kula.GetPoz().Y, 0.01d);
                Assert.AreEqual(9d, kula.GetSzybkosc().X, 0.01d);
                Assert.AreEqual(2.2222d, kula.GetSzybkosc().Y, 0.01d);
            });
        }

        [Test]
        public void SettersTest()
        {
            IKula kula = new Kula(100L, 0d, 0d, new Pozycja { X = 0d, Y = 0d }, new Pozycja { X = 0d, Y = 0d });

            Assert.NotNull(kula);

            Assert.Multiple(() =>
            {
                Assert.AreEqual(0d, kula.GetMasa(), 0.01d);
                Assert.AreEqual(0d, kula.GetPromien(), 0.01d);
                Assert.AreEqual(0d, kula.GetPoz().X, 0.01d);
                Assert.AreEqual(0d, kula.GetPoz().Y, 0.01d);
                Assert.AreEqual(0d, kula.GetSzybkosc().X, 0.01d);
                Assert.AreEqual(0d, kula.GetSzybkosc().Y, 0.01d);
            });

            kula.SetMasa(10.222d);

            Assert.Multiple(() =>
            {
                Assert.AreEqual(10.222d, kula.GetMasa(), 0.01d);
                Assert.AreEqual(0d, kula.GetPromien(), 0.01d);
                Assert.AreEqual(0d, kula.GetPoz().X, 0.01d);
                Assert.AreEqual(0d, kula.GetPoz().Y, 0.01d);
                Assert.AreEqual(0d, kula.GetSzybkosc().X, 0.01d);
                Assert.AreEqual(0d, kula.GetSzybkosc().Y, 0.01d);
            });

            kula.SetPromien(10.222d);

            Assert.Multiple(() =>
            {
                Assert.AreEqual(10.222d, kula.GetMasa(), 0.01d);
                Assert.AreEqual(10.222d, kula.GetPromien(), 0.01d);
                Assert.AreEqual(0d, kula.GetPoz().X, 0.01d);
                Assert.AreEqual(0d, kula.GetPoz().Y, 0.01d);
                Assert.AreEqual(0d, kula.GetSzybkosc().X, 0.01d);
                Assert.AreEqual(0d, kula.GetSzybkosc().Y, 0.01d);
            });

            kula.SetPoz(new Pozycja { X = 34533.33d, Y = 12.333d });

            Assert.Multiple(() =>
            {
                Assert.AreEqual(10.222d, kula.GetMasa(), 0.01d);
                Assert.AreEqual(10.222d, kula.GetPromien(), 0.01d);
                Assert.AreEqual(34533.33d, kula.GetPoz().X, 0.01d);
                Assert.AreEqual(12.333d, kula.GetPoz().Y, 0.01d);
                Assert.AreEqual(0d, kula.GetSzybkosc().X, 0.01d);
                Assert.AreEqual(0d, kula.GetSzybkosc().Y, 0.01d);
            });

            kula.SetSzybkosc(new Pozycja { X = 0.005d, Y = 1.12d });

            Assert.Multiple(() =>
            {
                Assert.AreEqual(10.222d, kula.GetMasa(), 0.01d);
                Assert.AreEqual(10.222d, kula.GetPromien(), 0.01d);
                Assert.AreEqual(34533.33d, kula.GetPoz().X, 0.01d);
                Assert.AreEqual(12.333d, kula.GetPoz().Y, 0.01d);
                Assert.AreEqual(0.005d, kula.GetSzybkosc().X, 0.001d);
                Assert.AreEqual(1.12d, kula.GetSzybkosc().Y, 0.01d);
            });

            kula.SetSrednica(kula.GetPromien() * 2 + 2);

            Assert.Multiple(() =>
            {
                Assert.AreEqual(10.222d, kula.GetMasa(), 0.01d);
                Assert.AreEqual(11.222d, kula.GetPromien(), 0.01d);
                Assert.AreEqual(34533.33d, kula.GetPoz().X, 0.01d);
                Assert.AreEqual(12.333d, kula.GetPoz().Y, 0.01d);
                Assert.AreEqual(0.005d, kula.GetSzybkosc().X, 0.001d);
                Assert.AreEqual(1.12d, kula.GetSzybkosc().Y, 0.01d);
            });
        }
    }
}
