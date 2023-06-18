using System;

namespace Dane
{
    public delegate void PositionChangedEventHandler(object source, PositionChangedEventArgs e);

    public class PositionChangedEventArgs : EventArgs
    {
        public Pozycja LastPoz { get; private set; }
        public Pozycja Szybkosc { get; private set; }
        public double ElapsedSeconds { get; private set; }

        public PositionChangedEventArgs(Pozycja lastPoz, Pozycja szybkosc, double seconds)
        {
            LastPoz = lastPoz;
            Szybkosc = szybkosc;
            ElapsedSeconds = seconds;
        }
    }
}
