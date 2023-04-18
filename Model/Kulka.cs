using Logika;
using System.Numerics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class KulkaModel
    {
        private readonly Kula kulka;

        public KulkaModel(Kula _kulka)
        {
            this.kulka = _kulka;
        }

        public int Srednica => kulka.Srednica;
        public Logika.Vector2 Pozycja => kulka.Pozycja;
        public Logika.Vector2 Szybkosc => kulka.Szybkosc;
    }
    public class WalidatorKulek : InterfaceValidator<int>
    {
        private readonly int min;
        private readonly int max;

        public WalidatorKulek(int min, int max)
        {
            this.min = min;
            this.max = max;
        }

        public WalidatorKulek() : this(Int32.MinValue) { }

        public WalidatorKulek(int min) : this(min, Int32.MaxValue) { }

        public bool IsValid(int val)
        {
            return val.IsBetween(min, max);
        }
    }
    public interface InterfaceValidator<T>
    {
        bool IsValid(T value);
    }
}