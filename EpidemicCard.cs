// Epidemic type of card in the Pandemic game
// John Ryder 219466419

using SplashKitSDK;

namespace Pandemic {
    class EpidemicCard : Card {
        private int _cardNumber;
        public EpidemicCard(int epidemicCardNumber) : base("EPIDEMIC!", CityGroup.epidemic) {
            _cardNumber = epidemicCardNumber;
            loadCardImages();
        }

        public override void loadCardImages() {
            SplashKit.LoadBitmap("InfectionFront"+_cardNumber, "InfectionCardFront.png");
            _frontOfCard = SplashKit.BitmapNamed("InfectionFront"+_cardNumber);
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