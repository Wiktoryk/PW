using System;

namespace Dane
{
    public delegate void PositionChangedEventHandler(object source, PositionChangedEventArgs e);

    public class PositionChangedEventArgs : EventArgs
    {
        public Pozycja LastPoz { get; private set; }
        public Pozycja NewPoz { get; private set; }
        public double ElapsedSeconds { get; private set; }

        public PositionChangedEventArgs(Pozycja lastPoz, Pozycja newPoz, double seconds)
        {
            LastPoz = lastPoz;
            NewPoz = newPoz;
            ElapsedSeconds = seconds;
        }
    }
}
