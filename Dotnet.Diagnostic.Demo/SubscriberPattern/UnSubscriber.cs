using System;
using System.Collections.Generic;

namespace Dotnet.Diagnostic.Demo.SubscriberPattern
{
    internal class UnSubscriber<T> : IDisposable
    {
        private List<IObserver<T>> _observers;
        private IObserver<T> _observer;

        internal UnSubscriber(List<IObserver<T>> observers, IObserver<T> observer)
        {
            this._observers = observers;
            this._observer = observer;
        }

        public void Dispose()
        {
            if (_observers.Contains(_observer))
            {
                Console.WriteLine("Unsubscribed!");
                _observers.Remove(_observer);
            }
        }
    }
}