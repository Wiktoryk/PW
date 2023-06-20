using System;

namespace Dane
{
    public interface IKula : INotifyPositionChanged, IDisposable
    {
        #region BallBase

        void SetMasa(double mass);
        void SetPromien(double radius);
        void SetSrednica(double diameter);
        void SetPoz(Pozycja pos);
        void SetSzybkosc(Pozycja vel);
        long GetId();
        double GetMasa();
        double GetPromien();
        double GetSrednica();
        Pozycja GetPoz();
        Pozycja GetSzybkosc();

        #endregion BallBase

        #region Thread

        void StartThread();
        void EndThread();

        #endregion Thread
    }
}
