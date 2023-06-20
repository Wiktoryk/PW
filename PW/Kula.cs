using System.Diagnostics;
using System.Threading;
using System;
using System.Text;

namespace Dane
{
    public class Kula : IKula
    {
        #region BallBase

        private readonly long _id;
        private double m_masa;
        private double m_promien;
        private Pozycja m_poz;
        private Pozycja m_szybkosc;

        private static readonly object masa_lock = new();
        private static readonly object promien_lock = new();
        private static readonly object poz_lock = new();
        private static readonly object szybkosc_lock = new();

        public Kula(long id, double masa, double promien, Pozycja poz, Pozycja szybkosc)
        {
            this._id = id;
            this.m_masa = masa;
            this.m_promien = promien;
            this.m_poz = poz;
            this.m_szybkosc = szybkosc;
            this.m_endThread = false;
            this.m_thread = new Thread(new ThreadStart(ThreadMethod))
            {
                IsBackground = true
            };
        }

        public void SetMasa(double mass)
        {
            lock (masa_lock)
            {
                this.m_masa = mass;
            }
        }

        public void SetPromien(double radius)
        {
            lock (promien_lock)
            {
                this.m_promien = radius;
            }
        }

        public void SetSrednica(double diameter)
        {
            lock (promien_lock)
            {
                this.m_promien = diameter / 2;
            }
        }

        public void SetPoz(Pozycja pos)
        {
            lock (poz_lock)
            {
                this.m_poz = pos;
            }
        }

        public void SetSzybkosc(Pozycja vel)
        {
            lock (szybkosc_lock)
            {
                this.m_szybkosc = vel;
            }
        }

        public long GetId()
        {
            return this._id;
        }

        public double GetMasa()
        {
            lock (masa_lock)
            {
                return this.m_masa;
            }
        }

        public double GetPromien()
        {
            lock (promien_lock)
            {
                return this.m_promien;
            }
        }

        public double GetSrednica()
        {
            lock (promien_lock)
            {
                return this.m_promien * 2;
            }
        }

        public Pozycja GetPoz()
        {
            lock (poz_lock)
            {
                return this.m_poz;
            }
        }

        public Pozycja GetSzybkosc()
        {
            lock (szybkosc_lock)
            {
                return this.m_szybkosc;
            }
        }

        #endregion BallBase

        #region INotifyPositionChanged

        public event PositionChangedEventHandler? OnPositionChanged;

        #endregion INotifyPositionChanged

        #region Thread

        private Thread m_thread;
        private bool m_endThread;

        public void StartThread()
        {
            if ((this.m_thread.ThreadState & System.Threading.ThreadState.Background) == System.Threading.ThreadState.Background && (this.m_thread.ThreadState & System.Threading.ThreadState.Unstarted) == System.Threading.ThreadState.Unstarted)
            {
                m_thread.Start();
            }
            else
            {
                BallLogger.Log(new StringBuilder("Tried to start thread which was already Started or not in Background State for Ball ").Append(this._id).ToString(), LogType.WARNING);
            }
        }

        private void ThreadMethod()
        {
            BallLogger.Log(new StringBuilder("Ball ").Append(this._id).Append(" thread started").ToString(), LogType.DEBUG);
            Stopwatch stopwatch = new();
            stopwatch.Start();
            while (!m_endThread)
            {
                Pozycja lastPoz = this.GetPoz();

                TimeSpan elapsed = stopwatch.Elapsed;
                this.SetPoz(this.GetPoz() + this.GetSzybkosc() * elapsed.TotalSeconds);
                lock (poz_lock)
                {
                    OnPositionChanged?.Invoke(this, new PositionChangedEventArgs(lastPoz, this.m_szybkosc, elapsed.TotalSeconds));
                }

                Pozycja newPoz = this.GetPoz();
                string message = new StringBuilder("Ball ")
                                    .Append(_id)
                                    .Append(" changed position from {x=").Append(lastPoz.X)
                                    .Append(", y=").Append(lastPoz.Y)
                                    .Append("} to {x=").Append(newPoz.X)
                                    .Append(", y=").Append(newPoz.Y)
                                    .Append("} in ").Append(elapsed.TotalSeconds).Append(" seconds")
                                    .ToString();
                BallLogger.Log(message, LogType.DEBUG);

                stopwatch.Restart();
                Thread.Sleep(5);
            }

            BallLogger.Log(new StringBuilder("Ball ").Append(this._id).Append(" thread ended").ToString(), LogType.DEBUG);
        }

        public void EndThread()
        {
            if ((this.m_thread.ThreadState & System.Threading.ThreadState.Background) == System.Threading.ThreadState.Background)
            {
                this.m_endThread = true;
                this.m_thread?.Join();
            }
            else
            {
                BallLogger.Log(new StringBuilder("Tried to stop thread which was not in Background State for Ball ").Append(this._id).ToString(), LogType.WARNING);
            }
        }

        #endregion Thread

        #region IDisposable

        public void Dispose()
        {
            Delegate[] delegates = OnPositionChanged?.GetInvocationList() ?? Array.Empty<Delegate>();
            foreach (Delegate d in delegates)
            {
                OnPositionChanged -= (PositionChangedEventHandler)d;
            }

            EndThread();
            GC.SuppressFinalize(this);
        }

        #endregion IDisposable
    }
}
