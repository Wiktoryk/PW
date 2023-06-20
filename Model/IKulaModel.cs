using System;
using System.ComponentModel;
using System.Windows.Media;
using Dane;

namespace Model
{
    public interface IKulaModel : INotifyPropertyChanged, IDisposable
    {
        double Diameter { get; }

        Pozycja ScenaPoz { get; set; }
    }
}