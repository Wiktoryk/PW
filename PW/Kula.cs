using System.Diagnostics;
using System.Threading;
using System;
using System.Text;

namespace Dane
{
    public class Kula : IKula
    {
        #region KulaBase

        private readonly long _id;
        private double m_promien;
        private Pozycja m_poz;
        private Pozycja m_szybkosc;
        private double masa;
        private static readonly object poz_lock = new object();
        private static readonly object vel_lock = new object();
        private static readonly object promien_lock = new object();

        public Kula(long id, double promien, Pozycja poz, Pozycja szybkosc)
        {
            this._id = id;
            this.m_promien = promien;
            this.m_poz = poz;
            this.m_szybkosc = szybkosc;
            this.masa = 20.0;
            this.m_endThread = false;
            this.m_thread = new Thread(new ThreadStart(this.ThreadMethod))
            {
                IsBackground = true
            };
        }

        public void SetPromien(double promien)
        {
            lock (promien_lock)
            {
                this.m_promien = promien;
            }
        }

        public void SetPoz(Pozycja poz)
        {
            lock (poz_lock)
            {
                this.m_poz = poz;
            }
        }

        public void SetSzybkosc(Pozycja szybkosc)
        {
            lock (vel_lock)
            {
                this.m_szybkosc = szybkosc;
            }
        }

        public long GetId()
        {
            return this._id;
        }

        public double GetPromien()
        {
            lock (promien_lock)
            {
                return this.m_promien;
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
            lock (vel_lock)
            {
                return this.m_szybkosc;
            }
        }

        public double GetMasa()
        {
            return this.masa;
        }
        public override string? ToString()
        {
            return $"Kula Poz=[{GetPoz().X:n1}, {GetPoz().Y:n1}], S=[{GetSzybkosc().X:n1}, {GetSzybkosc().Y:n1}]";
        }

        #endregion KulaBase

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
        }

        private void ThreadMethod()
        {
            BallLogger.Log(new StringBuilder("Ball ").Append(this._id).Append(" thread started").ToString(), LogType.DEBUG);
            Stopwatch stopwatch = new();
            stopwatch.Start();
            while (!m_endThread)
            {
                Pozycja lastPos = this.GetPoz();

                TimeSpan elapsed = stopwatch.Elapsed;
                this.SetPoz(this.GetPoz() + this.GetSzybkosc() * elapsed.TotalSeconds);
                OnPositionChanged?.Invoke(this, new PositionChangedEventArgs(lastPos, this.m_szybkosc, elapsed.TotalSeconds));
                Pozycja newPoz = this.GetPoz();
                string message =this.ToString();
                BallLogger.Log(message, LogType.DEBUG);
                stopwatch.Restart();
                Thread.Sleep(Math.Sqrt(this.m_szybkosc.X * this.m_szybkosc.X + this.m_szybkosc.Y * this.m_szybkosc.Y)>0?
                        (int)(10 /Math.Sqrt(this.m_szybkosc.X *this.m_szybkosc.X+this.m_szybkosc.Y*this.m_szybkosc.Y)):10);
            }
        }

        public void EndThread()
        {
            if ((this.m_thread.ThreadState & System.Threading.ThreadState.Background) == System.Threading.ThreadState.Background)
            {
                this.m_endThread = true;
                this.m_thread?.Join();
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
