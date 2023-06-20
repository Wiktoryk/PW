using Logika;
using Dane;

namespace LogikaTests
{
    public class CollisionManagerTest
    {
        [Test]
        public void TimeOfCollisionWithStaticLineTest()
        {
            Pozycja startPos = new()
            {
                X = 0d,
                Y = 0d
            };

            Pozycja vel = new()
            {
                X = 1d,
                Y = 1d
            };

            Pozycja linePoint1 = new()
            {
                X = 2.5d,
                Y = 0d
            };

            Pozycja linePoint2 = new()
            {
                X = 7.34d,
                Y = 9.68d
            };

            Assert.AreEqual(5d, CollisionManager.TimeOfCollisionWithStaticLine(startPos, vel, linePoint1, linePoint2), 0.01d);
        }

        [Test]
        public void TimeOfCollisionWithMovingLineTest()
        {
            Pozycja startPos = new()
            {
                X = 0d,
                Y = 0d
            };

            Pozycja vel = new()
            {
                X = 1d,
                Y = 1d
            };

            Pozycja linePoint1 = new()
            {
                X = 2.5d,
                Y = 0d
            };

            Pozycja linePoint2 = new()
            {
                X = 7.34d,
                Y = 9.68d
            };

            Pozycja lineVel = new()
            {
                X = 0.25d,
                Y = 0.25d
            };

            Assert.AreEqual(6.666d, CollisionManager.TimeOfCollisionWithMovingLine(startPos, vel, linePoint1, linePoint2, lineVel), 0.01d);
        }

        [Test]
        public void TimeOfCollisionWithStaticCircleTest()
        {
            Pozycja objPos = new(0, 20);
            Pozycja objVel = new(5, -2);

            Pozycja circlePos = new(15, 15);
            double radius = 5;

            (double t1, double t2) = CollisionManager.TimesOfCollisionWithStaticCircle(objPos, objVel, circlePos, radius);

            Assert.AreEqual(2.0188d, t1, 0.01d);
            Assert.AreEqual(3.8434d, t2, 0.01d);
        }

        [Test]
        public void TimeOfCollisionWithMovingCircleTest()
        {
            Pozycja objPos = new(0, 20);
            Pozycja objVel = new(5, -2);

            Pozycja circlePos = new(15, 15);
            Pozycja circleVel = new(-5, 0);
            double radius = 5;

            (double t1, double t2) = CollisionManager.TimesOfCollisionWithMovingCircle(objPos, objVel, circlePos, circleVel, radius);

            Assert.AreEqual(1.0875d, t1, 0.01d);
            Assert.AreEqual(1.9895d, t2, 0.01d);

            circleVel.X = 2;

            (t1, t2) = CollisionManager.TimesOfCollisionWithMovingCircle(objPos, objVel, circlePos, circleVel, radius);

            Assert.AreEqual(3.46d, t1, 0.01d);
            Assert.AreEqual(5.0d, t2, 0.01d);
        }

        [Test]
        public void VelocitiesAfterCollisionTest()
        {
            double m1 = 2;
            Pozycja v1 = new()
            {
                X = 2,
                Y = 2
            };

            double m2 = 1;
            Pozycja v2 = new()
            {
                X = -3,
                Y = 5
            };

            (Pozycja v3, Pozycja v4) = CollisionManager.VelocitiesAfterCollision(v1, m1, v2, m2);

            Assert.NotNull(v3);
            Assert.NotNull(v4);

            Assert.AreEqual(-1.333d, v3.X, 0.01d);
            Assert.AreEqual(4.000d, v3.Y, 0.01d);
            Assert.AreEqual(3.666d, v4.X, 0.01d);
            Assert.AreEqual(1.000d, v4.Y, 0.01d);
        }

        [Test]
        public void VelocitiesAfterBallsCollisionTest()
        {
            double m1 = 2;
            Pozycja v1 = new()
            {
                X = 2,
                Y = 2
            };

            Pozycja c1 = new()
            {
                X = 0,
                Y = 0
            };

            double m2 = 1;
            Pozycja v2 = new()
            {
                X = -3,
                Y = 5
            };

            Pozycja c2 = new()
            {
                X = 2,
                Y = 2
            };

            (Pozycja v3, Pozycja v4) = CollisionManager.VelocitiesAfterBallsCollision(v1, m1, c1, v2, m2, c2);

            Assert.NotNull(v3);
            Assert.NotNull(v4);

            Assert.AreEqual(1.333d, v3.X, 0.01d);
            Assert.AreEqual(1.333d, v3.Y, 0.01d);
            Assert.AreEqual(-1.666d, v4.X, 0.01d);
            Assert.AreEqual(6.333d, v4.Y, 0.01d);
        }
    }
}
