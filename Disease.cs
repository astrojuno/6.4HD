// Class for the diseases in the game Pandemic
// John Ryder 219466419

using System;

namespace Pandemic {
    public class Disease {
        private int _cubes = 16;
        private bool _cured = false;
        private CityGroup _type;
        public int cubes { get { return _cubes; } }
        public CityGroup type { get { return _type; } }
        public bool isCured { get { return _cured; } }

        public Disease(CityGroup type) {
            _type = type;
        }

        public void useCube(int numberOfCubes) {
            _cubes -= numberOfCubes;
        }

        public void returnCube(int numberOfCubes) {
            _cubes += numberOfCubes;
            // just to make sure cubes aren't manufactured anywhere
            if(_cubes > 16) {
                _cubes = 16;
            }
        }

        // a quick check to see if you have lost due to running out of cubes
        public bool hasLostGame() {
            return _cubes <= 0;
        }

        // cure the disease
        public void cureDisease() {
            _cured = true;
        }
    }
}