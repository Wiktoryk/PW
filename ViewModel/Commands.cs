using System;
using System.Windows.Input;
using System.ComponentModel;
using System.Windows;

namespace ViewModel
{
    internal abstract class CommandBase : ICommand
    {
        public event EventHandler? CanExecuteChanged;

        public virtual bool CanExecute(object? parameter)
        {
            return true;
        }

        public abstract void Execute(object? parameter);

        protected void OnCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, new EventArgs());
        }
    }
    internal class GenerateBallsCommand : CommandBase
    {
        private readonly MainViewModel m_mainView;

        public GenerateBallsCommand(MainViewModel mainView)
        {
            m_mainView = mainView;
            m_mainView.PropertyChanged += OnViewModelPropertyChanged;
        }

        public override bool CanExecute(object? parameter)
        {
            return m_mainView.BallsNumber > 0 && base.CanExecute(parameter) && m_mainView.BallsNumber <= m_mainView.MaxBallsNumber;
        }

        public override void Execute(object? parameter)
        {
            this.m_mainView.model.GenerateBalls(this.m_mainView.BallsNumber, MainViewModel.MinBallRadius, MainViewModel.MaxBallRadius, MainViewModel.MinBallVel, MainViewModel.MaxBallVel);
            this.m_mainView.OnPropertyChanged(nameof(this.m_mainView.Balls));
            MessageBox.Show("Generated " + m_mainView.BallsNumber + " Balls", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            this.m_mainView.model.Start();
        }

        private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(m_mainView.BallsNumber))
            {
                OnCanExecuteChanged();
            }
        }
    }
    internal class SimStopCommand : CommandBase
    {
        private readonly MainViewModel m_mainView;

        public SimStopCommand(MainViewModel mainView)
        {
            m_mainView = mainView;
            m_mainView.PropertyChanged += OnViewModelPropertyChanged;
        }

        public override bool CanExecute(object? parameter)
        {
            return m_mainView.BallsNumber > 0 && base.CanExecute(parameter) && m_mainView.BallsNumber <= m_mainView.MaxBallsNumber;
        }

        public override void Execute(object? parameter)
        {
            this.m_mainView.model.Stop();
        }

        private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(m_mainView.BallsNumber))
            {
                OnCanExecuteChanged();
            }
        }
    }
}
