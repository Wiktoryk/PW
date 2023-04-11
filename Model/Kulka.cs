using Logika;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Model
{
    public class Kulka : INotifyPropertyChanged
    {
        private double x;
        private double y;
        private double promien;

        public Kulka(Kula kula)
        {
            this.x = kula.X;
            this.y = kula.Y;
            this.promien = kula.Promien;
            kula.PropertyChanged += Update;
        }

        private void Update(object source, PropertyChangedEventArgs key)
        {
            Kula sourceKula = (Kula)source;
            if (key.PropertyName == "X")
            {
                this.x = sourceKula.X - sourceKula.Promien;
            }
            if (key.PropertyName == "Y")
            {
                this.y = sourceKula.Y - sourceKula.Promien;
            }
            if (key.PropertyName == "Radius")
            {
                this.promien = sourceKula.Promien;
            }

        }

        public double X
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