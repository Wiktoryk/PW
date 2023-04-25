using System.Diagnostics;
using System.Threading;
using System;

namespace Dane
{
    public class Kula : IKula
    {
        #region KulaBase

        private readonly long _id;
        private double m_promien;
        private Pozycja m_poz;
        private Pozycja m_szybkosc;

        public Kula(long id, double promien, Pozycja poz, Pozycja szybkosc)
        {
            this._id = id;
            this.m_promien = promien;
            this.m_poz = poz;
            this.m_szybkosc = szybkosc;
            this.m_endThread = false;
            this.m_thread = new Thread(new ThreadStart(this.ThreadMethod))
            {
                IsBackground = true
            };
        }

        public void SetPromien(double promien)
        {
            this.m_promien = promien;
        }

        public void SetPoz(Pozycja poz)
        {
            this.m_poz = poz;
        }

        public void SetSzybkosc(Pozycja szybkosc)
        {
            this.m_szybkosc = szybkosc;
        }

        public long GetId()
        {
            return this._id;
        }

        public double GetPromien()
        {
            return this.m_promien;
        }

        public Pozycja GetPoz()
        {
            return this.m_poz;
        }

        public Pozycja GetSzybkosc()
        {
            return this.m_szybkosc;
        }

        #endregion KulaBase

        #region INotifyPositionChanged

        public event PositionChangedEventHandler? OnPositionChanged;

        #endregion INotifyPositionChanged

        #region Thread

        private readonly Thread m_thread;
        private bool m_endThread;

        public void StartThread()
        {
            if (this.m_thread.ThreadState != System.Threading.ThreadState.Background)
            {
                m_thread.Start();
            }
        }

        private void ThreadMethod()
        {
            Stopwatch stopwatch = new();
            stopwatch.Start();
            while (!m_endThread)
            {
                Pozycja lastPos = this.m_poz;

                TimeSpan elapsed = stopwatch.Elapsed;
                stopwatch.Restart();
                Pozycja newPos = lastPos + (this.m_szybkosc * elapsed.TotalSeconds);
                this.m_poz = newPos;

                OnPositionChanged?.Invoke(this, new PositionChangedEventArgs(lastPos, newPos, elapsed.TotalSeconds));
                Thread.Sleep(10);
            }
        }

        public void EndThread()
        {
            if (this.m_thread.ThreadState == System.Threading.ThreadState.Background)
            {
                this.m_endThread = true;
                this.m_thread?.Join();
            }
        }

        #endregion Thread

        #region IDisposable

        public void Dispose()
        {
            EndThread();
            GC.SuppressFinalize(this);
        }

        #endregion IDisposable
    }
}
