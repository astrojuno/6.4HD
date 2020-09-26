// Researcher player for Pandemic
// John Ryder 219466419

using SplashKitSDK;

namespace Pandemic {
    // researcher has the ability to give any card when in a city, not just the city they
    // occupy. This is handled in the game, not here, so there isn't much to see here...
    public class ResearcherPlayer : Player {
        public ResearcherPlayer() : base(playerType.Researcher) {
            base._pawn = SplashKit.BitmapNamed("researcher");
        }
    }
}