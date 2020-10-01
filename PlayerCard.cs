// Player card for Pandemic
// John Ryder 219466419

using SplashKitSDK;

namespace Pandemic {
    public class PlayerCard : Card {
        // Constructor
        public PlayerCard(string city, CityGroup group) : base(city, group) {
            loadCardImages();
        }

        // Public Methods
        public override void loadCardImages() {
            switch(base._group) {
                case CityGroup.blue:
                    SplashKit.LoadBitmap("BluePlayerFront"+city, "BluePlayerCardFront.png");
                    _frontOfCard = SplashKit.BitmapNamed("BluePlayerFront"+city);
                    break;
                case CityGroup.red:
                    SplashKit.LoadBitmap("RedPlayerFront"+city, "RedPlayerCardFront.png");
                    _frontOfCard = SplashKit.BitmapNamed("RedPlayerFront"+city);
                    break;
                case CityGroup.yellow:
                    SplashKit.LoadBitmap("YellowPlayerFront"+city, "YellowPlayerCardFront.png");
                    _frontOfCard = SplashKit.BitmapNamed("YellowPlayerFront"+city);
                    break;
            }
            
            Rectangle rect = _frontOfCard.BoundingRectangle();
            int textWidth = Text.Width(city, "roboto", FONT_SIZE);
            int textHeight = Text.Height(city, "roboto", FONT_SIZE);
            double textX = (rect.Width - textWidth) / 2;
            double textY = (rect.Height - textHeight) / 2;

            _frontOfCard.DrawText(this.city, Color.Black, "roboto", FONT_SIZE, textX, textY);
            _backOfCard = SplashKit.BitmapNamed("PlayerBack");
        }
    }
}