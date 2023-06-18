using Dane;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Logika
{
    internal class SimulationManager : IDisposable
    {
        private readonly Scena m_scena;
        private readonly DaneApiBase dane;

        public List<IKula> Kule { get; private set; }
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
            this.Kule = new List<IKula>();
        }

        public void StworzKule(uint ballsNum, double minSzybkosc, double maxSzybkosc)
        {
            Pozycja scenaPoz = new(ScenaWidth, ScenaHeight);
            for (int i = 0; i < ballsNum; i++)
            {
                Kula newKula = this.dane.StworzKule(Pozycja.Zero, new Pozycja(ScenaWidth, ScenaHeight), minSzybkosc, maxSzybkosc);
                while (IsInBall(newKula))
                {
                    double minX = Pozycja.Zero.X + newKula.GetPromien();
                    double maxX = scenaPoz.X - newKula.GetPromien();

                    double minY = Pozycja.Zero.Y + newKula.GetPromien();
                    double maxY = scenaPoz.Y - newKula.GetPromien();

                    Random rnd = new();
                    newKula.SetPoz(new(rnd.NextDouble() * (maxX - minX) + minX, rnd.NextDouble() * (maxY - minY) + minY));
                }

                newKula.OnPositionChanged += CheckCollisions;
                //newKula.OnPositionChanged += LogPoz;
                Kule.Add(newKula);
            }
        }

        public void StartSimulation()
        {
            foreach (IKula ball in Kule)
            {
                ball.StartThread();
            }
        }

        public void StopSimulation()
        {
            foreach (IKula ball in Kule)
            {
                ball.EndThread();
            }
        }

        public void ClearBalls()
        {
            StopSimulation();
            foreach (IKula ball in Kule)
            {
                ball?.Dispose();
            }
            Kule.Clear();
        }
        //void LogPoz(object source, PositionChangedEventArgs e)
        //{
        //IKula kula = (IKula)source;
        //_logger.LogInfo($"{kula.ToString}");
        //}
        private bool IsInBall(IKula kula)
        {
            lock (Kule)
            {
                foreach (IKula k in Kule)
                {
                    double x = k.GetPoz().X - kula.GetPoz().X;
                    double y = k.GetPoz().Y - kula.GetPoz().Y;
                    double r = k.GetPromien() + kula.GetPromien();

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
            IKula kula = (IKula)source;
            Pozycja lastPos = e.LastPoz;
            Pozycja currVel = e.Szybkosc;
            double totalTime = e.ElapsedSeconds;
            double currentTime = totalTime;
            (lastPos, currVel, currentTime) = CheckBallsCollisions(kula, lastPos, currVel, currentTime, totalTime);
            (lastPos, currVel, currentTime) = CheckSceneBordersCollisions(kula, lastPos, currVel, currentTime, totalTime);
        }

        private (Pozycja, Pozycja, double) CheckBallsCollisions(IKula kula, Pozycja lastPos, Pozycja lastVel, double currentTime, double totalTime)
        {
            // Other Balls List (In Move Dist to current Ball)
            double moveDist = lastVel.Length + kula.GetPromien();
            List<(double, IKula)> nearBalls = new();
            for (int j = 0; j < Kule.Count; j++)
            {
                if (Kule[j] != kula)
                {
                    double dist = Math.Clamp((Kule[j].GetPoz() - lastPos).Length - Kule[j].GetPromien(), 0, double.MaxValue);
                    nearBalls.Add((dist, Kule[j]));
                }
            }
            // Sort By Dist
            nearBalls = nearBalls.OrderBy((e1) => e1.Item1).ToList();
            double tcmin, tcmax;
            for (int j = 0; j < nearBalls.Count; j++)
            {
                (double dist, IKula k) = nearBalls[j];
                Pozycja lastPos2 = k.GetPoz();
                Pozycja vel2 = k.GetPoz();
                (tcmin, tcmax) = CollisionManager.TimesOfCollisionWithBall(lastPos, lastVel, lastPos2, vel2, kula.GetPromien() + k.GetPromien());
                if (tcmin != double.NegativeInfinity && tcmax != double.PositiveInfinity)
                {
                    if (tcmin >= 0 && tcmax >= 0)
                    {
                        if (tcmin <= currentTime)
                        {
                            lastPos += lastVel * tcmin;
                            kula.SetPoz(lastPos);
                            Pozycja newPozycja = lastPos2 + vel2 * tcmin;
                            k.SetPoz(newPozycja);
                            //(lastVel, vel2) = CollisionManager.VelocitiesAfterBallsCollision(lastVel, ball.GetMass(), lastPos, vel2, b.GetMass(), newPos2);
                            (lastVel, vel2) = CollisionManager.VelocitiesAfterCollision(lastVel, kula.GetMasa(), vel2, k.GetMasa());
                            kula.SetSzybkosc(lastVel);
                            k.SetSzybkosc(vel2);
                            currentTime -= tcmin;
                        }
                    }
                    else if (tcmin < 0 && tcmax >= 0)
                    {
                        if (tcmin >= currentTime - totalTime)
                        {
                            lastPos += lastVel * tcmin;
                            kula.SetPoz(lastPos);
                            Pozycja newPozycja = lastPos2 + vel2 * tcmin;
                            k.SetPoz(newPozycja);
                            //(lastVel, vel2) = CollisionManager.VelocitiesAfterBallsCollision(lastVel, ball.GetMass(), lastPos, vel2, b.GetMass(), newPos2);
                            (lastVel, vel2) = CollisionManager.VelocitiesAfterCollision(lastVel, kula.GetMasa(), vel2, k.GetMasa());
                            kula.SetSzybkosc(lastVel);
                            k.SetSzybkosc(vel2);
                            currentTime -= tcmin;
                        }
                    }
                }
                else if (tcmin != double.NegativeInfinity)
                {
                    if (tcmin >= 0 && tcmin <= currentTime)
                    {
                        lastPos += lastVel * tcmin;
                        kula.SetPoz(lastPos);
                        Pozycja newPozycja = lastPos2 + vel2 * tcmin;
                        k.SetPoz(newPozycja);
                        //(lastVel, vel2) = CollisionManager.VelocitiesAfterBallsCollision(lastVel, ball.GetMass(), lastPos, vel2, b.GetMass(), newPos2);
                        (lastVel, vel2) = CollisionManager.VelocitiesAfterCollision(lastVel, kula.GetMasa(), vel2, k.GetMasa());
                        kula.SetSzybkosc(lastVel);
                        k.SetSzybkosc(vel2);
                        currentTime -= tcmin;
                    }
                }
                else if (tcmax != double.PositiveInfinity)
                {
                    if (tcmax >= 0 && tcmax <= currentTime)
                    {
                        lastPos += lastVel * tcmax;
                        kula.SetPoz(lastPos);
                        Pozycja newPozycja = lastPos2 + vel2 * tcmax;
                        k.SetPoz(newPozycja);
                        //(lastVel, vel2) = CollisionManager.VelocitiesAfterBallsCollision(lastVel, ball.GetMass(), lastPos, vel2, b.GetMass(), newPos2);
                        (lastVel, vel2) = CollisionManager.VelocitiesAfterCollision(lastVel, kula.GetMasa(), vel2, k.GetMasa());
                        kula.SetSzybkosc(lastVel);
                        k.SetSzybkosc(vel2);
                        currentTime -= tcmax;
                    }
                }
            }

            return (lastPos, lastVel, currentTime);
        }
        private (Pozycja, Pozycja, double) CheckSceneBordersCollisions(IKula kula, Pozycja lastPos, Pozycja lastVel, double currentTime, double totalTime)
        {
            Pozycja górnyLewyPunktSceny = new() { X = 0 + kula.GetPromien(), Y = 0 + kula.GetPromien() };
            Pozycja górnyPrawyPunktSceny = new() { X = ScenaWidth - kula.GetPromien(), Y = 0 + kula.GetPromien() };
            Pozycja dolnyPrawyPunktSceny = new() { X = ScenaWidth - kula.GetPromien(), Y = ScenaHeight - kula.GetPromien() };
            Pozycja dolnyLewyPunktSceny = new() { X = 0 + kula.GetPromien(), Y = ScenaHeight - kula.GetPromien() };

            if (currentTime >= 0)
            {
                double tc;
                // Plane Top Line:
                if (lastVel.Y < 0 && lastPos.Y > górnyLewyPunktSceny.Y)
                {
                    tc = CollisionManager.TimeOfCollisionWithLine(lastPos, lastVel, górnyLewyPunktSceny, górnyPrawyPunktSceny,Pozycja.Zero);
                    if (tc != double.PositiveInfinity && tc >= 0 && tc <= currentTime)
                    {
                        lastPos += lastVel * tc;
                        kula.SetPoz(lastPos);
                        lastVel.Y *= -1;
                        kula.SetSzybkosc(lastVel);
                        currentTime -= tc;
                    }
                }

                // Plane Right Line
                if (lastVel.X > 0 && lastPos.X < górnyPrawyPunktSceny.X)
                {
                    tc = CollisionManager.TimeOfCollisionWithLine(lastPos, lastVel, górnyPrawyPunktSceny, dolnyPrawyPunktSceny, Pozycja.Zero);
                    if (tc != double.PositiveInfinity && tc >= 0 && tc <= currentTime)
                    {
                        lastPos += lastVel * tc;
                        kula.SetPoz(lastPos);
                        lastVel.X *= -1;
                        kula.SetSzybkosc(lastVel);
                        currentTime -= tc;
                    }
                }

                // Plane Bottom Line:
                if (lastVel.Y > 0 && lastPos.Y < dolnyPrawyPunktSceny.Y)
                {
                    tc = CollisionManager.TimeOfCollisionWithLine(lastPos, lastVel, dolnyLewyPunktSceny, dolnyPrawyPunktSceny, Pozycja.Zero);
                    if (tc != double.PositiveInfinity && tc >= 0 && tc <= currentTime)
                    {
                        lastPos += lastVel * tc;
                        kula.SetPoz(lastPos);
                        lastVel.Y *= -1;
                        kula.SetSzybkosc(lastVel);
                        currentTime -= tc;
                    }
                }

                // Plane Left Line
                if (lastVel.X < 0 && lastPos.X < dolnyLewyPunktSceny.X)
                {
                    tc = CollisionManager.TimeOfCollisionWithLine(lastPos, lastVel, dolnyLewyPunktSceny, górnyLewyPunktSceny, Pozycja.Zero);
                    if (tc != double.PositiveInfinity && tc >= 0 && tc <= currentTime)
                    {
                        lastPos += lastVel * tc;
                        kula.SetPoz(lastPos);
                        lastVel.X *= -1;
                        kula.SetSzybkosc(lastVel);
                        currentTime -= tc;
                    }
                }

                kula.SetPoz(lastPos + lastVel * currentTime);
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
        public static double TimeOfCollisionWithLine(Pozycja startObjPos, Pozycja objVel, Pozycja linePoint1, Pozycja linePoint2, Pozycja lineVel)
        {
            if (objVel == Pozycja.Zero && lineVel == Pozycja.Zero) return double.PositiveInfinity;
            if (objVel - lineVel == Pozycja.Zero) return double.PositiveInfinity;

            // Ax + By + C = 0
            double A = linePoint1.Y - linePoint2.Y;
            double B = linePoint2.X - linePoint1.X;
            double C = (linePoint1.X * linePoint2.Y) - (linePoint2.X * linePoint1.Y);

            double tc = (-A * startObjPos.X - B * startObjPos.Y - C) / (A * objVel.X + B * objVel.Y);
            if (tc < 0) return double.PositiveInfinity;

            // Sprawdzanie czy punkt leży na lini ograniczonej punktami
            Pozycja cPos = startObjPos + objVel * tc;
            Pozycja lPos1 = linePoint1 * tc;

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
        public static (double, double) TimesOfCollisionWithStaticBall(Pozycja objPos, Pozycja objVel, Pozycja ballPos, double radius, double startAngle = 0, double endAngle = 0)
        {
            return TimesOfCollisionWithBall(objPos, objVel, ballPos, Pozycja.Zero, radius, startAngle, endAngle);
        }
        public static (double,double) TimesOfCollisionWithBall(Pozycja objPos, Pozycja objVel, Pozycja ballPos, Pozycja ballVel, double radius, double startAngle = 0, double endAngle = 0)
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

                // Testowanie czy min(t1, t2) należy do łuku
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
