using Dane;
using System.Reflection;
using Logika;

namespace LogikaTests
{
    public class SimulationManagerTest
    {
        [Test]
        public void ConstructorAndGettersTest()
        {
            SimulationManager sim = new(new Scena(100d, 90d));

            Assert.IsNotNull(sim);

            Assert.Multiple(() =>
            {
                Assert.IsNotNull(sim.Balls);
                Assert.AreEqual(0, sim.Balls.Count);
                Assert.AreEqual(90d, sim.ScenaHeight, 0.01d);
                Assert.AreEqual(100d, sim.ScenaWidth, 0.01d);
            });
        }

        [Test]
        public void SettersTest()
        {
            SimulationManager sim = new(new Scena(100d, 90d));

            Assert.IsNotNull(sim);

            // Height
            sim.ScenaHeight = 120.33d;

            Assert.AreEqual(120.33d, sim.ScenaHeight, 0.01d);

            sim.ScenaHeight = 120.33d;

            Assert.AreEqual(120.33d, sim.ScenaHeight, 0.01d);

            // Width
            sim.ScenaWidth = 33.32d;

            Assert.AreEqual(33.32d, sim.ScenaWidth, 0.01d);

            sim.ScenaHeight = 33.32d;

            Assert.AreEqual(33.32d, sim.ScenaWidth, 0.01d);
        }

        [Test]
        public void CreateRandomBallsTest()
        {
            SimulationManager sim = new(new Scena(100d, 90d));

            Assert.IsNotNull(sim);
            Assert.IsNotNull(sim.Balls);

            uint count = 10;

            sim.StworzKule(count, 0.1d, 10d, 0.1d, 10d, 1d, 6d);

            Assert.IsNotNull(sim.Balls);
            Assert.AreEqual(count, sim.Balls.Count);

            bool IsInBall(IKula ball)
            {
                foreach (IKula b in sim.Balls)
                {
                    if (b == ball) continue;

                    double x = b.GetPoz().X - ball.GetPoz().X;
                    double y = b.GetPoz().Y - ball.GetPoz().Y;
                    double r = b.GetPromien() + ball.GetPromien();

                    if (x * x + y * y < r * r)
                    {
                        return true;
                    }
                }

                return false;
            }

            for (uint i = 0; i < count; ++i)
            {
                // Mass
                Assert.LessOrEqual(sim.Balls[(int)i].GetMasa(), 10d);
                Assert.GreaterOrEqual(sim.Balls[(int)i].GetMasa(), 0.1d);

                // Radius
                Assert.LessOrEqual(sim.Balls[(int)i].GetPromien(), 10d);
                Assert.GreaterOrEqual(sim.Balls[(int)i].GetPromien(), 0.1d);

                // Pos
                Assert.LessOrEqual(sim.Balls[(int)i].GetPoz().X, 100d);
                Assert.GreaterOrEqual(sim.Balls[(int)i].GetPoz().X, 0d);
                Assert.LessOrEqual(sim.Balls[(int)i].GetPoz().Y, 90d);
                Assert.GreaterOrEqual(sim.Balls[(int)i].GetPoz().Y, 0d);

                // Vel
                Assert.LessOrEqual(sim.Balls[(int)i].GetSzybkosc().X, 6d);
                Assert.GreaterOrEqual(sim.Balls[(int)i].GetSzybkosc().X, 1d);
                Assert.LessOrEqual(sim.Balls[(int)i].GetSzybkosc().Y, 6d);
                Assert.GreaterOrEqual(sim.Balls[(int)i].GetSzybkosc().Y, 1d);

                // Is In Ball
                Assert.That(IsInBall(sim.Balls[(int)i]), Is.False);
            }
        }

        [Test, RequiresThread]
        public void StartSimulationTest()
        {
            SimulationManager sim = new(new Scena(100d, 90d));

            Assert.IsNotNull(sim);
            Assert.IsNotNull(sim.Balls);

            sim.StworzKule(1, 0.1d, 10d, 0.1d, 10d, 1d, 6d);

            Assert.IsNotNull(sim.Balls);
            Assert.AreEqual(1, sim.Balls.Count);

            Pozycja startPos = sim.Balls[0].GetPoz();

            sim.StartSimulation();

            Thread.Sleep(10);

            Pozycja pos = sim.Balls[0].GetPoz();

            Assert.Multiple(() =>
            {
                Assert.AreNotEqual(startPos.X, pos.X);
                Assert.AreNotEqual(startPos.Y, pos.Y);
            });

            sim.ClearBalls();

            Assert.AreEqual(0, sim.Balls.Count);
        }

        [Test]
        public void IsInBallTest()
        {
            SimulationManager sim = new(new Scena(100d, 90d));

            Assert.IsNotNull(sim);
            Assert.IsNotNull(sim.Balls);

            sim.StworzKule(1, 1d, 20d, 0.1d, 10d, 1d, 6d);

            Assert.IsNotNull(sim.Balls);
            Assert.AreEqual(1, sim.Balls.Count);

            Pozycja pos = sim.Balls[0].GetPoz();
            double radius = sim.Balls[0].GetPromien();

            Kula centerBall = new(111, 10d, 10d, pos, new Pozycja(10, 10));
            Kula insideBall = new(666, 10d, 10d, new Pozycja(pos.X - radius, pos.Y + radius), new Pozycja(10, 10));
            Kula rightBall = new(222, 10d, 10d, new Pozycja(pos.X - radius - 10.05, pos.Y), new Pozycja(10, 10));
            Kula leftBall = new(333, 10d, 10d, new Pozycja(pos.X + radius + 10.05, pos.Y), new Pozycja(10, 10));
            Kula topBall = new(444, 10d, 10d, new Pozycja(pos.X, pos.Y - radius - 10.05), new Pozycja(10, 10));
            Kula bottomBall = new(555, 10d, 10d, new Pozycja(pos.X, pos.Y + radius + 10.05), new Pozycja(10, 10));
            Kula someBall = new(666, 10d, 10d, new Pozycja(pos.X - radius - 20, pos.Y - radius - 20), new Pozycja(10, 10));

            var method = sim.GetType().GetMethod("IsInBall", BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.That(method, Is.Not.Null);
            Assert.That(method.IsConstructor, Is.False);

            Assert.Multiple(() =>
            {
                Assert.That((bool)method.Invoke(sim, new object[] { centerBall }), Is.True);
                Assert.That((bool)method.Invoke(sim, new object[] { insideBall }), Is.True);
                Assert.That((bool)method.Invoke(sim, new object[] { rightBall }), Is.False);
                Assert.That((bool)method.Invoke(sim, new object[] { leftBall }), Is.False);
                Assert.That((bool)method.Invoke(sim, new object[] { topBall }), Is.False);
                Assert.That((bool)method.Invoke(sim, new object[] { bottomBall }), Is.False);
                Assert.That((bool)method.Invoke(sim, new object[] { someBall }), Is.False);
            });
        }

        [Test]
        public void CheckPlaneBordersCollisionsTest()
        {
            SimulationManager sim = new SimulationManager(new Scena(10, 10));

            Pozycja lastPos = new Pozycja { X = 4, Y = 2 };
            Pozycja lastVel = new Pozycja { X = 2, Y = -2 };
            double totalTime = 2d;
            double currTime = totalTime;
            IKula kula = new Kula(1L, 1d, 1d, new Pozycja { X = 8, Y = -2 }, new Pozycja { X = 2, Y = 2 });

            var method = sim.GetType().GetMethod("CheckPlaneBordersCollisions", BindingFlags.NonPublic | BindingFlags.Instance);

            Assert.IsNotNull(method);

            var res = (ValueTuple<Pozycja, Pozycja, double>)method.Invoke(sim, new object?[] { kula, lastPos, lastVel, currTime, totalTime });

            Assert.NotNull(res);

            (Pozycja newLast, Pozycja newVel, double newTime) = res;

            Assert.NotNull(newLast);
            Assert.NotNull(newVel);

            Assert.AreEqual(totalTime - 0.5, newTime, 0.01d);
            Assert.AreEqual(5.0d, newLast.X, 0.01d);
            Assert.AreEqual(1.0d, newLast.Y, 0.01d);
            Assert.AreEqual(2.0d, newVel.X, 0.01d);
            Assert.AreEqual(2.0d, newVel.Y, 0.01d);
        }

        [Test]
        public void CheckBallsCollisionsTest()
        {
            SimulationManager sim = new(new Scena(10, 10));

            sim.StworzKule(1, 1, 1, 1, 1, 0, 0);

            Assert.That(sim.Balls.Count, Is.EqualTo(1));

            Pozycja lastPos = sim.Balls[0].GetPoz() + new Pozycja { X = 3, Y = 3 };
            Pozycja lastVel = new Pozycja { X = -3, Y = -3 };
            double totalTime = 2d;
            double currTime = totalTime;
            IKula kula = new Kula(1L, 1d, 1d, sim.Balls[0].GetPoz() - new Pozycja { X = 3, Y = 3 }, new Pozycja { X = -3, Y = -3 });

            var method = sim.GetType().GetMethod("CheckBallsCollisions", BindingFlags.NonPublic | BindingFlags.Instance);

            Assert.IsNotNull(method);

            var res = (ValueTuple<Pozycja, Pozycja, double>)method.Invoke(sim, new object?[] { kula, lastPos, lastVel, currTime, totalTime });

            Assert.NotNull(res);

            (Pozycja newLast, Pozycja newVel, double newTime) = res;

            Assert.NotNull(newLast);
            Assert.NotNull(newVel);

            Assert.AreEqual(totalTime - 0.528888d, newTime, 0.01d);
        }

        [Test]
        public void CheckCollisionsTest()
        {
            SimulationManager sim = new(new Scena(10, 10));

            sim.StworzKule(1, 1, 1, 1, 1, 0, 0);

            Assert.That(sim.Balls.Count, Is.EqualTo(1));

            Pozycja lastPos = sim.Balls[0].GetPoz() + new Pozycja { X = 3, Y = 3 };
            Pozycja lastVel = new Pozycja { X = -3, Y = -3 };
            double totalTime = 2d;
            IKula kula = new Kula(1L, 1d, 1d, sim.Balls[0].GetPoz() - new Pozycja { X = 3, Y = 3 }, new Pozycja { X = -3, Y = -3 });

            var method = sim.GetType().GetMethod("CheckCollisions", BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.IsNotNull(method);

            method.Invoke(sim, new object?[] { kula, new PositionChangedEventArgs(lastPos, lastVel, totalTime) });

            Assert.AreNotEqual(lastPos, kula.GetPoz());
        }
    }
}
