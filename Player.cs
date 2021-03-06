// Player object for Pandemic
// Image for player pawn used with free personal licence from https://www.pinclipart.com/pindetail/iomwR_monopoly-board-game-clipart-cliparthut-free-clipart-board/
// John Ryder 219466419

using System.Collections.Generic;
using SplashKitSDK;

namespace Pandemic {
    // public enums for the possible player types
    public enum playerType {
        Researcher, Dispatcher, Generalist, Medic
    }
    public abstract class Player {
        // Variables
        private List<Card> _cardsInHand;
        private City _location;
        private playerType _type;
        private double _xloc;
        private double _yloc;
        protected Bitmap _pawn;
        protected int _turns = 4;
        public City location { get { return _location; } }
        public List<Card> cardsInHand { get { return _cardsInHand; } }
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
        public Bitmap pawn { get { return _pawn; } }

        // Constructor
        public Player(playerType type) {
            _type = type;
            _cardsInHand = new List<Card>();
        }

        // Public Methods
        // treats the infection of the city the player is in
        // overridden by medic
        public virtual void TreatInfection(bool diseaseIsCured) {
            if(diseaseIsCured) {
                location.decreaseInfection(3);
            } else {
                location.decreaseInfection(1);
            }
            
        }
        
         // you need 4 of a colour cards, and to be in a city with a base
        public List<CityGroup> CanDiscoverCure() {
            List<CityGroup> curableOptions = new List<CityGroup>();
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
                    curableOptions.Add(CityGroup.blue);
                } 
                if(red >= 4) {
                    curableOptions.Add(CityGroup.red);
                }
                if(yellow >= 4) {
                    curableOptions.Add(CityGroup.yellow);
                }

            } 
            return curableOptions;
        }
        
        // updates the players location
        public void Move(City toCity) {
            _location = toCity;
        }

        // adds a card to the player hand
        public void AddCardToHand(PlayerCard card) {
            _cardsInHand.Add(card);
        }

        // discards a player card from hand
        public void DiscardCard(PlayerCard card) {
            foreach(PlayerCard cardInHand in _cardsInHand) {
                if(cardInHand == card) {
                    _cardsInHand.Remove(cardInHand);
                    return;
                }
            }
        }

        // remove 4 of a colour of card from the hand
        public List<PlayerCard> removeFourCardsToCureDisease(CityGroup cardColour) {
            int removedCount = 0;
            List<PlayerCard> returnList = new List<PlayerCard>();
            foreach(PlayerCard card in cardsInHand) {
                if(card.group == cardColour) {
                    cardsInHand.Remove(card);
                    returnList.Add(card);
                    removedCount++;
                    if(removedCount >= 4) {
                        return returnList;
                    }
                }
            }
            return returnList;
        }
    }
}