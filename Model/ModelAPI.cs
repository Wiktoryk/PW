using System;
using Logika;
using Dane;
using System.Collections.ObjectModel;
using System.Windows.Media;

namespace Model
{
    public abstract class ModelApiBase : IDisposable
    {
        public abstract ObservableCollection<IKulaModel> Balls { get; }
        public abstract double ScenaWidth { get; set; }
        public abstract double ScenaHeight { get; set; }

        public abstract void StworzKule(uint ballsNum, double minMass, double maxMass, double minRadius, double maxRadius, double minVel, double maxVel);

        public abstract void Start();
        public abstract void Stop();

        public abstract void Dispose();

        public static ModelApiBase GetApi(LogikaApiBase? logika = default)
        {
            return new ModelApi(logika ?? LogikaApiBase.GetApi());
        }
    }
    internal class ModelApi : ModelApiBase
    {
        private ObservableCollection<IKulaModel> _balls;
        public override ObservableCollection<IKulaModel> Balls => _balls;
        public override double ScenaWidth { get => logika.ScenaWidth; set => logika.ScenaWidth = value; }
        public override double ScenaHeight { get => logika.ScenaHeight; set => logika.ScenaHeight = value; }

        private readonly LogikaApiBase logika;

        public ModelApi(LogikaApiBase? logika = default)
        {
            this.logika = logika ?? LogikaApiBase.GetApi();
            this._balls = new ObservableCollection<IKulaModel>();
        }

        public override void StworzKule(uint ballsNum, double minMass, double maxMass, double minRadius, double maxRadius, double minVel, double maxVel)
        {
            this.logika.StworzKule(ballsNum, minMass, maxMass, minRadius, maxRadius, minVel, maxVel);
            this._balls = new ObservableCollection<IKulaModel>();
            foreach (IKula ball in this.logika.Balls)
            {
                this._balls.Add(new KulaModel(ball));//, color));
            }
        }

        public override void Start()
        {
            this.logika.StartSimulation();
        }

        public override void Stop()
        {
            this.logika.StopSimulation();
        }

        public override void Dispose()
        {
            logika.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}