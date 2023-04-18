using Logika;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class ScenaModel
    {
        private readonly Scena scena;

        public ScenaModel(Scena _scena)
        {
            this.scena = _scena;
        }

        public int Wysokosc => scena.Wysokosc;
        public int Szerokosc => scena.Szerokosc;
    }
}
