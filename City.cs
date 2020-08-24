// City object for Pandemic
// John Ryder 219466419

using System.Collections.Generic;
using SplashKitSDK;

namespace Pandemic {
    // The groupings of the cities
    public enum CityGroup {
        red, blue, yellow
    }
    public class City {
        // Variables
        private string _name;
        private int _infectionLevel;
        private bool _canBeOutbroken;
        private CityGroup _group;
        private List<City> _connectedCities;
        private bool _hasBase;
        private Rectangle _cityLoc;
        public int infectionLevel { get { return _infectionLevel; } }
        public string name { get { return _name;} }
        public bool hasBase { get { return _hasBase; } }
        public List<City> connectedCities { get { return _connectedCities; } }
        public Rectangle boardLocation { get { return _cityLoc;} }

        // Constructor
        public City(string name, CityGroup group) {
            _name = name;
            _group = group;
            _canBeOutbroken = true;
            _infectionLevel = 0;
            _connectedCities = new List<City>();
            _hasBase = false;
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

        public void decreaseInfection() {
            _infectionLevel -= 1;
            if(_infectionLevel < 0) {
                _infectionLevel = 0;
            }
        }

        public void addConnectedCity(City cityToAdd) {
            _connectedCities.Add(cityToAdd);
        }

        public void buildBase() {
            _hasBase = true;
        }

        public void setBoardLocation(Rectangle boardLocation) {
            _cityLoc = boardLocation;
        }

        // Private Methods
        private void outbreak() {
            _canBeOutbroken = false;
            foreach(City city in _connectedCities) {
                increaseInfection();
            }
            _canBeOutbroken = true;
        }

    }
}