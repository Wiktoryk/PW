using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Logika
{
    public class Kula : INotifyPropertyChanged
    {
        private double x;
        private double y;
        private double promien;

        public Kula(double x, double y, double promien)
        {
            this.x = x;
            this.y = y;
            this.promien = promien;
        }

        public double X //property
        {
            get { return x; }
            set
            {
                x = value;
                OnPropertyChanged("X");
            }
        }

        public double Y
        {
            get { return y; }
            set
            {
                y = value;
                OnPropertyChanged("Y");
            }
        }

        public double Promien
        {
            get { return promien; }
            set
            {
                promien = value;
                OnPropertyChanged("Radius");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
