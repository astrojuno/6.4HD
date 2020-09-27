// Dispatch player class for the Pandemic game
// John Ryder 219466419

using SplashKitSDK; 

namespace Pandemic {
    public class DispatchPlayer : Player {
        public DispatchPlayer() : base(playerType.Dispatcher) {
            base._pawn = SplashKit.BitmapNamed("dispatcher");
        }
    }
}