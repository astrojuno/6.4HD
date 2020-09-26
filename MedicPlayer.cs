// Medic class of player for Pandemic 
// John Ryder 219466419

using SplashKitSDK;

namespace Pandemic {
    public class MedicPlayer : Player {
        // Constructor
        public MedicPlayer() : base(playerType.Medic) {
            base._pawn = SplashKit.BitmapNamed("medic");
        }

        // Public Methods
        // The medics special power is to remove all infections when treating infection
        public override void TreatInfection(bool diseaseIsCured) {
            base.location.decreaseInfection(3);
        }
    }
}