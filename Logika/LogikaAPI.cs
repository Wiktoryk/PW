using System;
using System.Collections.Generic;
using Dane;

namespace Logika
{
    public abstract class LogikaApiBase : IDisposable
    {
        public abstract IEnumerable<IKula> Balls { get; }
        public abstract double ScenaWidth { get; set; }
        public abstract double ScenaHeight { get; set; }

        public abstract void StworzKule(uint ballsNum, double minMass, double maxMass, double minRadius, double maxRadius, double minVel, double maxVel);
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
        public override IEnumerable<IKula> Balls => simManager.Balls;
        public override double ScenaWidth { get => simManager.ScenaWidth; set => simManager.ScenaWidth = value; }
        public override double ScenaHeight { get => simManager.ScenaHeight; set => simManager.ScenaHeight = value; }

        private readonly SimulationManager simManager;
        private readonly DaneApiBase dane;

        public LogikaApi(DaneApiBase? dane)
        {
            this.dane = dane ?? DaneApiBase.GetApi();
            this.simManager = new SimulationManager(this.dane.StworzScene(0, 0));
        }

        public override void StworzKule(uint ballsNum, double minMass, double maxMass, double minRadius, double maxRadius, double minVel, double maxVel)
        {
            BallLogger.Log("LogikaApi: Generating Random Balls", LogType.DEBUG);
            simManager.ClearBalls();
            simManager.StworzKule(ballsNum, minMass, maxMass, minRadius, maxRadius, minVel, maxVel);
            BallLogger.Log("LogikaApi: Generated Random Balls", LogType.DEBUG);
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
