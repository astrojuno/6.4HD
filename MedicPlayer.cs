// Medic class of player for Pandemic 
// John Ryder 219466419

using System;
using SplashKitSDK;

namespace Pandemic {
    public class MedicPlayer : Player {
        // Constructor
        public MedicPlayer() : base(playerType.Medic) {
            base._pawn = SplashKit.BitmapNamed("medic");
        }

        // Public Methods
        // The medics special power is to remove all infections when treating infection
        public override void TreatInfection() {
            for(int i = 0; i < base.location.infectionLevel; i++) {
                base.location.decreaseInfection();
            }
        }

        // you need 4 of a colour cards, and to be in a city with a base
        public override void DiscoverCure() {
            if(location.hasBase) {
                // see if you have 4 of a colour of card
                int blue = 0;
                int red = 0;
                int yellow = 0;
                foreach(PlayerCard card in base.cardsInHand) {
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

        // you need the city card you are in to build there
        public override void Build() {
            foreach(PlayerCard card in base.cardsInHand) { 
                if(card.city == location.name) {
                    location.buildBase();
                    return;
                }
            }
            Console.WriteLine("Don't have the correct card");
        }

        // Private Methods
        // remove 4 of a colour of card from the hand
        private void removeFourCards(CityGroup cardColour) {
            int removedCount = 0;
            foreach(PlayerCard card in base.cardsInHand) {
                if(card.group == cardColour) {
                    base.cardsInHand.Remove(card);
                    removedCount++;
                    if(removedCount >= 4) {
                        return;
                    }
                }
            }
        }
    }
}