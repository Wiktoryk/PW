using System;
using System.ComponentModel;
using System.Threading;
using System.Windows.Threading;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Dane;
using Model;

namespace ViewModel
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        public Thread mainThread;

        public event PropertyChangedEventHandler? PropertyChanged;

        public ViewModelBase()
        {
            mainThread = Thread.CurrentThread;
        }

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
    }
    public class MainViewModel : ViewModelBase
    {
        public ModelApiBase model;

        public ObservableCollection<IKulaModel> Balls => model.Balls;
        private object ballsLock = new();

        public double ScenaWidth
        {
            get => model.ScenaWidth;

            set
            {
                model.ScenaWidth = value;
                OnPropertyChanged(nameof(ScenaWidth));
            }
        }
        public double ScenaHeight
        {
            get => model.ScenaHeight;

            set
            {
                model.ScenaHeight = value;
                OnPropertyChanged(nameof(ScenaHeight));
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

        private uint _currentMaxBallsNumber;
        public uint CurrentMaxBallsNumber
        {
            get
            {
                return _currentMaxBallsNumber;
            }
            set
            {
                if (value != _currentMaxBallsNumber)
                {
                    _currentMaxBallsNumber = value;
                    OnPropertyChanged(nameof(CurrentMaxBallsNumber));
                }
            }
        }
        public static uint MaxBallsNumber => 20;

        public static double MinBallMass => 10;
        public static double MaxBallMass => 10;

        public static double MaxBallRadius => 20;
        public static double CurrentMaxBallRadius { get; private set; }
        public static double MinBallRadius => 20;

        public static double MinBallVel => 10;
        public static double MaxBallVel => 100;

        public ICommand GenerateBallsCommand { get; private set; }

        public MainViewModel() : base()
        {
            BallLogger.SetMaxLogFileSizeKB(1024);
            BallLogger.SetMaxLogFilesNum(25);
            BallLogger.StartLogging();

            BallsNumber = 0;
            CurrentMaxBallsNumber = 0;
            GenerateBallsCommand = new SimpleCommand(this, Generate, (param) => { return BallsNumber > 0 && BallsNumber <= MaxBallsNumber; });
            ((SimpleCommand)GenerateBallsCommand).OnExecuteDone += (object source, CommandEventArgs e) =>
            {
                string message = new StringBuilder("Successfully generated ").Append(BallsNumber).Append(" balls").ToString();
                BallLogger.Log(message, LogType.INFO);
                MessageBox.Show(message, "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            };

            model = ModelApiBase.GetApi();
            PropertyChanged += RecalculateMaxBallsNumber;
        }

        private async Task<bool> Generate(object? parameter)
        {
            BallLogger.Log("Invoked Generate", LogType.DEBUG);
            return await Task.Run(() =>
            {
                lock (ballsLock)
                {
                    model.StworzKule(BallsNumber, MinBallMass, MaxBallMass, MinBallRadius, MaxBallRadius, MinBallVel, MaxBallVel);
                    model.Start();
                    OnPropertyChanged(nameof(Balls));
                }
                return true;
            });
        }

        void RecalculateMaxBallsNumber(object? source, PropertyChangedEventArgs? e)
        {
            if (e?.PropertyName == nameof(ScenaWidth) || e?.PropertyName == nameof(ScenaHeight))
            {
                double height = Math.Max(ScenaHeight - 2 * MinBallRadius, 0);
                double width = Math.Max(ScenaWidth - 2 * MinBallRadius, 0);

                double radius = Math.Sqrt((height * width) / (4 * (MaxBallsNumber + 40)));
                uint currentMaxNum = MaxBallsNumber;
                if (radius > MaxBallRadius) radius = MaxBallRadius;
                if (radius < MinBallRadius)
                {
                    if (radius < MinBallRadius) radius = MinBallRadius;

                    currentMaxNum = (uint)((height * width) / (4 * radius * radius));
                    currentMaxNum = currentMaxNum > 40 ? currentMaxNum - 40 : currentMaxNum;
                }
                CurrentMaxBallsNumber = currentMaxNum;
                CurrentMaxBallRadius = radius;
            }
        }
    }
}