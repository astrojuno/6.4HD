// Dispatch player class for the Pandemic game
// John Ryder 219466419

using SplashKitSDK; 

namespace Pandemic {
    public class DispatchPlayer : Player {
        // This player class has two special move abilities that are handled in Pandemic.cs
        public DispatchPlayer() : base(playerType.Dispatcher) {
            base._pawn = SplashKit.BitmapNamed("dispatcher");
        }
    }
}