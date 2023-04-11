using System;
using System.Collections.Generic;
using System.Text;

namespace Logika
{
    public class Scena
    {
        private readonly int szerokosc;
        public int Szerokosc
        {
            get { return szerokosc; }
        }

        private readonly int wysokosc;
        public int Wysokosc
        {
            get { return wysokosc; }
        }

        private bool wlaczona = false;
        public bool Wlaczona
        {
            get { return wlaczona; }
            set { wlaczona = value; }
        }

        private readonly List<Kula> kule = new List<Kula>();
        public List<Kula> Kule
        {
            get { return kule; }
        }

        public Scena(int szerokosc, int wysokosc)
        {
            this.szerokosc = szerokosc;
            this.wysokosc = wysokosc;
        }


        public void WygenerujListeKul(int licznoscKul, int promienKul)
        {
            kule.Clear();
            Random random = new Random();
            for (int i = 0; i < licznoscKul; i++)
            {
                int x = random.Next(promienKul, this.szerokosc - promienKul);
                int y = random.Next(promienKul, this.wysokosc - promienKul);
                this.kule.Add(new Kula(x,y,promienKul));
            }
        }
    }
}