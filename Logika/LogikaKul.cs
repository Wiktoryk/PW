using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Logika
{
    public class LogikaKul : INotifyPropertyChanged
    {
        private Kula kula;

        public LogikaKul(Kula kula)
        {
            this.kula = kula;
            kula.PropertyChanged += Update;
        }

        private void Update(object source, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "X")
            {
                OnPropertyChanged("X");
            }
            else if (e.PropertyName == "Y")
            {
                OnPropertyChanged("Y");
            }
            else if (e.PropertyName == "Radius")
            {
                OnPropertyChanged("Radius");
            }

        }
        public double X
        {
            get { return kula.X; }
            set
            {
                kula.X = value;
                OnPropertyChanged("X");
            }
        }
        public double Y
        {
            get { return kula.Y; }
            set
            {
                kula.Y = value;
                OnPropertyChanged("Y");
            }
        }
        public double Radius
        {
            get { return kula.Promien; }
            set
            {
                kula.Promien = value;
                OnPropertyChanged("Radius");
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
