using Model;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace ViewModel
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;//auto-generate

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));//auto-generate
        }

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = "")
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
            {
                return false;
            }
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
    public class ViewModelMain : ViewModelBase
    {
        public ViewModelBase thisViewModel { get; }

        public ViewModelMain() : base()
        {
            thisViewModel = new ViewModelSim(walidatorKulek: new WalidatorKulek(1, 20));
        }
    }
    public class ViewModelSim : ViewModelBase, IObserver<IEnumerable<KulkaModel>>
    {
        private IDisposable? unsubscriber;

        private ObservableCollection<KulkaModel> kulki;
        private readonly ApiModel logic;
        private readonly InterfaceValidator<int> validator;
        private int liczbaKulek = 5;
        private bool flag = false;

        public int LiczbaKulek
        {
            get => liczbaKulek;
            set
            {
                if (validator.IsValid(value)) SetField(ref liczbaKulek, value);
                else liczbaKulek = 1;
            }
        }
        public bool getSetFlag
        {
            get => flag;
            private set => SetField(ref flag, value);
        }
        public IEnumerable<KulkaModel> Kulki => kulki;
        public ICommand SimStartCommand { get; init; }
        public ICommand SimStopCommand { get; init; }

        public ViewModelSim(ApiModel? model = default, InterfaceValidator<int>? walidatorKulek = default)
            : base()
        {
            logic = model ?? ApiModel.StworzModelApi();
            validator = walidatorKulek ?? new WalidatorKulek();
            kulki = new ObservableCollection<KulkaModel>();

            SimStartCommand = new SimStartCommand(this);
            SimStopCommand = new SimStopCommand(this);
            Subscribe(logic);
        }

        public void Subscribe(IObservable<IEnumerable<KulkaModel>> provider)//generated
        {
            unsubscriber = provider.Subscribe(this);
        }

        public void Unsubscribe()
        {
            unsubscriber?.Dispose();//generated
        }


        public void SimStart()
        {
            getSetFlag = true;
            logic.GenerowanieKul(LiczbaKulek);
            logic.Start();
        }

        public void SimStop()
        {
            getSetFlag = false;
            logic.Stop();
        }

        //wymagane przez visual
        public void OnCompleted()
        {
            Unsubscribe();
        }

        public void OnError(Exception error)
        {
            throw error;
        }
        public void OnNext(IEnumerable<KulkaModel> kulki)
        {
            if (kulki == null)
            {
                kulki = new List<KulkaModel>();
            }
            this.kulki = new ObservableCollection<KulkaModel>(kulki);
            OnPropertyChanged(nameof(Kulki));
        }
    }
}