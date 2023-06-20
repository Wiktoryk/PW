using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dane;

namespace Logika
{
    internal class SimulationManager : IDisposable
    {
        private readonly Scena m_scena;
        private readonly DaneApiBase dane;

        public List<IKula> Balls { get; private set; }
        public double ScenaWidth
        {
            get
            {
                return m_scena.GetWidth();
            }

            set
            {
                if (m_scena != null && m_scena.GetWidth() != value)
                {
                    m_scena.SetWidth(value);
                }
            }
        }
        public double ScenaHeight
        {
            get
            {
                return m_scena.GetHeight();
            }

            set
            {
                if (m_scena != null && m_scena.GetHeight() != value)
                {
                    m_scena.SetHeight(value);
                }
            }
        }

        public SimulationManager(Scena scena, DaneApiBase? dane = default)
        {
            this.m_scena = scena;
            this.dane = dane ?? DaneApiBase.GetApi();
            this.Balls = new List<IKula>();

            BallLogger.Log("Created Simulation Manager", LogType.DEBUG);
        }

        public void StworzKule(uint ballsNum, double minMass, double maxMass, double minRadius, double maxRadius, double minVel, double maxVel)
        {
            Pozycja planePos = new(ScenaWidth, ScenaHeight);
            for (int i = 0; i < ballsNum; i++)
            {
                Kula newBall = this.dane.StworzKule(minMass, maxMass, minRadius, maxRadius, Pozycja.Zero, new Pozycja(ScenaWidth, ScenaHeight), minVel, maxVel);

                while (IsInBall(newBall))
                {
                    double minX = Pozycja.Zero.X + newBall.GetPromien();
                    double maxX = planePos.X - newBall.GetPromien();

                    double minY = Pozycja.Zero.Y + newBall.GetPromien();
                    double maxY = planePos.Y - newBall.GetPromien();

                    Random rnd = new();
                    newBall.SetPoz(new(rnd.NextDouble() * (maxX - minX) + minX, rnd.NextDouble() * (maxY - minY) + minY));
                }

                newBall.OnPositionChanged += CheckCollisions;

                Balls.Add(newBall);
            }
        }

        public void StartSimulation()
        {
            foreach (IKula ball in Balls)
            {
                ball.StartThread();
            }
            BallLogger.Log("Started Simulation", LogType.INFO);
        }

        public void StopSimulation()
        {
            foreach (IKula ball in Balls)
            {
                ball.EndThread();
            }
            BallLogger.Log("Simulation Stopped", LogType.INFO);
        }

        public void ClearBalls()
        {
            int counter = 0;
            BallLogger.Log(new StringBuilder("Balls Num: ").Append(Balls.Count).ToString(), LogType.DEBUG);
            foreach (IKula ball in Balls)
            {
                ball?.Dispose();
                counter++;
            }
            BallLogger.Log(new StringBuilder("Disposed: ").Append(counter).ToString(), LogType.DEBUG);
            Balls.Clear();
            BallLogger.Log(new StringBuilder("Cleared Balls: ").Append(Balls.Count).ToString(), LogType.DEBUG);
            BallLogger.Log("Balls Cleared", LogType.DEBUG);
        }

        private bool IsInBall(IKula ball)
        {
            lock (Balls)
            {
                foreach (IKula b in Balls)
                {
                    double x = b.GetPoz().X - ball.GetPoz().X;
                    double y = b.GetPoz().Y - ball.GetPoz().Y;
                    double r = b.GetPromien() + ball.GetPromien();

                    if (x * x + y * y < r * r)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private void CheckCollisions(object source, PositionChangedEventArgs e)
        {
            IKula ball = (IKula)source;
            Pozycja lastPos = e.LastPos;
            Pozycja currVel = e.Vel;
            double totalTime = e.ElapsedSeconds;
            double currentTime = totalTime;
            (lastPos, currVel, currentTime) = CheckBallsCollisions(ball, lastPos, currVel, currentTime, totalTime);
            (lastPos, currVel, currentTime) = CheckPlaneBordersCollisions(ball, lastPos, currVel, currentTime, totalTime);
        }

        private (Pozycja, Pozycja, double) CheckBallsCollisions(IKula ball, Pozycja lastPos, Pozycja lastVel, double currentTime, double totalTime)
        {
            // Other Balls List (In Move Dist to current Ball)
            double moveDist = lastVel.Length + ball.GetPromien();
            List<(double, IKula)> nearBalls = new();
            for (int j = 0; j < Balls.Count; j++)
            {
                if (Balls[j] != ball)
                {
                    double dist = Math.Clamp((Balls[j].GetPoz() - lastPos).Length - Balls[j].GetPromien(), 0, double.MaxValue);
                    nearBalls.Add((dist, Balls[j]));
                }
            }
            // Sort By Dist
            nearBalls = nearBalls.OrderBy((e1) => e1.Item1).ToList();
            double tcmin, tcmax;
            for (int j = 0; j < nearBalls.Count; j++)
            {
                (double dist, IKula b) = nearBalls[j];
                Pozycja lastPos2 = b.GetPoz();
                Pozycja vel2 = b.GetSzybkosc();
                (tcmin, tcmax) = CollisionManager.TimesOfCollisionWithMovingCircle(lastPos, lastVel, lastPos2, vel2, ball.GetPromien() + b.GetPromien());
                if (tcmin != double.NegativeInfinity && tcmax != double.PositiveInfinity)
                {
                    if (tcmin >= 0 && tcmax >= 0)
                    {
                        if (tcmin <= currentTime)
                        {
                            lastPos += lastVel * tcmin;
                            ball.SetPoz(lastPos);
                            Pozycja newPos2 = lastPos2 + vel2 * tcmin;
                            b.SetPoz(newPos2);
                            //(lastVel, vel2) = CollisionManager.VelocitiesAfterBallsCollision(lastVel, ball.GetMass(), lastPos, vel2, b.GetMass(), newPos2);
                            (lastVel, vel2) = CollisionManager.VelocitiesAfterCollision(lastVel, ball.GetMasa(), vel2, b.GetMasa());
                            ball.SetSzybkosc(lastVel);
                            b.SetSzybkosc(vel2);
                            currentTime -= tcmin;
                        }
                    }
                    else if (tcmin < 0 && tcmax >= 0)
                    {
                        if (tcmin >= currentTime - totalTime)
                        {
                            lastPos += lastVel * tcmin;
                            ball.SetPoz(lastPos);
                            Pozycja newPos2 = lastPos2 + vel2 * tcmin;
                            b.SetPoz(newPos2);
                            //(lastVel, vel2) = CollisionManager.VelocitiesAfterBallsCollision(lastVel, ball.GetMass(), lastPos, vel2, b.GetMass(), newPos2);
                            (lastVel, vel2) = CollisionManager.VelocitiesAfterCollision(lastVel, ball.GetMasa(), vel2, b.GetMasa());
                            ball.SetSzybkosc(lastVel);
                            b.SetSzybkosc(vel2);
                            currentTime -= tcmin;
                        }
                    }
                }
                else if (tcmin != double.NegativeInfinity)
                {
                    if (tcmin >= 0 && tcmin <= currentTime)
                    {
                        lastPos += lastVel * tcmin;
                        ball.SetPoz(lastPos);
                        Pozycja newPos2 = lastPos2 + vel2 * tcmin;
                        b.SetPoz(newPos2);
                        //(lastVel, vel2) = CollisionManager.VelocitiesAfterBallsCollision(lastVel, ball.GetMass(), lastPos, vel2, b.GetMass(), newPos2);
                        (lastVel, vel2) = CollisionManager.VelocitiesAfterCollision(lastVel, ball.GetMasa(), vel2, b.GetMasa());
                        ball.SetSzybkosc(lastVel);
                        b.SetSzybkosc(vel2);
                        currentTime -= tcmin;
                    }
                }
                else if (tcmax != double.PositiveInfinity)
                {
                    if (tcmax >= 0 && tcmax <= currentTime)
                    {
                        lastPos += lastVel * tcmax;
                        ball.SetPoz(lastPos);
                        Pozycja newPos2 = lastPos2 + vel2 * tcmax;
                        b.SetPoz(newPos2);
                        //(lastVel, vel2) = CollisionManager.VelocitiesAfterBallsCollision(lastVel, ball.GetMass(), lastPos, vel2, b.GetMass(), newPos2);
                        (lastVel, vel2) = CollisionManager.VelocitiesAfterCollision(lastVel, ball.GetMasa(), vel2, b.GetMasa());
                        ball.SetSzybkosc(lastVel);
                        b.SetSzybkosc(vel2);
                        currentTime -= tcmax;
                    }
                }
            }

            return (lastPos, lastVel, currentTime);
        }

        private (Pozycja, Pozycja, double) CheckPlaneBordersCollisions(IKula ball, Pozycja lastPos, Pozycja lastVel, double currentTime, double totalTime)
        {
            // Plane Points:
            Pozycja topLeftPlanePoint = new() { X = 0 + ball.GetPromien(), Y = 0 + ball.GetPromien() };
            Pozycja topRightPlanePoint = new() { X = ScenaWidth - ball.GetPromien(), Y = 0 + ball.GetPromien() };
            Pozycja bottomRightPlanePoint = new() { X = ScenaWidth - ball.GetPromien(), Y = ScenaHeight - ball.GetPromien() };
            Pozycja bottomLeftPlanePoint = new() { X = 0 + ball.GetPromien(), Y = ScenaHeight - ball.GetPromien() };

            if (currentTime >= 0)
            {
                double tc;
                // Plane Top Line:
                if (lastVel.Y < 0 && lastPos.Y > topLeftPlanePoint.Y)
                {
                    tc = CollisionManager.TimeOfCollisionWithStaticLine(lastPos, lastVel, topLeftPlanePoint, topRightPlanePoint);
                    if (tc != double.PositiveInfinity && tc >= 0 && tc <= currentTime)
                    {
                        lastPos += lastVel * tc;
                        ball.SetPoz(lastPos);
                        lastVel.Y *= -1;
                        ball.SetSzybkosc(lastVel);
                        currentTime -= tc;
                    }
                }

                // Plane Right Line
                if (lastVel.X > 0 && lastPos.X < topRightPlanePoint.X)
                {
                    tc = CollisionManager.TimeOfCollisionWithStaticLine(lastPos, lastVel, topRightPlanePoint, bottomRightPlanePoint);
                    if (tc != double.PositiveInfinity && tc >= 0 && tc <= currentTime)
                    {
                        lastPos += lastVel * tc;
                        ball.SetPoz(lastPos);
                        lastVel.X *= -1;
                        ball.SetSzybkosc(lastVel);
                        currentTime -= tc;
                    }
                }

                // Plane Bottom Line:
                if (lastVel.Y > 0 && lastPos.Y < bottomRightPlanePoint.Y)
                {
                    tc = CollisionManager.TimeOfCollisionWithStaticLine(lastPos, lastVel, bottomLeftPlanePoint, bottomRightPlanePoint);
                    if (tc != double.PositiveInfinity && tc >= 0 && tc <= currentTime)
                    {
                        lastPos += lastVel * tc;
                        ball.SetPoz(lastPos);
                        lastVel.Y *= -1;
                        ball.SetSzybkosc(lastVel);
                        currentTime -= tc;
                    }
                }

                // Plane Left Line
                if (lastVel.X < 0 && lastPos.X > bottomLeftPlanePoint.X)
                {
                    tc = CollisionManager.TimeOfCollisionWithStaticLine(lastPos, lastVel, bottomLeftPlanePoint, topLeftPlanePoint);
                    if (tc != double.PositiveInfinity && tc >= 0 && tc <= currentTime)
                    {
                        lastPos += lastVel * tc;
                        ball.SetPoz(lastPos);
                        lastVel.X *= -1;
                        ball.SetSzybkosc(lastVel);
                        currentTime -= tc;
                    }
                }

                ball.SetPoz(lastPos + lastVel * currentTime);
            }
            return (lastPos, lastVel, currentTime);
        }

        public void Dispose()
        {
            ClearBalls();
            GC.SuppressFinalize(this);
        }
    }
    internal class CollisionManager
    {
        // Calculating Times:

        public static double TimeOfCollisionWithStaticLine(Pozycja objPos, Pozycja objVel, Pozycja linePoint1, Pozycja linePoint2)
        {
            return TimeOfCollisionWithMovingLine(objPos, objVel, linePoint1, linePoint2, Pozycja.Zero);
        }

        public static double TimeOfCollisionWithMovingLine(Pozycja objPos, Pozycja objVel, Pozycja linePoint1, Pozycja linePoint2, Pozycja lineVel)
        {
            if (objVel == Pozycja.Zero && lineVel == Pozycja.Zero) return double.PositiveInfinity;
            if (objVel - lineVel == Pozycja.Zero) return double.PositiveInfinity;

            // Ax + By + C = 0
            double A = linePoint1.Y - linePoint2.Y;
            double B = linePoint2.X - linePoint1.X;
            double C = (linePoint1.X * linePoint2.Y) - (linePoint2.X * linePoint1.Y);

            double tc = (-A * objPos.X - B * objPos.Y - C) / (A * (objVel.X - lineVel.X) + B * (objVel.Y - lineVel.Y));
            if (tc < 0) return double.PositiveInfinity;

            // Sprawdzanie czy punkt leży na lini ograniczonej punktami
            Pozycja cPos = objPos + objVel * tc;
            Pozycja lPos1 = linePoint1 + lineVel * tc;
            //Pos2D lPos2 = linePoint2 + lineVel * tc;

            // dx = B
            // dy = -A
            Pozycja d = new(B, -A);
            // rx = cp.x - lp1.x
            // ry = cp.y - lp1.y
            Pozycja r = cPos - lPos1;
            // d_p = dx * rx + dy * ry
            double dot_product = d.DotProduct(r);
            // l_s = dx * dx + dy * dy
            double length_squared = d.DotProduct(d);

            if (dot_product < 0 || dot_product > length_squared)
                return double.PositiveInfinity;

            return tc;
        }

        public static (double, double) TimesOfCollisionWithStaticCircle(Pozycja objPos, Pozycja objVel, Pozycja ballPos, double radius, double startAngle = 0, double endAngle = 0)
        {
            return TimesOfCollisionWithMovingCircle(objPos, objVel, ballPos, Pozycja.Zero, radius, startAngle, endAngle);
        }

        public static (double, double) TimesOfCollisionWithMovingCircle(Pozycja objPos, Pozycja objVel, Pozycja ballPos, Pozycja ballVel, double radius, double startAngle = 0, double endAngle = 0)
        {
            if (radius == 0) return (double.NegativeInfinity, double.PositiveInfinity);

            // Wypadkowe Vel (xv - xov, yv - yov)
            Pozycja wypVel = objVel - ballVel;
            // Wypadkowe Pos (xs - xo, ys - yo)
            Pozycja wypPos = objPos - ballPos;

            // at^2 + bt + c = 0
            double a = wypVel.X * wypVel.X + wypVel.Y * wypVel.Y;
            double b = 2 * (wypPos.X * wypVel.X + wypPos.Y * wypVel.Y);
            double c = wypPos.X * wypPos.X + wypPos.Y * wypPos.Y - radius * radius;

            // delt = b^2 - 4ac
            double delt = b * b - 4 * a * c;
            if (delt <= 0) return (double.NegativeInfinity, double.PositiveInfinity);

            double t1 = (-b - Math.Sqrt(delt)) / (2 * a);
            double t2 = (-b + Math.Sqrt(delt)) / (2 * a);

            if (t1 >= 0 || t2 >= 0)
            {
                // test start i end Angle:
                startAngle %= 360;
                endAngle %= 360;
                if (endAngle < startAngle)
                {
                    (endAngle, startAngle) = (startAngle, endAngle);
                }

                // Testowanie czy należą pozycje kolizji do łuku rysowanego od startAngle do endAngle
                double tmin = Math.Min(t1, t2);
                double tmax = Math.Max(t1, t2);
                if (startAngle == endAngle) return (tmin, tmax);

                // Tesotowanie czy min(t1, t2) należy do łuku
                Pozycja cPos = objPos + objVel * tmin;
                Pozycja bPos = ballPos + ballVel * tmin;

                double cos = (cPos.X - bPos.X) / radius;
                double radians = Math.Asin((cPos.Y - bPos.Y) / radius);
                if (cos < 0) radians += Math.PI;

                double alpha = ((radians * 180) / Math.PI) % 360;
                if (alpha < startAngle || alpha > endAngle) tmin = double.NegativeInfinity;

                // Testowanie czy max(t1, t2) należy do łuku
                cPos = objPos + objVel * tmax;
                bPos = ballPos + ballVel * tmax;

                cos = (cPos.X + bPos.X) / radius;
                radians = Math.Asin((cPos.Y - bPos.Y) / radius);
                if (cos < 0) radians += Math.PI;

                alpha = ((radians * 180) / Math.PI) % 360;
                if (alpha < startAngle || alpha > endAngle) tmax = double.PositiveInfinity;

                return (tmin, tmax);
            }

            return (double.NegativeInfinity, double.PositiveInfinity);
        }

        /*public static double TimeOfCollisionWithStaticRect(Pos2D objPos, Pos2D objVel, Pos2D rectPos, double width, double height)
        {
            Pos2D topLeft = new(rectPos.X - width / 2, rectPos.Y - height / 2);
            Pos2D topRight = new(rectPos.X + width / 2, rectPos.Y - height / 2);
            Pos2D bottomLeft = new(rectPos.X - width / 2, rectPos.Y + height / 2);
            Pos2D bottomRight = new(rectPos.X + width / 2, rectPos.Y + height / 2);

            // Czas kolizji z lewą scianą boczną
            double tl = TimeOfCollisionWithStaticLine(objPos, objVel, topLeft, bottomLeft);
            double t = tl;

            double tt = TimeOfCollisionWithStaticLine(objPos, objVel, topLeft, topRight);
            if (tt < t) { t = tt; }

            double tr = TimeOfCollisionWithStaticLine(objPos, objVel, topRight, bottomRight);
            if (tr < t) { t = tr; }

            double tb = TimeOfCollisionWithStaticLine(objPos, objVel, bottomRight, bottomLeft);
            if (tb < t) { t = tb; }

            return t;
        }*/

        /*public static double TimeOfCollisionWithStaticRoundedRect(Pos2D objPos, Pos2D objVel, Pos2D rectPos, double width, double height, double radius)
        {
            // Lewa ściana boczna:
            Pos2D leftLine1 = new(rectPos.X - width / 2, rectPos.Y + (height / 2 - radius));
            Pos2D leftLine2 = new(rectPos.X - width / 2, rectPos.Y - (height / 2 - radius));
            double tl = TimeOfCollisionWithStaticLine(objPos, objVel, leftLine1, leftLine2);
            double t = tl;

            // Górna ściana:
            Pos2D topLine1 = new(rectPos.X - (width / 2 - radius), rectPos.Y - height / 2);
            Pos2D topLine2 = new(rectPos.X + (width / 2 - radius), rectPos.Y - height / 2);
            double tt = TimeOfCollisionWithStaticLine(objPos, objVel, topLine1, topLine2);
            if (tt < t) { t = tt; }

            // Prawa ściana boczna:
            Pos2D rightLine1 = new(rectPos.X + width / 2, rectPos.Y - (height / 2 - radius));
            Pos2D rightLine2 = new(rectPos.X + width / 2, rectPos.Y + (height / 2 - radius));
            double tr = TimeOfCollisionWithStaticLine(objPos, objVel, rightLine1, rightLine2);
            if (tr < t) { t = tr; }

            // Dolna ściana:
            Pos2D bottomLine1 = new(rectPos.X + (width / 2 - radius), rectPos.Y + height / 2);
            Pos2D bottomLine2 = new(rectPos.X - (width / 2 - radius), rectPos.Y + height / 2);
            double tb = TimeOfCollisionWithStaticLine(objPos, objVel, bottomLine1 , bottomLine2);
            if (tb < t) { t = tb; }

            // Górny lewy łuk:
            Pos2D topLeftPos = new(rectPos.X - (width / 2 - radius), rectPos.Y - (height / 2 - radius));
            double ttl = TimeOfCollisionWithStaticCircle(objPos, objVel, topLeftPos, radius, 90, 180);
            if (ttl < t) { t = ttl; }

            // Górny prawy łuk:
            Pos2D topRightPos = new(rectPos.X + (width / 2 - radius), rectPos.Y - (height / 2 - radius));
            double ttr = TimeOfCollisionWithStaticCircle(objPos, objVel, topRightPos, radius, 0, 90);
            if (ttr < t) { t = ttr; }

            // Dolny prawy łuk:
            Pos2D bottomRightPos = new(rectPos.X + (width / 2 - radius), rectPos.Y + (height / 2 - radius));
            double tbr = TimeOfCollisionWithStaticCircle(objPos, objVel, bottomRightPos, radius, 270, 360);
            if (tbr < t) { t = tbr; }

            // Dolny lewy łuk:
            Pos2D bottomLeftPos = new(rectPos.X - (width / 2 - radius), rectPos.Y + (height / 2 - radius));
            double tbl = TimeOfCollisionWithStaticCircle(objPos, objVel, bottomLeftPos, radius, 180, 270);
            if (tbl < t) { t = tbl; }

            return t;
        }*/

        // Calculating Bounce
        public static (Pozycja, Pozycja) VelocitiesAfterCollision(Pozycja vel1, double mass1, Pozycja vel2, double mass2)
        {
            if (mass1 == 0 && mass2 == 0) return (Pozycja.Zero, Pozycja.Zero);

            double massSum = mass1 + mass2;
            Pozycja newVel1 = ((mass1 - mass2) * vel1 + 2 * mass2 * vel2) / massSum;
            Pozycja newVel2 = (2 * mass1 * vel1 + (mass2 - mass1) * vel2) / massSum;

            return (newVel1, newVel2);
        }

        public static (Pozycja, Pozycja) VelocitiesAfterBallsCollision(Pozycja vel1, double mass1, Pozycja center1, Pozycja vel2, double mass2, Pozycja center2)
        {
            if (mass1 == 0 && mass2 == 0) return (Pozycja.Zero, Pozycja.Zero);

            Pozycja centerLine = (center1 - center2);
            if (centerLine.X < 0) centerLine *= -1;

            // Kąt pomiędzy osią X a linią łączącą środki kul
            double alpha = Math.Acos(centerLine.X / centerLine.Length);
            // Kąt pomiędzy linią łączącą środki kul a wektorem prędkości pierwszej kuli
            double theta1 = Math.Acos(vel1.X / vel1.Length);
            // Kąt pomiędzy linią łączącą środki kul a wektorem prędkości pierwszej kuli
            double theta2 = Math.Acos(vel2.X / vel2.Length);

            double massSum = mass1 + mass2;

            // Nowy wektor prędkości dla 1 ciała
            double v1FirstHalf = vel1.Length * Math.Cos(theta1 - alpha) * (mass1 - mass2) + 2 * mass2 * vel2.Length * Math.Cos(theta2 - alpha);
            double v1SecondHalf = vel1.Length * Math.Sin(theta1 - alpha);
            double v1x = (v1FirstHalf * Math.Cos(alpha)) / massSum + v1SecondHalf * Math.Cos(alpha + Math.PI / 2);
            double v1y = (v1FirstHalf * Math.Sin(alpha)) / massSum + v1SecondHalf * Math.Sin(alpha + Math.PI / 2);
            Pozycja newVel1 = new(v1x, v1y);

            // Nowy wektor prędkości dla 2 ciała
            double v2FirstHalf = vel2.Length * Math.Cos(theta2 - alpha) * (mass2 - mass1) + 2 * mass1 * vel1.Length * Math.Cos(theta1 - alpha);
            double v2SecondHalf = vel2.Length * Math.Sin(theta2 - alpha);
            double v2x = (v2FirstHalf * Math.Cos(alpha)) / massSum + v2SecondHalf * Math.Cos(alpha + Math.PI / 2);
            double v2y = (v2FirstHalf * Math.Sin(alpha)) / massSum + v2SecondHalf * Math.Sin(alpha + Math.PI / 2);
            Pozycja newVel2 = new(v2x, v2y);

            return (newVel1, newVel2);
        }
    }
}