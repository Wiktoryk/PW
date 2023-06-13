using Dane;
using Logika;
using System;
using System.ComponentModel;
using System.Windows.Media;

namespace Model
{
    public class KulaModel : IKulaModel
    {
        private readonly IKula ball;

        public double Diameter
        {
            get
            {
                return ball.GetPromien() * 2;
            }
        }

        public Pozycja CanvasPos
        {
            get
            {
                return ball.GetPoz() - (Pozycja.One * ball.GetPromien());
            }
            set
            {
                double radius = ball.GetPromien();
                Pozycja canvasPos = ball.GetPoz() - (Pozycja.One * radius);
                if (canvasPos.X != value.X || canvasPos.Y != value.Y)
                {
                    ball.SetPoz(value + (Pozycja.One * radius));
                }
            }
        }


        public KulaModel(IKula ball)
        {
            this.ball = ball;
            this.ball.OnPositionChanged += BallPositionUpdate;
        }

        private void BallPositionUpdate(object sender, PositionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(CanvasPos));
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Dispose()
        {
            ball.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}