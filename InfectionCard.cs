// Infection card for Pandemic
// John Ryder 219466419

using SplashKitSDK;

namespace Pandemic {
    public class InfectionCard : Card {
        // Constructor
        public InfectionCard(string city, CityGroup group) : base(city, group) {
            loadCardImages();
        }

        // Public Methods
        public override void loadCardImages() {
            SplashKit.LoadBitmap("InfectionFront"+city, "InfectionCardFront.png");
            _frontOfCard = SplashKit.BitmapNamed("InfectionFront"+city);
            Rectangle rect = _frontOfCard.BoundingRectangle();
            int textWidth = Text.Width(city, "roboto", FONT_SIZE);
            int textHeight = Text.Height(city, "roboto", FONT_SIZE);
            double textX = (rect.Width - textWidth) / 2;
            double textY = (rect.Height - textHeight) / 2;

            _frontOfCard.DrawText(this.city, Color.Black, "roboto", FONT_SIZE, textX, textY);
            _backOfCard = SplashKit.BitmapNamed("InfectionBack");
        }
    }
}