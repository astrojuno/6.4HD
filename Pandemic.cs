// Simple version of the Pandemic game
// John Ryder 219466419

using System;
using SplashKitSDK;

namespace Pandemic {
    

    public class Pandemic {
        static void Main(string[] args) {
            Console.WriteLine("hello world");
            PandemicGame game = new PandemicGame();
            game.playPandemic();
        }
    }

    public class PandemicGame {
        private Window gameWindow;
        //private PlayerCard _card;
        private Board board;
        private const int INFECTION_X = 23;
        private const int INFECTION_Y = 23;
        private const int FLIPPED_INFECTION_X = 23;
        private const int FLIPPED_INFECTION_Y = 165;
        private const int PLAYER_CARD_X = 23;
        private const int PLAYER_CARD_Y = 650;
        private const int FLIPPED_PLAYER_CARD_X = 158;
        private const int FLIPPED_PLAYER_CARD_Y = 650;
        public void playPandemic() {
            loadResources();
            
            // to create a good sized window. tested on 15" laptop. 
            double scalar = 0.25F;
            int windowHeight = (int)(SplashKit.BitmapNamed("boardImage").Height * scalar);
            int windowWidth = (int)(SplashKit.BitmapNamed("boardImage").Width * scalar);
            gameWindow = new Window("Pandemic", windowWidth, windowHeight);
            
            board = new Board();

            // for some unknown reason, splashkit draws a scaled bitmap anchored from the centre. so the 0, 0 point
            // needs to be offset when scaling. frustrating.
            double xOffset = (windowWidth - SplashKit.BitmapNamed("boardImage").Width) / 2;
            double yOffset = (windowHeight - SplashKit.BitmapNamed("boardImage").Height) / 2;
            
            gameWindow.DrawBitmap(SplashKit.BitmapNamed("boardImage"), xOffset, yOffset, SplashKit.OptionScaleBmp(scalar, scalar));
            
            InfectionCard infectionCardToFlip = board.nextInfectionCard;
            InfectionCard flippedInfectionCard = null;
            PlayerCard playerCardToFlip = board.nextPlayerCard;
            PlayerCard flippedPlayerCard = null;
            
            // if(playerCardToFlip.cardImage != null) {
            //     Console.WriteLine(playerCardToFlip.cardImage);
            //     Console.WriteLine(playerCardToFlip.cardImage.BoundingRectangle().Height + " " + playerCardToFlip.cardImage.BoundingRectangle().Width);
            // } else {
            //     Console.WriteLine("null");
            // }

            while(!gameWindow.CloseRequested) {
                SplashKit.ProcessEvents();
                
                //perhaps move draw card to the card object, then it can keep track of it's location?
                //not sure this is a good idea, just a thought...
                //_card.xLoc = 100;
                //_card.yLoc = 100;
                
                gameWindow.DrawBitmap(playerCardToFlip.cardImage, PLAYER_CARD_X, PLAYER_CARD_Y);
                if(flippedPlayerCard != null) {
                    gameWindow.DrawBitmap(flippedPlayerCard.cardImage, FLIPPED_PLAYER_CARD_X, FLIPPED_PLAYER_CARD_Y);
                }
                
                gameWindow.DrawBitmap(infectionCardToFlip.cardImage, INFECTION_X, INFECTION_Y);
                if(flippedInfectionCard != null) {
                    gameWindow.DrawBitmap(flippedInfectionCard.cardImage, FLIPPED_INFECTION_X, FLIPPED_INFECTION_Y);
                }

                Rectangle cardRect = SplashKit.RectangleFrom(PLAYER_CARD_X, PLAYER_CARD_Y, playerCardToFlip.cardImage.Width, playerCardToFlip.cardImage.Height);
                Rectangle infCardRect = SplashKit.RectangleFrom(INFECTION_X, INFECTION_Y, infectionCardToFlip.cardImage.Width, infectionCardToFlip.cardImage.Height);
                //gameWindow.DrawRectangle(Color.Red, cardRect);
                //gameWindow.DrawRectangle(Color.Blue, infCardRect);
                
                if(SplashKit.MouseClicked(MouseButton.LeftButton)){
                    Point2D mouseLoc = SplashKit.MousePosition();
                    if(SplashKit.PointInRectangle(mouseLoc, cardRect)) {
                        flippedPlayerCard = playerCardToFlip;
                        flippedPlayerCard.isFaceUp = true;
                        playerCardToFlip = board.nextPlayerCard;
                        //Console.WriteLine("player card");
                    }
                    else if(SplashKit.PointInRectangle(mouseLoc, infCardRect)) {
                        flippedInfectionCard = infectionCardToFlip;
                        flippedInfectionCard.isFaceUp = true;
                        infectionCardToFlip = board.nextInfectionCard;
                        //Console.WriteLine("infection card");
                    }
                }
                gameWindow.Refresh(60);
            }
        }

        private void loadResources() {
            SplashKit.LoadFont("roboto", "Roboto-Bold.ttf");
            SplashKit.LoadBitmap("PlayerBack", "PlayerCardback.png");
            SplashKit.LoadBitmap("InfectionBack", "InfectionCardBack.png");
            SplashKit.LoadBitmap("boardImage", "Board.png");
        }
    }
}

    