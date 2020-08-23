// Player object for Pandemic
// John Ryder 219466419

using System.Collections.Generic;

namespace Pandemic {
    // public enums for the possible player types
    public enum playerType {
        Researcher, QuarantineSpecialist, OperationsExpert, Medic
    }
    public abstract class Player {
        // Variables
        private List<PlayerCard> _cardsInHand;
        private City _location;
        private playerType _type;

        public City location { get { return _location; } }
        public List<PlayerCard> cardsInHand { get { return _cardsInHand; } }
        public playerType type { get { return _type;} }

        // Constructor
        public Player(playerType type) {
            _type = type;
            _cardsInHand = new List<PlayerCard>();
        }

        // Public Methods
        public abstract void TreatInfection();
        public abstract void DiscoverCure();
        public abstract void Build();
        
        public void Move(City toCity) {
            _location = toCity;
        }

        public void AddCardToHand(PlayerCard card) {
            _cardsInHand.Add(card);
            while(_cardsInHand.Count > 7) {
                DiscardCard();
            }
        }

        // Private Methods
        private void DiscardCard() {
            
        }
    }
}