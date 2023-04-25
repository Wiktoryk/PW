using Dane;
using NUnit.Framework;

namespace DaneTests
{
    public class BallTest
    {
        [Test]
        public void ConstructorAndGettersTest()
        {
            IKula ball = new Kula(100L, 0.1d, new Pozycja { X = 10.0d, Y = 11.34d }, new Pozycja { X = 9d, Y = 2.2222d });

            Assert.NotNull(ball);

            Assert.Multiple(() =>
            {
                Assert.AreEqual(100L, ball.GetId());
                Assert.AreEqual(0.1d, ball.GetPromien(), 0.01d);
                Assert.AreEqual(10.0d, ball.GetPoz().X, 0.01d);
                Assert.AreEqual(11.34d, ball.GetPoz().Y, 0.01d);
                Assert.AreEqual(9d, ball.GetSzybkosc().X, 0.01d);
                Assert.AreEqual(2.2222d, ball.GetSzybkosc().Y, 0.01d);
            });
        }

        [Test]
        public void SettersTest()
        {
            IKula ball = new Kula(100L, 0d, new Pozycja { X = 0d, Y = 0d }, new Pozycja { X = 0d, Y = 0d });

            Assert.NotNull(ball);

            Assert.Multiple(() =>
            {
                Assert.AreEqual(0d, ball.GetPromien(), 0.01d);
                Assert.AreEqual(0d, ball.GetPoz().X, 0.01d);
                Assert.AreEqual(0d, ball.GetPoz().Y, 0.01d);
                Assert.AreEqual(0d, ball.GetSzybkosc().X, 0.01d);
                Assert.AreEqual(0d, ball.GetSzybkosc().Y, 0.01d);
            });

            ball.SetPromien(10.222d);

            Assert.Multiple(() =>
            {
                Assert.AreEqual(10.222d, ball.GetPromien(), 0.01d);
                Assert.AreEqual(0d, ball.GetPoz().X, 0.01d);
                Assert.AreEqual(0d, ball.GetPoz().Y, 0.01d);
                Assert.AreEqual(0d, ball.GetSzybkosc().X, 0.01d);
                Assert.AreEqual(0d, ball.GetSzybkosc().Y, 0.01d);
            });

            ball.SetPoz(new Pozycja { X = 34533.33d, Y = 12.333d });

            Assert.Multiple(() =>
            {
                Assert.AreEqual(10.222d, ball.GetPromien(), 0.01d);
                Assert.AreEqual(34533.33d, ball.GetPoz().X, 0.01d);
                Assert.AreEqual(12.333d, ball.GetPoz().Y, 0.01d);
                Assert.AreEqual(0d, ball.GetSzybkosc().X, 0.01d);
                Assert.AreEqual(0d, ball.GetSzybkosc().Y, 0.01d);
            });

            ball.SetSzybkosc(new Pozycja { X = 0.005d, Y = 1.12d });

            Assert.Multiple(() =>
            {
                Assert.AreEqual(10.222d, ball.GetPromien(), 0.01d);
                Assert.AreEqual(34533.33d, ball.GetPoz().X, 0.01d);
                Assert.AreEqual(12.333d, ball.GetPoz().Y, 0.01d);
                Assert.AreEqual(0.005d, ball.GetSzybkosc().X, 0.001d);
                Assert.AreEqual(1.12d, ball.GetSzybkosc().Y, 0.01d);
            });
        }

        [Test, RequiresThread]
        public void PositionChangeTest()
        {
            IKula ball = new Kula(100L, 0d, new Pozycja { X = 0d, Y = 0d }, new Pozycja { X = 1d, Y = 1d });

            Assert.NotNull(ball);

            ball.StartThread();

            Thread.Sleep(10);

            Pozycja pos = ball.GetPoz();

            Assert.Multiple(() =>
            {
                Assert.AreNotEqual(0d, pos.X);
                Assert.AreNotEqual(0d, pos.Y);
            });

            ball.Dispose();
        }

        [Test, RequiresThread]
        public void OnPositionChangeEventTest()
        {
            bool eventRaised = false;

            IKula ball = new Kula(100L, 0d, new Pozycja { X = 0d, Y = 0d }, new Pozycja { X = 0d, Y = 0d });
            Assert.NotNull(ball);

            ball.OnPositionChanged += (object source, PositionChangedEventArgs e) => { eventRaised = true; };

            ball.StartThread();

            Thread.Sleep(10);

            Assert.That(eventRaised, Is.True.After(10));

            ball.Dispose();
        }
    }
}
