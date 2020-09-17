// Player object for Pandemic
// Image for player pawn used with free personal licence from https://www.pinclipart.com/pindetail/iomwR_monopoly-board-game-clipart-cliparthut-free-clipart-board/
// John Ryder 219466419

using System;
using System.Collections.Generic;
using SplashKitSDK;

namespace Pandemic {
    // public enums for the possible player types
    public enum playerType {
        Researcher, Dispatcher, Generalist, Medic
    }
    public abstract class Player {
        // Variables
        private List<PlayerCard> _cardsInHand;
        private City _location;
        private playerType _type;
        private double _xloc;
        private double _yloc;
        protected Bitmap _pawn;
        protected int _turns = 4;
        public City location { get { return _location; } }
        public List<PlayerCard> cardsInHand { get { return _cardsInHand; } }
        public playerType type { get { return _type;} }
        public int turns { get { return _turns; } }
        public double xloc {
            get { return _xloc; }
            set { _xloc = value; }
        }
        public double yloc {
            get { return _yloc; }
            set { _yloc = value; }
        }
        public string typeToString { 
            get {
                switch(_type) {
                    case playerType.Researcher:
                        return "Researcher";
                    case playerType.Dispatcher:
                        return "Dispatcher";
                    case playerType.Generalist:
                        return "Generalist";
                    case playerType.Medic:
                        return "Medic";
                    // default needed to silence compiler warnings
                    default:
                        return "Player";
                }
            }
        }
        public Bitmap pawn { get { return _pawn; } }

        // Constructor
        public Player(playerType type) {
            _type = type;
            _cardsInHand = new List<PlayerCard>();
        }

        // Public Methods
        public virtual void TreatInfection() {
            location.decreaseInfection();
        }
        
         // you need 4 of a colour cards, and to be in a city with a base
        public virtual void DiscoverCure() {
            if(location.hasBase) {
                // see if you have 4 of a colour of card
                int blue = 0;
                int red = 0;
                int yellow = 0;
                foreach(PlayerCard card in cardsInHand) {
                    switch(card.group) {
                        case CityGroup.blue:
                            blue++;
                            break;
                        case CityGroup.red:
                            red++;
                            break;
                        case CityGroup.yellow:
                            yellow++;
                            break;
                    }
                }
                
                if(blue >= 4) {
                    removeFourCards(CityGroup.blue);
                    //board.cure(CityGroup.blue);
                } else if(red >= 4) {
                    removeFourCards(CityGroup.red);
                } else if(yellow >= 4) {
                    removeFourCards(CityGroup.yellow);
                }

            } else {
                Console.WriteLine("No base");
            }
        }
        //public abstract void Build();
        
        public void Move(City toCity) {
            _location = toCity;
        }

        public void AddCardToHand(PlayerCard card) {
            _cardsInHand.Add(card);
            // while(_cardsInHand.Count > 7) {
            //     DiscardCard();
            // }
        }

        public void DiscardCard(PlayerCard card) {
            foreach(PlayerCard cardInHand in _cardsInHand) {
                if(cardInHand == card) {
                    _cardsInHand.Remove(cardInHand);
                    return;
                }
            }
        }

        // Private Methods
       // Private Methods
        // remove 4 of a colour of card from the hand
        private void removeFourCards(CityGroup cardColour) {
            int removedCount = 0;
            foreach(PlayerCard card in cardsInHand) {
                if(card.group == cardColour) {
                    cardsInHand.Remove(card);
                    removedCount++;
                    if(removedCount >= 4) {
                        return;
                    }
                }
            }
        }
    }
}