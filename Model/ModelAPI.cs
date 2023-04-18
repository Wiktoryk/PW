using Logika;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection.PortableExecutable;

namespace Model
{
    public abstract class ApiModel : IObserver<IEnumerable<Kula>>, IObservable<IEnumerable<KulkaModel>>
    {
        public abstract void GenerowanieKul(int liczba_kulek);
        public abstract void Start();
        public abstract void Stop();


        public abstract void OnCompleted();
        public abstract void OnError(Exception error);
        public abstract void OnNext(IEnumerable<Kula> val);
        public abstract IDisposable Subscribe(IObserver<IEnumerable<KulkaModel>> obs);

        public static ApiModel StworzModelApi(LogikaAbstractApi? logika = default)
        {
            return new Model(logika ?? LogikaAbstractApi.StworzLogikaApi());
        }
    }
    public class Model : ApiModel
    {
        private readonly ISet<IObserver<IEnumerable<KulkaModel>>> observers;
        private IDisposable? unsubscriber;
        private readonly LogikaAbstractApi logika;      //checklist1

        public Model(LogikaAbstractApi? logika = default)
        {
            this.logika = logika ?? LogikaAbstractApi.StworzLogikaApi();
            observers = new HashSet<IObserver<IEnumerable<KulkaModel>>>();
            Subscribe(logika);
        }


        public override void GenerowanieKul(int liczba_kulek)
        {
            logika.GenerowanieKul(liczba_kulek);
        }

        public override void Start()
        {
            logika.StartSim();
        }

        public override void Stop()
        {
            logika.StopSim();
        }

        public static IEnumerable<KulkaModel> m(IEnumerable<Kula> kulki)
        {
            return kulki.Select(kulka => new KulkaModel(kulka));
        }

        public void Subscribe(IObservable<IEnumerable<Kula>> p)
        {
            unsubscriber = p.Subscribe(this);
        }

        public override void OnCompleted()
        {
            Unsubscribe();
            EndTransmission();
        }

        public override void OnError(Exception error)
        {
            throw error;
        }
        public override void OnNext(IEnumerable<Kula> kulki)
        {
            SledzKulki(m(kulki));
        }

        public void Unsubscribe()
        {
            unsubscriber?.Dispose();
        }
        public override IDisposable Subscribe(IObserver<IEnumerable<KulkaModel>> obs)
        {
            if (!observers.Contains(obs))
            {
                observers.Add(obs);
            }
            return new Unsubscriber(observers, obs);
        }

        private class Unsubscriber : IDisposable
        {
            private readonly ISet<IObserver<IEnumerable<KulkaModel>>> observers;
            private readonly IObserver<IEnumerable<KulkaModel>> observer;

            public Unsubscriber(ISet<IObserver<IEnumerable<KulkaModel>>> observers, IObserver<IEnumerable<KulkaModel>> observer)
            {
                this.observers = observers;
                this.observer = observer;
            }
            //musi byc
            public void Dispose()
            {
                if (observer != null)
                {
                    observers.Remove(observer);
                }
            }

        }
        public void SledzKulki(IEnumerable<KulkaModel> kulki)
        {
            foreach (var observer in this.observers)
            {
                if (kulki == null)
                {
                    observer.OnError(new NullReferenceException("Obiekt kulka jest null!"));
                }
                else
                {
                    observer.OnNext(kulki);
                }
            }
        }

        public void EndTransmission()
        {
            foreach (var observer in observers)
            {
                observer.OnCompleted();
            }

            observers.Clear();
        }
    }
}