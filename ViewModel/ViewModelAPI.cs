using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using Model;

namespace ViewModel
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    public class MainViewModel : ViewModelBase
    {
        public ModelApiBase model;

        public ObservableCollection<IKulaModel> Balls => model.Balls;

        public double PlaneWidth
        {
            get => model.ScenaWidth;

            set
            {
                model.ScenaWidth = value;
                OnPropertyChanged(nameof(PlaneWidth));
            }
        }
        public double PlaneHeight
        {
            get => model.ScenaHeight;

            set
            {
                model.ScenaHeight = value;
                OnPropertyChanged(nameof(PlaneHeight));
            }
        }

        private uint _ballsNum;
        public uint BallsNumber
        {
            get
            {
                return _ballsNum;
            }
            set
            {
                _ballsNum = value;
                OnPropertyChanged(nameof(BallsNumber));
            }
        }

        private uint _maxBallsNum;
        public uint MaxBallsNumber
        {
            get
            {
                return _maxBallsNum;
            }
            private set
            {
                if (_maxBallsNum != value)
                {
                    _maxBallsNum = value;
                    OnPropertyChanged(nameof(MaxBallsNumber));
                }
            }
        }

        public static uint MaxBallRadius => 50;
        public static uint MinBallRadius => 5;

        public static double MinBallVel => 10;
        public static double MaxBallVel => 100;

        public ICommand GenerateBallsCommand { get; private set; }
        public ICommand SimStopCommand { get; private set; }

        public MainViewModel()
        {
            this.BallsNumber = 0;
            this.MaxBallsNumber = 0;
            this.GenerateBallsCommand = new GenerateBallsCommand(this);
            this.SimStopCommand = new SimStopCommand(this);

            this.model = ModelApiBase.GetApi();
            this.PropertyChanged += RecalculateMaxBallsNumber;
        }

        void RecalculateMaxBallsNumber(object? source, PropertyChangedEventArgs? e)
        {
            if (e?.PropertyName == nameof(PlaneWidth) || e?.PropertyName == nameof(PlaneHeight))
            {
                uint ballsInHeight = (uint)(PlaneHeight / (MaxBallRadius * 2));
                uint ballsInWidth = (uint)(PlaneWidth / (MaxBallRadius * 2));

                uint ballsNumber = ballsInHeight * ballsInWidth;
                MaxBallsNumber = ballsNumber >= 40 ? ballsNumber - 40 : 0;
            }
        }
    }
}