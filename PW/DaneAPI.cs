using System;

namespace Dane
{
    public abstract class DaneAPIAbstrakcyjne
    {
        public abstract void StworzScene(int szerokosc, int wysokosc, int licznoscKul, int promienKul);
        public abstract List<Kula> PobierzKule();
        public abstract void Wylacz();

        public abstract Scena Scena { get; }
        public static DaneAPIAbstrakcyjne StworzAPI()
        {
            return new DaneAPI();
        }

        internal sealed class DaneAPI : DaneAPIAbstrakcyjne
        {
            private readonly object locked = new object();
            private bool wlaczone = false;
            private Scena scena;

            public bool Wlaczone
            {
                get { return wlaczone; }
                set { wlaczone = value; }
            }

            public override Scena Scena
            {
                get { return scena; }
            }

            public override void StworzScene(int szerokosc, int wysokosc, int licznoscKul, int promienKul)
            {
                this.scena = new Scena(szerokosc, wysokosc, licznoscKul, promienKul);
                this.Wlaczone = true;
                List<Kula> kule = PobierzKule();

                foreach (Kula kula in kule)
                {
                    Thread t = new Thread(() =>
                    {
                        while (this.Wlaczone)
                        {
                            lock (locked)
                            {
                                kula.rusz(scena);
                            }

                            Thread.Sleep(5);
                        }
                    });
                    t.Start();
                }
            }

            public override List<Kula> PobierzKule()
            {
                return Scena.Kule;
            }

            public override void Wylacz()
            {
                this.Wlaczone = false;
            }
        }
    }
}
