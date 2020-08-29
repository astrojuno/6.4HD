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
        private const double PLAYER_PAWN_SPACING = 30;
        public void playPandemic() {
            loadResources();
            
            // to create a good sized window. tested on 15" laptop. 
            // if you change this then you'll need to change ALL the city locations on the board also!
            double scalar = 0.25F;
            int windowHeight = 873; //(int)(SplashKit.BitmapNamed("boardImage").Height * scalar);
            int windowWidth = 1316; //(int)(SplashKit.BitmapNamed("boardImage").Width * scalar);
            //Console.WriteLine("W: {0}\tH: {1}", windowWidth, windowHeight);
            gameWindow = new Window("Pandemic", windowWidth, windowHeight);
            
            board = new Board();

            // for some unknown reason, splashkit draws a scaled bitmap anchored from the centre. so the 0, 0 point
            // needs to be offset when scaling. frustrating.
            double xOffset = (windowWidth - SplashKit.BitmapNamed("boardImage").Width) / 2;
            double yOffset = (windowHeight - SplashKit.BitmapNamed("boardImage").Height) / 2;
            
            gameWindow.DrawBitmap(SplashKit.BitmapNamed("boardImage"), xOffset, yOffset, SplashKit.OptionScaleBmp(scalar, scalar));
            //board.drawRects();

            MedicPlayer medic = new MedicPlayer();
            medic.Move(board.getCity("Atlanta"));
            board.addPlayer(medic);
            Player currentPlayer = board.players[0];

            // Infection card pile
            InfectionCard infectionCardToFlip = board.nextInfectionCard;
            InfectionCard flippedInfectionCard = null;
            // Player card pile
            PlayerCard playerCardToFlip = board.nextPlayerCard;
            PlayerCard flippedPlayerCard = null;

            // Rects to keep track of important spaces on the board
            Rectangle infCardRect;
            Rectangle playerCardRect;

            //medic.AddCardToHand(board.nextPlayerCard);
            //medic.AddCardToHand(board.nextPlayerCard);
            for(int i = 0; i < 15; i++) {
                medic.AddCardToHand(board.nextPlayerCard);
            }
            
            while(!gameWindow.CloseRequested) {
                SplashKit.ProcessEvents();
                // draw all the board items
                gameWindow.DrawBitmap(SplashKit.BitmapNamed("boardImage"), xOffset, yOffset, SplashKit.OptionScaleBmp(scalar, scalar));
                infCardRect = drawInfectionCards(infectionCardToFlip, flippedInfectionCard);
                playerCardRect = drawPlayerCards(playerCardToFlip, flippedPlayerCard);
                
                // ALL BOARD DRAWING NEEDS TO HAPPEN BELOW HERE

                drawPlayers(board.players);
                drawHUD(currentPlayer);

                // check for game ending conditions
                if(board.outOfInfectionCards || board.outOfPlayerCards) {
                    while(true) {
                        gameWindow.Clear(Color.Red);
                        gameWindow.Refresh(60);
                    }
                }
                
                if(SplashKit.MouseClicked(MouseButton.LeftButton)){
                    Point2D mouseLoc = SplashKit.MousePosition();
                    //Console.WriteLine("X: {0}\tY: {1}", mouseLoc.X, mouseLoc.Y);
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
                            //Console.WriteLine("City: {0}", clickedCity.name);
                        }
                    }
                }
                if(SplashKit.KeyTyped(KeyCode.Num1Key)) {
                    City cityToMoveTo = null;
                    while(cityToMoveTo == null) {
                        cityToMoveTo = getCityToMoveTo(board, "Click on an adjoining city to select it");
                        if(cityToMoveTo != null) {
                            if(currentPlayer.location.connectedCities.Contains(cityToMoveTo)) {
                                currentPlayer.Move(cityToMoveTo);
                            } else {
                                cityToMoveTo = null;
                            }
                        }
                    } 
                }
                if(SplashKit.KeyTyped(KeyCode.Num2Key)) {
                    PlayerCard chosenCard = playerChosenCard(currentPlayer, "Choose city card to move to that city");
                    currentPlayer.DiscardCard(chosenCard);
                    flippedPlayerCard = chosenCard;
                    currentPlayer.Move(board.getCity(chosenCard.city));
                }
                if(SplashKit.KeyTyped(KeyCode.Num3Key)) {
                    PlayerCard chosenCard = playerChosenCard(currentPlayer, "Choose your current city card");
                    gameWindow.DrawBitmap(SplashKit.BitmapNamed("boardImage"), xOffset, yOffset, SplashKit.OptionScaleBmp(scalar, scalar));
                    drawPlayers(board.players);

                    if(currentPlayer.location == board.getCity(chosenCard.city)) {
                        City cityToMoveTo = null;
                        while(cityToMoveTo == null) {
                            cityToMoveTo = getCityToMoveTo(board, "Click any city to move there");
                            if(cityToMoveTo != null) {
                                currentPlayer.Move(cityToMoveTo);
                                currentPlayer.DiscardCard(chosenCard);
                                flippedPlayerCard = chosenCard;
                            }
                        } 
                    }
                }
                if(SplashKit.KeyTyped(KeyCode.Num4Key)) {
                     up to here, impliment infections first
                }
                if(SplashKit.KeyTyped(KeyCode.Num7Key)) {
                    showCards(currentPlayer);
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
            double Xoffset = 0;
            double Yoffset = 0;

            foreach(Player player in players) {
                switch(player.type) {
                    case playerType.Medic:
                        Xoffset = 0-PLAYER_PAWN_SPACING;
                        Yoffset = 0-PLAYER_PAWN_SPACING;
                        break;
                    case playerType.OperationsExpert:
                        Xoffset = PLAYER_PAWN_SPACING;
                        Yoffset = 0-PLAYER_PAWN_SPACING;
                        break;
                    case playerType.QuarantineSpecialist:
                        Xoffset = 0-PLAYER_PAWN_SPACING;
                        Yoffset = PLAYER_PAWN_SPACING;
                        break;
                    case playerType.Researcher:
                        Xoffset = PLAYER_PAWN_SPACING;
                        Yoffset = PLAYER_PAWN_SPACING;
                        break;
                    // needed to silence compiler warnings
                    // default: 
                    //     Xoffset = 0-PLAYER_PAWN_SPACING;
                    //     Yoffset = 0-PLAYER_PAWN_SPACING;
                    //     break;
                }
                playerX = player.location.cityCentre.X + Xoffset;
                playerY = player.location.cityCentre.Y + Yoffset;
                //playerX = player.location.boardLocation.X + (player.location.boardLocation.Width / 2) - PLAYER_PAWN_SPACING;
                //playerY = player.location.boardLocation.Y + (player.location.boardLocation.Height / 2) - PLAYER_PAWN_SPACING;
                SplashKit.DrawBitmap(player.pawn, playerX, playerY);
            }
        }

        // draws the HUD that coaches players
        private void drawHUD(Player player) {
            Rectangle hudRect = SplashKit.RectangleFrom(1096, 418, 317, 145);
            SplashKit.FillRectangle(Color.Black, hudRect);
            SplashKit.DrawText(player.typeToString + " you can do one of the following:", Color.White, "roboto", 10, hudRect.X+5, hudRect.Y+5);
            SplashKit.DrawText("1: Drive/Ferry (Move to a connected city)", Color.White, "roboto", 10, hudRect.X+5, hudRect.Y+15);
            SplashKit.DrawText("2: Direct Flight (Discard a City card to move", Color.White, "roboto", 10, hudRect.X+5, hudRect.Y+25);
            SplashKit.DrawText("directly to that city)", Color.White, "roboto", 10, hudRect.X+5, hudRect.Y+35);
            SplashKit.DrawText("3: Charter Flight (Discard the City card", Color.White, "roboto", 10, hudRect.X+5, hudRect.Y+45);
            SplashKit.DrawText("matching your city to move directly to any city)", Color.White, "roboto", 10, hudRect.X+5, hudRect.Y+55);
            SplashKit.DrawText("4: Treat Disease (Remove 1 disease cube", Color.White, "roboto", 10, hudRect.X+5, hudRect.Y+65);
            SplashKit.DrawText("from your city)", Color.White, "roboto", 10, hudRect.X+5, hudRect.Y+75);
            SplashKit.DrawText("5: Share Knowledge (Give or take the City card", Color.White, "roboto", 10, hudRect.X+5, hudRect.Y+85);
            SplashKit.DrawText("mathcing your city from a player in your city)", Color.White, "roboto", 10, hudRect.X+5, hudRect.Y+95);
            SplashKit.DrawText("6: Discover a Cure (In Atlanta, discard 4 City", Color.White, "roboto", 10, hudRect.X+5, hudRect.Y+105);
            SplashKit.DrawText("cards of the same colour to cure that disease)", Color.White, "roboto", 10, hudRect.X+5, hudRect.Y+115);
            SplashKit.DrawText("7: Show your cards in hand", Color.White, "roboto", 10, hudRect.X+5, hudRect.Y+125);
            SplashKit.DrawBitmap(player.pawn, hudRect.X-20, hudRect.Y);
        }

        // Get and return a city based on user mouse click
        private City getCityToMoveTo(Board board, string message) {
            City citySelected = null;
            while(citySelected == null) {
                drawHUDWarning(message);
                gameWindow.Refresh(60);
                SplashKit.ProcessEvents();
                if(SplashKit.MouseClicked(MouseButton.LeftButton)){
                    Point2D mouseLoc = SplashKit.MousePosition();
                    citySelected = board.isPointACity(mouseLoc);
                } 
            }

            return citySelected;
        }

        // draws a hud warning with specified text
        private void drawHUDWarning(string message) {
            Rectangle hudRect = SplashKit.RectangleFrom(1096, 418, 317, 145);
            SplashKit.FillRectangle(Color.Black, hudRect);
            SplashKit.DrawText(message, Color.White, "roboto", 10, hudRect.X+5, hudRect.Y+35);
        }

        // shows the players cards on screen
        private void showCards(Player player) {
            while(true) {
                SplashKit.ProcessEvents();
                displayCards(player);
                if(SplashKit.MouseClicked(MouseButton.LeftButton) || SplashKit.KeyTyped(KeyCode.ReturnKey) || SplashKit.KeyTyped(KeyCode.SpaceKey) || SplashKit.KeyTyped(KeyCode.Num7Key)) { 
                    
                    return;
                }
                gameWindow.Refresh(60);
            }
        }

        // gets a user selected card from their hand and returns it
        private PlayerCard playerChosenCard(Player player, string message) {
            PlayerCard chosenCard = null;
            while(chosenCard == null) {
                SplashKit.ProcessEvents();
                displayCards(player);
                drawHUDWarning(message);
                if(SplashKit.MouseClicked(MouseButton.LeftButton)) {
                    Point2D mouseLoc = SplashKit.MousePosition();
                    foreach(PlayerCard card in player.cardsInHand) {
                        Rectangle cardRect = new Rectangle();
                        cardRect.X = card.xLoc;
                        cardRect.Y = card.yLoc;
                        cardRect.Width = card.cardImage.BoundingRectangle().Width;
                        cardRect.Height = card.cardImage.BoundingRectangle().Height;
                        if(SplashKit.PointInRectangle(mouseLoc, cardRect)) {
                            return card;
                        }
                    }

                }
                gameWindow.Refresh(60);
            }
            // needed to silence compiler warnings
            return null;
        }

        // the mechanics behind showing the cards
        private void displayCards(Player player) {
            double cardSpacing = 15;
            double cardX;
            double cardY = cardSpacing;
            int split = 0;
            for(int i = 0; i < player.cardsInHand.Count; i++) {
                Card card = player.cardsInHand[i];
                card.isFaceUp = true;
                cardX = ((card.cardImage.BoundingRectangle().Width + cardSpacing) * (i-split)) + cardSpacing;
                
                if(cardX + card.cardImage.BoundingRectangle().Width + cardSpacing > gameWindow.Width) {
                    split = i;
                    cardY = cardY + card.cardImage.BoundingRectangle().Height + cardSpacing;
                    cardX = cardSpacing;
                }
                gameWindow.DrawBitmap(card.cardImage, cardX, cardY);
                card.xLoc = cardX;
                card.yLoc = cardY;
            }
        }
    }
}

    