using Model;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace ViewModel
{
    public class ViewModelAPI : INotifyPropertyChanged
    {
        private ModelAPIAbstrakcyjne modelAPI=ModelAPIAbstrakcyjne.StworzAPI(null);
        private int licznoscKul = 1;
        public string LicznoscKul
        {
            get
            {
                return Convert.ToString(licznoscKul);
            }
            set
            {
                licznoscKul = Convert.ToInt32(value);
                OnPropertyChanged("OrbQuantity");
            }
        }
        private int promienKul = 25;

        public ICommand SygnalWlacz
        {
            get;
            set;
        }
        public ICommand SygnalWylacz
        {
            get;
            set;
        }

        private ObservableCollection<Kulka> listaKul;
        public ObservableCollection<Kulka> ListaKul
        {
            get { return listaKul; }
            set
            {
                if (value.Equals(this.listaKul)) return;
                listaKul = value;
                OnPropertyChanged("OrbList");
            }
        }

        public ViewModelAPI() : this(null) { }
        public ViewModelAPI(ModelAPIAbstrakcyjne modelAPI)
        {
            SygnalWlacz = new Sygnal(Wlacz);
            SygnalWylacz = new Sygnal(Wylacz);
            if (modelAPI == null)
            {
                this.modelAPI = ModelAPIAbstrakcyjne.StworzAPI(null);
            }
            else
            {
                this.modelAPI = modelAPI;
            }
        }

        private bool czyWlaczone = true;
        public bool CzyWlaczone
        {
            get { return czyWlaczone; }
            set
            {
                czyWlaczone = value;
                OnPropertyChanged("IsEnabled");
                OnPropertyChanged("IsDisabled");
            }
        }
        public bool CzyWylaczone
        {
            get
            {
                return !czyWlaczone;
            }
        }

        private void Wlacz()
        {
            modelAPI.StworzScene(licznoscKul, promienKul);
            modelAPI.Wlacz();
            czyWlaczone = true;
            ListaKul = modelAPI.PobierzWszystkieKulki();
        }

        private void Wylacz()
        {
            modelAPI.Wylacz();
            CzyWlaczone = false;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}