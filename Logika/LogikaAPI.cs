using Dane;
using System;
using System.Collections.Generic;

namespace Logika
{
    public abstract class LogikaApiBase : IDisposable
    {
        public abstract IEnumerable<IKula> Balls { get; }
        public abstract double ScenaWidth { get; set; }
        public abstract double ScenaHeight { get; set; }

        public abstract void StworzKule(uint ballsNum, double minSzybkosc, double maxSzybkosc);
        public abstract void StartSimulation();
        public abstract void StopSimulation();

        public abstract void Dispose();

        public static LogikaApiBase GetApi(DaneApiBase? dane = null)
        {
            return new LogikaApi(dane ?? DaneApiBase.GetApi());
        }
    }
    internal class LogikaApi : LogikaApiBase
    {
        public override IEnumerable<IKula> Balls => simManager.Kule;
        public override double ScenaWidth { get => simManager.ScenaWidth; set => simManager.ScenaWidth = value; }
        public override double ScenaHeight { get => simManager.ScenaHeight; set => simManager.ScenaHeight = value; }

        private readonly SimulationManager simManager;
        private readonly DaneApiBase dane;

        public LogikaApi(DaneApiBase? dane)
        {
            this.dane = dane ?? DaneApiBase.GetApi();
            this.simManager = new SimulationManager(this.dane.StworzScene(0, 0));
        }

        public override void StworzKule(uint ballsNum, double minSzybkosc, double maxSzybkosc)
        {
            simManager.ClearBalls();
            simManager.StworzKule(ballsNum, minSzybkosc, maxSzybkosc);
        }

        public override void StartSimulation()
        {
            simManager.StartSimulation();
        }

        public override void StopSimulation()
        {
            simManager.StopSimulation();
        }

        public override void Dispose()
        {
            simManager.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
