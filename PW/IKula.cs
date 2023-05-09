using System;

namespace Dane
{
    public interface IKula : INotifyPositionChanged, IDisposable
    {
        #region KulaBase
        void SetPromien(double promien);
        void SetPoz(Pozycja poz);
        void SetSzybkosc(Pozycja szybkosc);
        //void SetMasa(double masa);
        long GetId();
        double GetPromien();
        Pozycja GetPoz();
        Pozycja GetSzybkosc();
        double GetMasa();

        #endregion KulaBase

        #region Thread
        void StartThread();
        void EndThread();

        #endregion Thread
    }
}
