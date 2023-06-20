using System;

namespace Dane
{
    public delegate void PositionChangedEventHandler(object source, PositionChangedEventArgs e);

    public class PositionChangedEventArgs : EventArgs
    {
        public Pozycja LastPos { get; private set; }
        public Pozycja Vel { get; private set; }
        public double ElapsedSeconds { get; private set; }

        public PositionChangedEventArgs(Pozycja lastPos, Pozycja vel, double seconds)
        {
            LastPos = lastPos;
            Vel = vel;
            ElapsedSeconds = seconds;
        }
    }
}
