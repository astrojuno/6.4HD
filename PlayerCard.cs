// Player card for Pandemic
// John Ryder 219466419

using SplashKitSDK;

namespace Pandemic {
    public class PlayerCard : Card {
        // Variables
        public CityGroup group { get { return base._group; } }

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
                    //_frontOfCard = SplashKit.BitmapNamed("YellowPlayerFront");
                    break;
                case CityGroup.red:
                    SplashKit.LoadBitmap("RedPlayerFront"+city, "RedPlayerCardFront.png");
                    _frontOfCard = SplashKit.BitmapNamed("RedPlayerFront"+city);
                    //_frontOfCard = SplashKit.BitmapNamed("YellowPlayerFront");
                    break;
                case CityGroup.yellow:
                    SplashKit.LoadBitmap("YellowPlayerFront"+city, "YellowPlayerCardFront.png");
                    _frontOfCard = SplashKit.BitmapNamed("YellowPlayerFront"+city);
                    //_frontOfCard = SplashKit.BitmapNamed("YellowPlayerFront");
                    break;
            }
            
            //SplashKit.LoadBitmap("testFront"+city, "test.jpg");
            //_frontOfCard = SplashKit.BitmapNamed("testFront"+city);
            // SplashKit.LoadBitmap("ship", "Aquarii.png");
            // SplashKit.DrawBitmap("ship", 200, 200);
            // SplashKit.DrawBitmap("testFront"+city, 100, 100);
            // if(_frontOfCard != null) {
            //     Console.WriteLine(_frontOfCard);
            // } else {
            //     Console.WriteLine(_frontOfCard);
            // }

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