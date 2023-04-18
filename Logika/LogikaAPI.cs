using Dane;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logika
{
    public abstract class LogikaAbstractApi : IObservable<IEnumerable<Kula>>
    {
        public abstract IEnumerable<Kula> Kule { get; }

        public abstract void GenerowanieKul(int liczba_kul);
        public abstract void Sim();
        public abstract void StartSim();
        public abstract void StopSim();

        public abstract IDisposable Subscribe(IObserver<IEnumerable<Kula>> observer);

        public static LogikaAbstractApi StworzLogikaApi(DaneAbstractApi? dane = default)
        {
            return new SimKontroler(dane ?? DaneAbstractApi.StworzDaneApi());
        }
    }
    public static class Extension
    {
        public static bool IsBetween(this int val, int min, int max)
        {
            return val >= min && val <= max;
        }

        public static bool IsBetween(this float val, float min, float max, float pad = 0f)
        {
            if (pad < 0f)
            {
                throw new ArgumentException("Argument pad musi mieć wartość dodatnią!", nameof(pad));
            }

            return (val - pad >= min) && (val + pad <= max);
        }
    }
}
