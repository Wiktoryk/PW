﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Dane
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

        public Scena(int szerokosc, int wysokosc, int licznoscKul, int promienKul)
        {
            this.szerokosc = szerokosc;
            this.wysokosc = wysokosc;
            WygenerujListeKul(licznoscKul, promienKul);
        }

        public Kula WygenerujKule(int promien)
        {
            Random random = new Random();
            int x = random.Next(promien, this.szerokosc - promien);
            int y = random.Next(promien, this.wysokosc - promien);
            return new Kula(x, y,promien);
        }

        public void WygenerujListeKul(int licznoscKul,int promienKul)
        {
            kule.Clear();
            for (int i = 0; i < licznoscKul; i++)
            {
                Kula kula = WygenerujKule(promienKul);
                this.kule.Add(kula);
            }
        }
    }
}