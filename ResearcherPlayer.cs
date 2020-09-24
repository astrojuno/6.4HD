// Researcher player for Pandemic
// John Ryder 219466419

using SplashKitSDK;

namespace Pandemic {
    public class ResearcherPlayer : Player {
        public ResearcherPlayer() : base(playerType.Researcher) {
            base._pawn = SplashKit.BitmapNamed("researcher");
        }
    }
}