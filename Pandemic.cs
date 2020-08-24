// Simple version of the Pandemic game
// John Ryder 219466419

using System;
using System.Collections.Generic;
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
        private const double PLAYER_PAWN_SPACING = 0;
        public void playPandemic() {
            loadResources();
            
            // to create a good sized window. tested on 15" laptop. 
            // if you change this then you'll need to change ALL the city locations on the board also!
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
            //board.drawRects();

            // ALL BOARD DRAWING NEEDS TO HAPPEN BELOW HERE

            MedicPlayer medic = new MedicPlayer();
            medic.Move(board.getCity("Atlanta"));
            board.addPlayer(medic);

            drawPlayers(board.players);

            drawHUD();
            
            
            // Infection card pile
            InfectionCard infectionCardToFlip = board.nextInfectionCard;
            InfectionCard flippedInfectionCard = null;
            // Player card pile
            PlayerCard playerCardToFlip = board.nextPlayerCard;
            PlayerCard flippedPlayerCard = null;

            // Rects to keep track of important spaces on the board
            Rectangle infCardRect;
            Rectangle playerCardRect;

            //double oldx = 0;
            //double oldy = 0;
            
            while(!gameWindow.CloseRequested) {
                SplashKit.ProcessEvents();

                infCardRect = drawInfectionCards(infectionCardToFlip, flippedInfectionCard);
                playerCardRect = drawPlayerCards(playerCardToFlip, flippedPlayerCard);

                
                if(SplashKit.MouseClicked(MouseButton.LeftButton)){
                    Point2D mouseLoc = SplashKit.MousePosition();
                    Console.WriteLine("X: {0}\tY: {1}", mouseLoc.X, mouseLoc.Y);
                    // if(oldx == 0) {
                    //     oldx = mouseLoc.X;
                    //     oldy = mouseLoc.Y;
                    // } else {
                    //     Console.WriteLine("diff x: {0}\tdiff y: {1}", mouseLoc.X - oldx, mouseLoc.Y - oldy);
                    //     oldx = 0;
                    //     oldy = 0;    
                    // }
                    
                    if(SplashKit.PointInRectangle(mouseLoc, playerCardRect)) {
                        flippedPlayerCard = playerCardToFlip;
                        flippedPlayerCard.isFaceUp = true;
                        playerCardToFlip = board.nextPlayerCard;
                    }
                    else if(SplashKit.PointInRectangle(mouseLoc, infCardRect)) {
                        flippedInfectionCard = infectionCardToFlip;
                        flippedInfectionCard.isFaceUp = true;
                        infectionCardToFlip = board.nextInfectionCard;
                    } else {
                        City clickedCity = board.isPointACity(mouseLoc);
                        if(clickedCity != null) {
                            Console.WriteLine("City: {0}", clickedCity.name);
                        }
                    }
                    
                }
                gameWindow.Refresh(60);
            }
        }

        // Loads the splashkit resources
        private void loadResources() {
            SplashKit.LoadFont("roboto", "Roboto-Bold.ttf");
            SplashKit.LoadBitmap("PlayerBack", "PlayerCardback.png");
            SplashKit.LoadBitmap("InfectionBack", "InfectionCardBack.png");
            SplashKit.LoadBitmap("boardImage", "Board.png");
            SplashKit.LoadBitmap("medic", "medicPawn.png");
        }

        // draws the infection card pile and returns the rect where the pile is
        private Rectangle drawInfectionCards(InfectionCard infectionCardToFlip, InfectionCard flippedInfectionCard) {
            gameWindow.DrawBitmap(infectionCardToFlip.cardImage, INFECTION_X, INFECTION_Y);
            if(flippedInfectionCard != null) {
                gameWindow.DrawBitmap(flippedInfectionCard.cardImage, FLIPPED_INFECTION_X, FLIPPED_INFECTION_Y);
            }

            return SplashKit.RectangleFrom(INFECTION_X, INFECTION_Y, infectionCardToFlip.cardImage.Width, infectionCardToFlip.cardImage.Height);
        }

        // draws the player card pile and returns the rect where the pile is
        private Rectangle drawPlayerCards(PlayerCard playerCardToFlip, PlayerCard flippedPlayerCard) {
            gameWindow.DrawBitmap(playerCardToFlip.cardImage, PLAYER_CARD_X, PLAYER_CARD_Y);
                if(flippedPlayerCard != null) {
                    gameWindow.DrawBitmap(flippedPlayerCard.cardImage, FLIPPED_PLAYER_CARD_X, FLIPPED_PLAYER_CARD_Y);
                }
            
            return SplashKit.RectangleFrom(PLAYER_CARD_X, PLAYER_CARD_Y, playerCardToFlip.cardImage.Width, playerCardToFlip.cardImage.Height);
        }

        // draws the players
        private void drawPlayers(List<Player> players) {
            double playerX; 
            double playerY; 
            foreach(Player player in players) {
                playerX = player.location.boardLocation.X + PLAYER_PAWN_SPACING;
                playerY = player.location.boardLocation.Y + PLAYER_PAWN_SPACING;
                SplashKit.DrawBitmap(player.pawn, playerX, playerY);
            }
        }

        // draws the HUD that coaches players
        private void drawHUD() {
            Rectangle hudRect = SplashKit.RectangleFrom(1096, 418, 317, 145);
            SplashKit.DrawRectangle(Color.Coral, hudRect);
            here
        }
    }
}

    