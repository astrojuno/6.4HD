// Player object for Pandemic
// Image for player pawn used with free personal licence from https://www.pinclipart.com/pindetail/iomwR_monopoly-board-game-clipart-cliparthut-free-clipart-board/
// John Ryder 219466419

using System.Collections.Generic;
using SplashKitSDK;

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
        protected Bitmap _pawn;

        public City location { get { return _location; } }
        public List<PlayerCard> cardsInHand { get { return _cardsInHand; } }
        public playerType type { get { return _type;} }
        public Bitmap pawn { get { return _pawn; } }

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