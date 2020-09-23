// Abstract class for cards in Pandemic
// John Ryder 219466419

using SplashKitSDK;

namespace Pandemic {
    public abstract class Card {
        // Constant
        public const int FONT_SIZE = 12;

        // Variables
        private string _city;
        protected CityGroup _group;
        private bool _isFaceUp;
        private double _xLoc;
        private double _yLoc;
        protected Bitmap _backOfCard;
        protected Bitmap _frontOfCard;
        public CityGroup group { get { return _group; } }
        public double xLoc { 
            get { 
                return _xLoc; 
            }
            set {
                _xLoc = value;
            } 
        }
        public double yLoc { 
            get { 
                return _yLoc; 
            }
            set {
                _yLoc = value;
            } 
        }
        public string city { get { return _city; } }
        public bool isFaceUp { 
            get { 
                return _isFaceUp; 
            } 
            set {
                _isFaceUp = value;
            }
        } 
        public Bitmap cardImage { 
            get {
                if (_isFaceUp) {
                    return _frontOfCard;
                } else {
                    return _backOfCard;
                }
            }
        }

        // Constructor
        public Card(string city, CityGroup group) {
            _city = city;
            _isFaceUp = false;
            _group = group;
        }

        // public methods
        public abstract void loadCardImages();
    }
}