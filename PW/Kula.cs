using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Dane
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

        public void rusz(Scena scena)
        {
            Random random = new Random();
            this.X = random.Next(1, scena.Szerokosc);
            this.Y = random.Next(1, scena.Wysokosc);
            OnPropertyChanged("Position");
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
