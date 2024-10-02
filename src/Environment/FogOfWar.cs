// src/Environment/FogOfWar.cs
using System;
using System.Collections.Generic;
using GridMind.Environment;

namespace GridMind.Environment
{
    public class FogOfWar : IObservable<GridCell>
    {
        private readonly List<IObserver<GridCell>> observers;

        public FogOfWar()
        {
            observers = new List<IObserver<GridCell>>();
        }

        public IDisposable Subscribe(IObserver<GridCell> observer)
        {
            if (!observers.Contains(observer))
                observers.Add(observer);

            return new Unsubscriber(observers, observer);
        }

        public void CellExplored(GridCell cell)
        {
            foreach (var observer in observers)
            {
                observer.OnNext(cell);
            }
        }

        private class Unsubscriber : IDisposable
        {
            private List<IObserver<GridCell>> _observers;
            private IObserver<GridCell> _observer;

            public Unsubscriber(List<IObserver<GridCell>> observers, IObserver<GridCell> observer)
            {
                this._observers = observers;
                this._observer = observer;
            }

            public void Dispose()
            {
                if (_observers.Contains(_observer))
                    _observers.Remove(_observer);
            }
        }
    }
}