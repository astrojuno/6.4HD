// City object for Pandemic
// John Ryder 219466419

using System.Collections.Generic;
using SplashKitSDK;
using System;

namespace Pandemic {
    // The groupings of the cities
    public enum CityGroup {
        red, blue, yellow, epidemic
    }
    public class City : IObservable<City> {
        // Variables
        private List<IObserver<City>> _observers;
        private string _name;
        private int _infectionLevel;
        private bool _canBeOutbroken;
        private CityGroup _group;
        private List<City> _connectedCities;
        private bool _hasBase;
        private Rectangle _cityLoc;
        private Point2D _cityCentre;
        public int infectionLevel { get { return _infectionLevel; } }
        public string name { get { return _name;} }
        public bool hasBase { get { return _hasBase; } }
        public List<City> connectedCities { get { return _connectedCities; } }
        public Rectangle boardLocation { get { return _cityLoc;} }
        public Point2D cityCentre { get { return _cityCentre; } }
        public CityGroup type { get { return _group; } }
        public Color cityColour {
            get {
                switch(_group) {
                    case CityGroup.blue:
                        return Color.Blue;
                    case CityGroup.red:
                        return Color.Red;
                    case CityGroup.yellow:
                        return Color.Yellow;
                    // needed to silence compiler warnings
                    default: 
                        return Color.Blue;
                }
            }
        }

        // Constructor
        public City(string name, CityGroup group) {
            _name = name;
            _group = group;
            _canBeOutbroken = true;
            _infectionLevel = 0;
            _connectedCities = new List<City>();
            _hasBase = false;
            _observers = new List<IObserver<City>>();
        }

        // Public Methods
        public void increaseInfection() {
            // to catch runaway outbreaks
            if(_canBeOutbroken) {
                _infectionLevel += 1;
                if(infectionLevel > 3) {
                    outbreak();
                    _infectionLevel = 3;
                }
            }
        }

        public void decreaseInfection(int numberToDecreaseBy) {
            _infectionLevel -= numberToDecreaseBy;
            if(_infectionLevel < 0) {
                _infectionLevel = 0;
            }
        }

        public void addConnectedCity(City cityToAdd) {
            _connectedCities.Add(cityToAdd);
        }

        public void setBoardLocation(Rectangle boardLocation, Point2D cityCentre) {
            _cityLoc = boardLocation;
            _cityCentre = cityCentre;
        }

        // Subscriber Methods
        public IDisposable Subscribe(IObserver<City> observer) {
            if (!_observers.Contains(observer)) {
                _observers.Add(observer);
            }
            return new Unsubscriber<City>(_observers, observer);
        }
        
        // Private Methods
        private void outbreak() {
            _canBeOutbroken = false;
            foreach(IObserver<City> observer in _observers) {
                observer.OnNext(this);
            }
            foreach(City city in _connectedCities) {
                city.increaseInfection();
            }
            _canBeOutbroken = true;
        }

    }
}