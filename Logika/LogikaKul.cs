using Dane;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Logika
{
    internal class SimKontroler : LogikaAbstractApi
    {
        public override IEnumerable<Kula> Kule => simMenager.Kule;

        private readonly ISet<IObserver<IEnumerable<Kula>>> obs;
        private readonly DaneAbstractApi dane;
        private readonly SimMenager simMenager;

        private bool flag = false;

        public SimKontroler(DaneAbstractApi? dane = default)
        {
            this.dane = dane ?? DaneAbstractApi.StworzDaneApi();
            simMenager = new SimMenager(new Scena(this.dane.SzerokoscSceny, this.dane.WysokoscSceny),
                this.dane.SrednicaKuli);
            obs = new HashSet<IObserver<IEnumerable<Kula>>>();
        }

        public override void GenerowanieKul(int liczba_kul)
        {
            simMenager.RandGenKul(liczba_kul);
        }

        public override void Sim()
        {
            while (flag)
            {
                simMenager.PchnijK();
                SledzKule(Kule);
                Thread.Sleep(10);
            }
        }

        public override void StartSim()
        {
            if (!flag)
            {
                flag = true;
                Task.Run(Sim);
            }
        }

        public override void StopSim()
        {
            if (flag)
            {
                flag = false;
            }
        }

        public override IDisposable Subscribe(IObserver<IEnumerable<Kula>> _observer)
        {
            obs.Add(_observer);
            return new Unsubcriber(obs, _observer);
        }

        private class Unsubcriber : IDisposable//mechanizm zrzucania nieprzydzielonych zasobów
        {
            private readonly ISet<IObserver<IEnumerable<Kula>>> observers;
            private readonly IObserver<IEnumerable<Kula>> observer;

            public Unsubcriber(ISet<IObserver<IEnumerable<Kula>>> observers, IObserver<IEnumerable<Kula>> observer)
            {
                this.observers = observers;
                this.observer = observer;
            }
            public void Dispose()//wymog do Disposable
            {
                if (observer is not null)
                {
                    observers.Remove(observer);
                }
            }
        }

        public void SledzKule(IEnumerable<Kula> kule)
        {
            foreach (var observer in obs)
            {
                if (kule is null)
                {
                    observer.OnError(new NullReferenceException("Obiekt Kula jest null"));
                }
                else
                {
                    observer.OnNext(kule);
                }
            }
        }
    }
    public class SimMenager
    {
        private const float maxSzybkosc = 11;

        private readonly Scena Scena;
        private readonly int SrednicaKuli;
        private readonly Random rand;

        public SimMenager(Scena scena, int srednicaKuli)
        {
            Scena=scena;
            SrednicaKuli = srednicaKuli;
            rand = new Random();
            Kule = new List<Kula>();
        }

        public IList<Kula> Kule { get; private set; }

        public void PchnijK()
        {
            foreach (var k in Kule)
            {
                Thread thread = new Thread(() =>
                {
                    k.Poruszanie(Scena.GranicaX, Scena.GranicaY);
                });
                thread.Start();
            }
        }

        private Vector2 GetRandPozycja()
        {
            int x = rand.Next((SrednicaKuli / 2), Scena.Szerokosc - (SrednicaKuli / 2)) - 15;
            int y = rand.Next((SrednicaKuli / 2), Scena.Wysokosc - (SrednicaKuli / 2)) - 10;
            return new Vector2(x, y);
        }

        private Vector2 GetRandSzybkosc()
        {
            double x = rand.NextDouble() * (maxSzybkosc / 2f);
            double y = rand.NextDouble() * (maxSzybkosc / 2f);
            return new Vector2((float)x, (float)y);
        }

        public IList<Kula> RandGenKul(int liczba_kul)
        {
            Kule = new List<Kula>(liczba_kul);

            for (int i = 0; i < liczba_kul; i++)
            {
                Vector2 pozycja = GetRandPozycja();
                Vector2 speed = GetRandSzybkosc();
                Kule.Add(new Kula(SrednicaKuli, speed, pozycja));
            }
            return Kule;
        }
    }
    public struct Vector2 : IEquatable<Vector2>
    {
        public static readonly Vector2 Zero = new Vector2(0, 0);

        public float X { get; set; }
        public float Y { get; set; }
        public Vector2(float x, float y)
        {
            this.X = x;
            this.Y = y;
        }

        public bool Equals(Vector2 other)
        {
            float xDiff = X - other.X;
            float yDiff = Y - other.Y;
            return xDiff * xDiff + yDiff * yDiff < 9.99999944E-11f;
        }

        public bool CzyZero()
        {
            return Equals(Zero);
        }

        public void Deconstruct(out float x, out float y)
        {
            x = this.X;
            y = this.Y;
        }


        public static Vector2 operator +(Vector2 lhs, Vector2 rhs)
        {
            return new Vector2
            {
                X = lhs.X + rhs.X,
                Y = lhs.Y + rhs.Y,
            };
        }

        public static bool operator ==(Vector2 lhs, Vector2 rhs)
        {
            return lhs.Equals(rhs);
        }

        public static bool operator !=(Vector2 lhs, Vector2 rhs)
        {
            return !(lhs == rhs);
        }

    }
}
