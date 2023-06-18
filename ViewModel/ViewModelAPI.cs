using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Model;
using Dane;

namespace ViewModel
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        public Thread mainThread;
        public event PropertyChangedEventHandler? PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            try
            {
                Dispatcher.FromThread(mainThread).Invoke(new Action(() => { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)); }));
            }
            catch
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        public ViewModelBase()
        {
            mainThread = Thread.CurrentThread;
        }
    }
    public class MainViewModel : ViewModelBase
    {
        public ModelApiBase model;

        public ObservableCollection<IKulaModel> Balls => model.Balls;
        private object ballsLock = new();

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
            BallLogger.SetMaxLogFileSizeKB(1024);
            BallLogger.SetMaxLogFilesNum(25);
            BallLogger.StartLogging();
            this.BallsNumber = 0;
            this.MaxBallsNumber = 0;
            this.GenerateBallsCommand = new GenerateBallsCommand(this);
            this.SimStopCommand = new SimStopCommand(this);

            this.model = ModelApiBase.GetApi();
            this.PropertyChanged += RecalculateMaxBallsNumber;
        }
        private async Task<bool> Generate(object? parameter)
        {
            return await Task.Run(() =>
            {
                lock (ballsLock)
                {
                    model.GenerateBalls(BallsNumber, MinBallVel, MaxBallVel);
                    model.Start();
                    OnPropertyChanged(nameof(Balls));
                }
                return true;
            });
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