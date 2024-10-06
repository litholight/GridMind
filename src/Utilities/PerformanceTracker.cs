// src/Utilities/PerformanceTracker.cs
using System;
using System.Collections.Generic;

namespace GridMind.Utilities
{
    public class PerformanceTracker : IObservable<PerformanceMetrics>
    {
        private readonly List<IObserver<PerformanceMetrics>> observers =
            new List<IObserver<PerformanceMetrics>>();
        public PerformanceMetrics Metrics { get; }

        public PerformanceTracker(string agentName)
        {
            Metrics = new PerformanceMetrics(agentName);
        }

        public IDisposable Subscribe(IObserver<PerformanceMetrics> observer)
        {
            observers.Add(observer);
            return new Unsubscriber<PerformanceMetrics>(observers, observer);
        }

        public void Notify()
        {
            foreach (var observer in observers)
            {
                observer.OnNext(Metrics);
            }
        }

        // Helper method to notify observers when metrics are updated
        public void UpdateMetrics(Action<PerformanceMetrics> updateAction)
        {
            updateAction(Metrics);
            Notify();
        }
    }

    // Helper class to handle unsubscription
    public class Unsubscriber<T> : IDisposable
    {
        private readonly List<IObserver<T>> _observers;
        private readonly IObserver<T> _observer;

        public Unsubscriber(List<IObserver<T>> observers, IObserver<T> observer)
        {
            _observers = observers;
            _observer = observer;
        }

        public void Dispose()
        {
            if (_observers.Contains(_observer))
            {
                _observers.Remove(_observer);
            }
        }
    }
}
