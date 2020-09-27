// Observer design for Pandemic game
// John Ryder 219466419

using System;
using System.Collections.Generic;

namespace Pandemic {
    // needed to unsubscribe before all the notifications have been sent
    internal class Unsubscriber<City> : IDisposable {
        private List<IObserver<City>> _observers;
        private IObserver<City> _observer;

        // Constructor
        internal Unsubscriber(List<IObserver<City>> observers, IObserver<City> observer) {
            _observers = observers;
            _observer = observer;
        }

        // Public Methods
        // used to remove the subscriber
        public void Dispose() {
            if(_observers.Contains(_observer)) {
                _observers.Remove(_observer);
            }
        }
    }
}