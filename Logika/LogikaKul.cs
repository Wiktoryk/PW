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
        private readonly object m_lock = new object();
        private readonly Log _logger = new Log();

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
            Kule = new List<IKula>();
            for (int i = 0; i < ballsNum; i++)
            {
                Kula newKula = this.dane.StworzKule(Pozycja.Zero, new Pozycja(ScenaWidth, ScenaHeight), minSzybkosc, maxSzybkosc);
                newKula.OnPositionChanged += CheckCollisions;
                newKula.OnPositionChanged += LogPoz;
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
        void LogPoz(object source, PositionChangedEventArgs e)
        {
            Kula kula = (Kula)source;
            _logger.LogInfo($"{kula.ToString}");
        }

        private void CheckCollisions(object source, PositionChangedEventArgs e)
        {
            IKula ball = (IKula)source;
            Pozycja currVel = ball.GetSzybkosc();
            Pozycja newPos = e.NewPoz;
            lock (Kula.move_lock)
            {
                (newPos, currVel) = CheckCollisionsRecursion(e.LastPoz, newPos, currVel, ball.GetPromien(), e.ElapsedSeconds, ball.GetMasa(), Kule, ball);
                if (currVel != ball.GetSzybkosc())
                {
                    ball.SetSzybkosc(currVel);
                }

                if (newPos != e.NewPoz)
                {
                    ball.SetPoz(newPos);
                }
            }
        }

        private (Pozycja, Pozycja) CheckCollisionsRecursion(Pozycja lastPoz, Pozycja newPoz, Pozycja obecnaSzybkosc, double promien, double totalTime, double masa, List<IKula> inne, IKula kula, uint maxNum = 3, uint num = 0)
        {
            Pozycja górnyLewyPunktSceny = new() { X = 0 + promien, Y = 0 + promien };
            Pozycja górnyPrawyPunktSceny = new() { X = ScenaWidth - promien, Y = 0 + promien };
            Pozycja dolnyPrawyPunktSceny = new() { X = ScenaWidth - promien, Y = ScenaHeight - promien };
            Pozycja dolnyLewyPunktSceny = new() { X = 0 + promien, Y = ScenaHeight - promien };

            if (totalTime > 0)
            {
                double tc;
                if (newPoz.Y <= górnyLewyPunktSceny.Y && obecnaSzybkosc.Y < 0)
                {
                    tc = CollisionManager.TimeOfCollisionWithLine(lastPoz, obecnaSzybkosc, górnyLewyPunktSceny, górnyPrawyPunktSceny);
                    if (tc != double.PositiveInfinity && tc >= 0 && tc <= totalTime)
                    {
                        lastPoz = obecnaSzybkosc * tc + lastPoz;
                        obecnaSzybkosc.Y *= -1;
                        totalTime -= tc;
                        newPoz = obecnaSzybkosc * totalTime + lastPoz;
                    }
                }

                if (newPoz.X >= górnyPrawyPunktSceny.X && obecnaSzybkosc.X > 0)
                {
                    tc = CollisionManager.TimeOfCollisionWithLine(lastPoz, obecnaSzybkosc, górnyPrawyPunktSceny, dolnyPrawyPunktSceny);
                    if (tc != double.PositiveInfinity && tc >= 0 && tc <= totalTime)
                    {
                        lastPoz = obecnaSzybkosc * tc + lastPoz;
                        obecnaSzybkosc.X *= -1;
                        totalTime -= tc;
                        newPoz = obecnaSzybkosc * totalTime + lastPoz;
                    }
                }

                if (newPoz.Y >= dolnyPrawyPunktSceny.Y && obecnaSzybkosc.Y > 0)
                {
                    tc = CollisionManager.TimeOfCollisionWithLine(lastPoz, obecnaSzybkosc, dolnyLewyPunktSceny, dolnyPrawyPunktSceny);
                    if (tc != double.PositiveInfinity && tc >= 0 && tc <= totalTime)
                    {
                        lastPoz = obecnaSzybkosc * tc + lastPoz;
                        obecnaSzybkosc.Y *= -1;
                        totalTime -= tc;
                        newPoz = obecnaSzybkosc * totalTime + lastPoz;
                    }
                }

                if (newPoz.X <= dolnyLewyPunktSceny.X && obecnaSzybkosc.X < 0)
                {
                    tc = CollisionManager.TimeOfCollisionWithLine(lastPoz, obecnaSzybkosc, dolnyLewyPunktSceny, górnyLewyPunktSceny);
                    if (tc != double.PositiveInfinity && tc >= 0 && tc <= totalTime)
                    {
                        lastPoz = obecnaSzybkosc * tc + lastPoz;
                        obecnaSzybkosc.X *= -1;
                        totalTime -= tc;
                        newPoz = obecnaSzybkosc * totalTime + lastPoz;
                    }
                }

                foreach (var kulaInna in inne)
                {
                    if (kula.GetId() != kulaInna.GetId())
                    {
                        Pozycja deltaPozycji = new Pozycja(kulaInna.GetPoz().X - kula.GetPoz().X, kulaInna.GetPoz().Y - kula.GetPoz().Y);
                        double sumaPromieni = kula.GetPromien() + kulaInna.GetPromien();
                        double odleglosc = Math.Sqrt(deltaPozycji.X * deltaPozycji.X + deltaPozycji.Y * deltaPozycji.Y);
                        double overlap = sumaPromieni - odleglosc;
                        if (Math.Abs(overlap) > 0)
                        {
                            tc = CollisionManager.TimeOfCollisionWithBall(kula, kulaInna);
                            if (tc != double.PositiveInfinity && tc >= 0 && tc <= totalTime)
                            {
                                Pozycja normal = new Pozycja(deltaPozycji.X / odleglosc, deltaPozycji.Y / odleglosc);
                                Pozycja szybkosc1 = new Pozycja(kula.GetSzybkosc().X, kula.GetSzybkosc().Y);
                                Pozycja szybkosc2 = new Pozycja(kulaInna.GetSzybkosc().X, kulaInna.GetSzybkosc().Y);
                                double dotProduct1 = (szybkosc1.X * normal.X + szybkosc1.Y * normal.Y);
                                double dotProduct2 = (szybkosc2.X * normal.X + szybkosc2.Y * normal.Y);
                                double impulse1 = (2.0 * (dotProduct2 - dotProduct1)) / (kula.GetMasa() + kulaInna.GetMasa());
                                //double impulse2 = (2.0 * (dotProduct1 - dotProduct2)) / (kula.GetMasa() + kulaInna.GetMasa());
                                szybkosc1.X += impulse1 * normal.X;
                                szybkosc1.Y += impulse1 * normal.Y;
                                //szybkosc2.X += impulse2 * normal.X;
                                //szybkosc2.Y += impulse2 * normal.Y;
                                //kulaInna.SetSzybkosc(szybkosc2);
                                //kula.SetSzybkosc(szybkosc1);
                                lastPoz = obecnaSzybkosc * tc + lastPoz;
                                obecnaSzybkosc.X = -szybkosc1.X;
                                obecnaSzybkosc.Y = -szybkosc1.Y;
                                totalTime -= tc;
                                newPoz = obecnaSzybkosc * totalTime + lastPoz;
                            }
                        }
                    }
                }
            }

            if (++num >= maxNum)
            {
                return (newPoz, obecnaSzybkosc);
            }
            return CheckCollisionsRecursion(lastPoz, newPoz, obecnaSzybkosc, promien, totalTime, masa, inne, kula, maxNum, num);
        }

        public void Dispose()
        {
            ClearBalls();
            GC.SuppressFinalize(this);
        }
    }
    internal class CollisionManager
    {
        public static double TimeOfCollisionWithLine(Pozycja startObjPos, Pozycja objVel, Pozycja linePoint1, Pozycja linePoint2)
        {
            double xDiff = linePoint2.X - linePoint1.X;
            double tc;
            if (xDiff != 0)
            {
                double a = (linePoint2.Y - linePoint1.Y) / xDiff;
                double b = linePoint1.Y - a * linePoint1.X;

                tc = (objVel.Y - a * objVel.X) != 0 ? (a * startObjPos.X + b - startObjPos.Y) / (objVel.Y - a * objVel.X) : double.PositiveInfinity;
            }
            else
            {
                tc = objVel.X != 0 ? (linePoint1.X - startObjPos.X) / objVel.X : double.PositiveInfinity;
            }

            if (tc != double.PositiveInfinity)
            {
                // punkt kolizji
                Pozycja c = objVel * tc + startObjPos;

                // minimalny i maksymalna wartość x
                double minX = linePoint1.X > linePoint2.X ? linePoint2.X : linePoint1.X;
                double maxX = linePoint1.X > linePoint2.X ? linePoint1.X : linePoint2.X;

                // minimalna i maksymalna wartość y
                double minY = linePoint1.Y > linePoint2.Y ? linePoint2.Y : linePoint1.Y;
                double maxY = linePoint1.Y > linePoint2.Y ? linePoint1.Y : linePoint2.Y;

                if (c.X < minX || c.X > maxX || c.Y < minY || c.Y > maxY)
                {
                    tc = double.PositiveInfinity;
                }
            }

            return tc;
        }
        public static double TimeOfCollisionWithBall(IKula kula1, IKula kula2)
        {
            double a = Math.Pow(kula1.GetSzybkosc().X - kula2.GetSzybkosc().X, 2) + Math.Pow(kula1.GetSzybkosc().Y - kula2.GetSzybkosc().Y, 2);
            double b = 2 * ((kula1.GetSzybkosc().X - kula2.GetSzybkosc().X) * (kula1.GetPoz().X - kula2.GetPoz().X) + (kula1.GetSzybkosc().Y - kula2.GetSzybkosc().Y) * (kula1.GetPoz().Y - kula2.GetPoz().Y));
            double c = Math.Pow(kula1.GetPoz().X - kula2.GetPoz().X, 2) + Math.Pow(kula1.GetPoz().Y - kula2.GetPoz().Y, 2) - 4 * Math.Pow(kula1.GetMasa(), 2);
            if (a == 0)
            {
                return double.PositiveInfinity;
            }

            double delta = b * b - 4 * a * c;
            if (delta < 0)
            {
                return double.PositiveInfinity;
            }

            double t1 = (-b - Math.Sqrt(delta)) / (2 * a);
            double t2 = (-b + Math.Sqrt(delta)) / (2 * a);

            if (t1 < 0 && t2 < 0)
            {
                return double.PositiveInfinity;
            }

            double tc = Math.Min(t1, t2);
            return tc;
        }
    }
}
