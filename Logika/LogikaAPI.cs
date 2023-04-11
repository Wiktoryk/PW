using Dane;
using System;
using System.Collections.Generic;
using System.Threading;
using static Logika.LogikaAPIAbstrakcyjne;
using static System.Formats.Asn1.AsnWriter;

namespace Logika
{
    public abstract class LogikaAPIAbstrakcyjne
    {
        public static LogikaAPIAbstrakcyjne StworzAPI(DaneAPIAbstrakcyjne daneAPIAbstrakcyjne = null)
        {
            return new LogikaAPI(daneAPIAbstrakcyjne);
        }

        public abstract void StworzScene(int szerokosc, int wysokosc, int licznoscKul, int promienKul);

        public abstract List<Kula> PobierzKule();

        public abstract void Wlacz();

        public abstract void Wylacz();

        public abstract bool CzyWlaczone();

        internal sealed class LogikaAPI : LogikaAPIAbstrakcyjne
        {
            private DaneAPIAbstrakcyjne daneApi;

            private Scena scena;
            private DaneAPIAbstrakcyjne daneAPIAbstrakcyjne;

            public LogikaAPI(DaneAPIAbstrakcyjne daneAPIAbstrakcyjne)
            {
                if (daneAPIAbstrakcyjne == null)
                {
                    this.daneApi = DaneAPIAbstrakcyjne.StworzAPI();
                }
                else
                {
                    this.daneApi = daneAPIAbstrakcyjne;
                }
            }

            public override void StworzScene(int szerokosc, int wysokosc, int licznoscKul, int promienKul)
            {
                this.scena = new Scena(szerokosc, wysokosc);
                scena.WygenerujListeKul(licznoscKul, promienKul);
                foreach (Kula kula in scena.Kule)
                {
                    Thread thread = new Thread(() =>
                    {
                        int newX;
                        int newY;

                        while (this.scena.Wlaczona)
                        {
                            Random random = new Random();
                            newX = random.Next(1, 101);
                            newY = random.Next(1, 101);
                            if(newX<scena.Szerokosc)
                            {
                                kula.X = newX;
                            }
                            if(newY<scena.Wysokosc)
                            {
                                kula.Y = newY;
                            }
                            Thread.Sleep(15);
                        }
                    });
                    thread.Start();
                }
            }

            public override List<Kula> PobierzKule()
            {
                return scena.Kule;
            }

            public override void Wlacz()
            {
                this.scena.Wlaczona = true;
            }

            public override void Wylacz()
            {
                this.scena.Wlaczona = false;
            }

            public override bool CzyWlaczone()
            {
                return this.scena.Wlaczona;
            }
        }
    }

}
