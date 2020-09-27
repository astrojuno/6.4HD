// Generalist player type for the game Pandemic 
// John Ryder 219466419

using SplashKitSDK;

namespace Pandemic {
    public class GeneralistPlayer : Player {
        // Constructor
        public GeneralistPlayer() : base(playerType.Generalist) {
            base._pawn = SplashKit.BitmapNamed("generalist");
            base._turns = 5;
        }
    }
}