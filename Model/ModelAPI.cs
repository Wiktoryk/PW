using System;
using System.Collections.ObjectModel;
using Logika;

namespace Model
{
    public abstract class ModelAPIAbstrakcyjne
    {
        public static ModelAPIAbstrakcyjne StworzAPI(LogikaAPIAbstrakcyjne logikaAPIAbstrakcyjne)
        {
            return new ModelApi();
        }
        public abstract void StworzScene(int licznoscKul, int promienKul);
        public abstract ObservableCollection<Kulka> PobierzWszystkieKulki();
        public abstract void Wlacz();
        public abstract void Wylacz();
        public abstract bool CzyWlaczone();
        public sealed class ModelApi : ModelAPIAbstrakcyjne
        {
            private LogikaAPIAbstrakcyjne logikaApi = LogikaAPIAbstrakcyjne.StworzAPI(null);
            private ObservableCollection<Kulka> kulki = new ObservableCollection<Kulka>();
            public ObservableCollection<Kulka> Kulki
            {
                get
                {
                    return kulki;
                }
                set
                {
                    kulki = value;
                }
            }

            public ModelApi(LogikaAPIAbstrakcyjne logikaAPIAbstrakcyjne = null)
            {
                if (logikaAPIAbstrakcyjne == null)
                {
                    this.logikaApi = LogikaAPIAbstrakcyjne.StworzAPI(null);
                }
                else
                {
                    this.logikaApi = logikaAPIAbstrakcyjne;
                }
            }

            public override void StworzScene(int licznoscKul, int promienKul)
            {
                logikaApi.StworzScene(750, 600, licznoscKul, promienKul);
            }

            public override ObservableCollection<Kulka> PobierzWszystkieKulki()
            {
                List<Kula> kule = logikaApi.PobierzKule();
                Kulki.Clear();
                foreach (Kula kula in kule)
                {
                    Kulki.Add(new Kulka(kula));
                }
                return Kulki;
            }

            public override void Wlacz()
            {
                logikaApi.Wlacz();
            }

            public override void Wylacz()
            {
                logikaApi.Wylacz();
            }

            public override bool CzyWlaczone()
            {
                return logikaApi.CzyWlaczone();
            }
        }
    }
}