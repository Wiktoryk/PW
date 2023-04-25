using Dane;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
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
            Kule = new List<IKula>();
            for (int i = 0; i < ballsNum; i++)
            {
                Kula newKula = this.dane.StworzKule(Pozycja.Zero, new Pozycja(ScenaWidth, ScenaHeight), minSzybkosc, maxSzybkosc);
                newKula.OnPositionChanged += CheckCollisions;
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

        private void CheckCollisions(object source, PositionChangedEventArgs e)
        {
            IKula ball = (IKula)source;
            Pozycja currVel = ball.GetSzybkosc();
            Pozycja newPos = e.NewPoz;
            (newPos, currVel) = CheckCollisionsRecursion(e.LastPoz, newPos, currVel, ball.GetPromien(), e.ElapsedSeconds);

            if (currVel != ball.GetSzybkosc())
            {
                ball.SetSzybkosc(currVel);
            }

            if (newPos != e.NewPoz)
            {
                ball.SetPoz(newPos);
            }
        }

        private (Pozycja, Pozycja) CheckCollisionsRecursion(Pozycja lastPoz, Pozycja newPoz, Pozycja obecnaSzybkosc, double promien, double totalTime, uint maxNum = 3, uint num = 0)
        {
            Pozycja topLeftPlanePoint = new() { X = 0 + promien, Y = 0 + promien };
            Pozycja topRightPlanePoint = new() { X = ScenaWidth - promien, Y = 0 + promien };
            Pozycja bottomRightPlanePoint = new() { X = ScenaWidth - promien, Y = ScenaHeight - promien };
            Pozycja bottomLeftPlanePoint = new() { X = 0 + promien, Y = ScenaHeight - promien };

            if (totalTime > 0)
            {
                double tc;
                if (newPoz.Y <= topLeftPlanePoint.Y && obecnaSzybkosc.Y < 0)
                {
                    tc = CollisionManager.TimeOfCollisionWithLine(lastPoz, obecnaSzybkosc, topLeftPlanePoint, topRightPlanePoint);
                    if (tc != double.PositiveInfinity && tc >= 0 && tc <= totalTime)
                    {
                        lastPoz = obecnaSzybkosc * tc + lastPoz;
                        obecnaSzybkosc.Y *= -1;
                        totalTime -= tc;
                        newPoz = obecnaSzybkosc * totalTime + lastPoz;
                    }
                }

                if (newPoz.X >= topRightPlanePoint.X && obecnaSzybkosc.X > 0)
                {
                    tc = CollisionManager.TimeOfCollisionWithLine(lastPoz, obecnaSzybkosc, topRightPlanePoint, bottomRightPlanePoint);
                    if (tc != double.PositiveInfinity && tc >= 0 && tc <= totalTime)
                    {
                        lastPoz = obecnaSzybkosc * tc + lastPoz;
                        obecnaSzybkosc.X *= -1;
                        totalTime -= tc;
                        newPoz = obecnaSzybkosc * totalTime + lastPoz;
                    }
                }

                if (newPoz.Y >= bottomRightPlanePoint.Y && obecnaSzybkosc.Y > 0)
                {
                    tc = CollisionManager.TimeOfCollisionWithLine(lastPoz, obecnaSzybkosc, bottomLeftPlanePoint, bottomRightPlanePoint);
                    if (tc != double.PositiveInfinity && tc >= 0 && tc <= totalTime)
                    {
                        lastPoz = obecnaSzybkosc * tc + lastPoz;
                        obecnaSzybkosc.Y *= -1;
                        totalTime -= tc;
                        newPoz = obecnaSzybkosc * totalTime + lastPoz;
                    }
                }

                if (newPoz.X <= bottomLeftPlanePoint.X && obecnaSzybkosc.X < 0)
                {
                    tc = CollisionManager.TimeOfCollisionWithLine(lastPoz, obecnaSzybkosc, bottomLeftPlanePoint, topLeftPlanePoint);
                    if (tc != double.PositiveInfinity && tc >= 0 && tc <= totalTime)
                    {
                        lastPoz = obecnaSzybkosc * tc + lastPoz;
                        obecnaSzybkosc.X *= -1;
                        totalTime -= tc;
                        newPoz = obecnaSzybkosc * totalTime + lastPoz;
                    }
                }
            }

            if (++num >= maxNum)
            {
                return (newPoz, obecnaSzybkosc);
            }
            return CheckCollisionsRecursion(lastPoz, newPoz, obecnaSzybkosc, promien, totalTime, maxNum, num);
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
    }
}
