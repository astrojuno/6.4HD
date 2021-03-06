// Generalist player type for the game Pandemic 
// John Ryder 219466419

using SplashKitSDK;

namespace Pandemic {
    public class GeneralistPlayer : Player {
        // Constructor
        public GeneralistPlayer() : base(playerType.Generalist) {
            base._pawn = SplashKit.BitmapNamed("generalist");
            // generalist player gets 5 turns instead of 4
            base._turns = 5;
        }
    }
}