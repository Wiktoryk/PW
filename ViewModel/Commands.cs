using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.ComponentModel;

namespace ViewModel
{
    public abstract class CommandBase : ICommand
    {
        public event EventHandler? CanExecuteChanged;

        public virtual bool CanExecute(object? parameter) => true;

        public abstract void Execute(object? parameter);

        protected void OnCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
    public class SimStartCommand : CommandBase
    {
        private readonly ViewModelSim viewModelSim;

        public SimStartCommand(ViewModelSim viewModelSim) : base()
        {
            this.viewModelSim = viewModelSim;

            this.viewModelSim.PropertyChanged += OnSimViewModelPropertyChanged;
        }

        public override bool CanExecute(object? parameter)
        {
            return base.CanExecute(parameter)
                && !viewModelSim.getSetFlag;
        }


        public override void Execute(object? parameter)
        {
            viewModelSim.SimStart();
        }

        private void OnSimViewModelPropertyChanged(object? sn, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ViewModelSim.getSetFlag))
            {
                OnCanExecuteChanged();
            }
        }
    }
    public class SimStopCommand : CommandBase
    {
        private readonly ViewModelSim viewModelSim;

        public SimStopCommand(ViewModelSim viewModelSim) : base()
        {
            this.viewModelSim = viewModelSim;

            this.viewModelSim.PropertyChanged += OnSimViewModelPropertyChanged;
        }

        public override bool CanExecute(object? parameter)
        {
            return base.CanExecute(parameter)
                && viewModelSim.getSetFlag;
        }


        public override void Execute(object? parameter)
        {
            viewModelSim.SimStop();
        }

        private void OnSimViewModelPropertyChanged(object? sn, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(viewModelSim.getSetFlag))
            {
                OnCanExecuteChanged();
            }
        }
    }
}
