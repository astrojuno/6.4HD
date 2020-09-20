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
        // to create a good sized window. tested on 15" laptop. 
        // if you change this then you'll need to change ALL the city locations on the board also!
        private const double SCALAR = 0.25F;
        private const int WINDOW_HEIGHT = 873; 
        private const int WINDOW_WIDTH = 1316;
        private const int INFECTION_X = 23;
        private const int INFECTION_Y = 23;
        private const int FLIPPED_INFECTION_X = 23;
        private const int FLIPPED_INFECTION_Y = 165;
        private const int PLAYER_CARD_X = 23;
        private const int PLAYER_CARD_Y = 650;
        private const int FLIPPED_PLAYER_CARD_X = 158;
        private const int FLIPPED_PLAYER_CARD_Y = 650;
        private const double PLAYER_PAWN_SPACING = 30;
        private const double INFECTION_CUBE_SIZE = 20;
        private const double INFECTION_CUBE_BUFFER = 5;
        public void playPandemic() {
            loadResources();
            
            //(int)(SplashKit.BitmapNamed("boardImage").Width * SCALAR);
            //Console.WriteLine("W: {0}\tH: {1}", WINDOW_WIDTH, WINDOW_HEIGHT);
            gameWindow = new Window("Pandemic", WINDOW_WIDTH, WINDOW_HEIGHT);
            
            board = new Board();

            // // for some unknown reason, splashkit draws a scaled bitmap anchored from the centre. so the 0, 0 point
            // // needs to be offset when scaling. frustrating.
            double xOffset = (WINDOW_WIDTH - SplashKit.BitmapNamed("boardImage").Width) / 2;
            double yOffset = (WINDOW_HEIGHT - SplashKit.BitmapNamed("boardImage").Height) / 2;
            
            drawBoard();

            // create the players
            MedicPlayer medic = new MedicPlayer();
            medic.Move(board.getCity("Atlanta"));
            board.addPlayer(medic);
            Player currentPlayer = board.players[0];

            GeneralistPlayer generalist = new GeneralistPlayer();
            generalist.Move(board.getCity("Atlanta"));
            board.addPlayer(generalist);

            // Infection card pile
            InfectionCard infectionCardToFlip = board.nextInfectionCard;
            InfectionCard flippedInfectionCard = null;
            // Player card pile
            PlayerCard playerCardToFlip = board.nextPlayerCard;
            PlayerCard flippedPlayerCard = null;

            // Rects to keep track of important spaces on the board
            Rectangle infCardRect;
            Rectangle playerCardRect;
            infCardRect = drawInfectionCards(infectionCardToFlip, flippedInfectionCard);
            playerCardRect = drawPlayerCards(playerCardToFlip, flippedPlayerCard);

            // teach the player
            string[] teaching = new string[] {"Top left is your infection card pile (click to continue)", 
                                            "Next to that is your infection rate, that's how many infection cards you have to flip each round",
                                            "Bottom left is your player card pile. You have two of these already, and you'll draw two more each turn",
                                            "Top right is your outbreak tracker",
                                            "The current player, and their turn options are here on the right",
                                            "You can lose in many ways...",
                                            "If you run out of cubes for a disease, you'll lose",
                                            "If you outbreak 4 times, you'll lose",
                                            "If you run out of player cards in the pile, you'll lose",
                                            "You win if you cure all diseases. Cure a disease by handing in 4 cards of that colour in Atlanta"};
            //int count = 0;
            gameWindow.Refresh(60);
            for(int i = 0; i < teaching.Length; i++) {
                showMessage(teaching[i]);
                gameWindow.Refresh(60);
            }
 
            // initial infection
            flippedInfectionCard = infectBoard(board);

            showMessage("OK, now it's time to play. The current player and options are here on the right ->");
            
            // deal two cards to each player
            foreach(Player player in board.players) {
                player.AddCardToHand(board.nextPlayerCard);
                player.AddCardToHand(board.nextPlayerCard);
            }
            
            for(int i = 0; i < 12; i++) {
                generalist.AddCardToHand(board.nextPlayerCard);
            }
            
            while(!gameWindow.CloseRequested) {
                SplashKit.ProcessEvents();
                // draw all the board items
                drawBoard();
                infCardRect = drawInfectionCards(infectionCardToFlip, flippedInfectionCard);
                playerCardRect = drawPlayerCards(playerCardToFlip, flippedPlayerCard);
                
                
                // ALL BOARD DRAWING NEEDS TO HAPPEN BELOW HERE

                drawPlayers(board.players);
                drawHUD(currentPlayer);
                drawCityInfections();
                drawDiseaseLevels(board.diseases);

                // check for game ending conditions
                bool outOfCubes = false;
                foreach(Disease disease in board.diseases) {
                    if(disease.hasLostGame()) {
                        outOfCubes = true;
                    }
                }
                if(board.outOfInfectionCards || board.outOfPlayerCards || outOfCubes) {
                    while(true) {
                        gameWindow.Clear(Color.Red);
                        gameWindow.Refresh(60);
                    }
                }
                
                // move to an adjoining city
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
                // fly to a city whose card you discard
                if(SplashKit.KeyTyped(KeyCode.Num2Key)) {
                    PlayerCard chosenCard = playerChosenCard(currentPlayer, "Choose city card to move to that city");
                    currentPlayer.DiscardCard(chosenCard);
                    flippedPlayerCard = chosenCard;
                    currentPlayer.Move(board.getCity(chosenCard.city));
                }
                // fly to any city by discarding your current city card
                if(SplashKit.KeyTyped(KeyCode.Num3Key)) {
                    PlayerCard chosenCard = playerChosenCard(currentPlayer, "Choose your current city card");
                    drawBoard();
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
                // remove a disease cube
                if(SplashKit.KeyTyped(KeyCode.Num4Key)) {
                    // calculate how many cubes to put back
                    int previousInfectionLevel = currentPlayer.location.infectionLevel;
                    currentPlayer.TreatInfection();
                    int newInfectionLevel = currentPlayer.location.infectionLevel;
                    Disease diseaseAtCity = board.GetDisease(currentPlayer.location.type);
                    diseaseAtCity.returnCube(previousInfectionLevel - newInfectionLevel);
                }
                // share knowledge. transfer the city card for the city you are in to another
                // player who is also in that city
                if(SplashKit.KeyTyped(KeyCode.Num5Key)) {
                    // give or take a city card
                    // you must be sharing the city with someone
                    bool locationShared = false;
                    foreach(Player player in board.players) {
                        if(player.location == currentPlayer.location && player != currentPlayer) {
                            locationShared = true;
                        }
                    }
                    if(locationShared) {
                        // check if we have the card
                        if(doesPlayerHaveCard(currentPlayer)) {
                            PlayerCard chosenCard = null;
                            while(chosenCard == null || chosenCard.city.ToLower() != currentPlayer.location.name.ToLower()) {
                                chosenCard = playerChosenCard(currentPlayer, "You can share your current city card");
                                                        
                                if(chosenCard.city.ToLower() != currentPlayer.location.name.ToLower()) {
                                    showMessage("You can only give the card for the city you are in");
                                    drawBoard();
                                    infCardRect = drawInfectionCards(infectionCardToFlip, flippedInfectionCard);
                                    playerCardRect = drawPlayerCards(playerCardToFlip, flippedPlayerCard);
                                    drawPlayers(board.players);
                                    drawHUD(currentPlayer);
                                    drawCityInfections();
                                    gameWindow.Refresh(60);
                                }
                            }
                            // redraw the board to remove the card images
                            drawBoard();
                            infCardRect = drawInfectionCards(infectionCardToFlip, flippedInfectionCard);
                            playerCardRect = drawPlayerCards(playerCardToFlip, flippedPlayerCard);
                            drawPlayers(board.players);
                            drawHUD(currentPlayer);
                            drawCityInfections();
                            gameWindow.Refresh(60);
                            
                            // get the player to give the card to
                            Player playerToGiveCardTo = getPlayer();
                            board.transferCard(currentPlayer, playerToGiveCardTo, chosenCard);
                        } else {
                            // take the card from another player
                            Player playerWithCard = null;
                            // find the player with the card
                            foreach(Player player in board.players) {
                                if(doesPlayerHaveCard(player)) {
                                    playerWithCard = player;
                                    break;
                                }
                            }

                            // see if they're sharing your location
                            if(playerWithCard != null) {
                                if(playerWithCard.location == currentPlayer.location) {
                                    // take the card
                                    PlayerCard cardToTransfer = null;
                                    foreach(PlayerCard card in playerWithCard.cardsInHand) {
                                        if(card.city.ToLower() == playerWithCard.location.name.ToLower()) {
                                            cardToTransfer = card;
                                        }
                                    }
                                    string alert = "Taking " + currentPlayer.location + " card from " +  playerWithCard.type;
                                    showMessage(alert);
                                    board.transferCard(playerWithCard, currentPlayer, cardToTransfer);
                                }
                            }
                        }
                    } else {
                        showMessage("You must be sharing your city with another player to share knowledge");
                    }
                }
                // discover a cure
                if(SplashKit.KeyTyped(KeyCode.Num6Key)) {
                    // check location is atlanta
                    if(currentPlayer.location == board.getCity("Atlanta")) {
                        // check you have 4 of the right cards 
                        List<CityGroup> diseasesPlayerCanCure = currentPlayer.CanDiscoverCure();
                        if(diseasesPlayerCanCure.Count != 0) {
                            bool hasCured = false;
                            foreach(CityGroup diseaseColour in diseasesPlayerCanCure) {
                                Disease diseaseToCheck = board.GetDisease(diseaseColour);
                                if(!diseaseToCheck.isCured) {
                                    diseaseToCheck.cureDisease();
                                    hasCured = true;
                                    break;
                                }
                            }
                            if(!hasCured) {
                                showMessage("You don't have the right city cards to cure any diseases");
                            }
                        } else {
                            showMessage("You need 4 cards of the same colour to cure a disease");
                        }

                    } else {
                        showMessage("You can only discover a cure in Atlanta");
                    }
                    
                    // check if disease is cured already
                    // cure the disease
                }
                // show your player cards
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
            SplashKit.LoadBitmap("generalist", "generalistPawn.png");
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
                    case playerType.Dispatcher:
                        Xoffset = PLAYER_PAWN_SPACING;
                        Yoffset = 0-PLAYER_PAWN_SPACING;
                        break;
                    case playerType.Generalist:
                        Xoffset = 0-PLAYER_PAWN_SPACING;
                        Yoffset = PLAYER_PAWN_SPACING;
                        break;
                    case playerType.Researcher:
                        Xoffset = PLAYER_PAWN_SPACING;
                        Yoffset = PLAYER_PAWN_SPACING;
                        break;
                }
                playerX = player.location.cityCentre.X + Xoffset;
                playerY = player.location.cityCentre.Y + Yoffset;
                
                player.xloc = playerX;
                player.yloc = playerY;

                SplashKit.DrawBitmap(player.pawn, playerX, playerY);
            }
        }

        // initial infection for the board
        private InfectionCard infectBoard(Board board) {
            bool showAlert = true;
            while(showAlert) {
                SplashKit.ProcessEvents();
                showMessage("We need to infect the board. Turn over 6 infection cards.");
                if(SplashKit.MouseClicked(MouseButton.LeftButton) || SplashKit.KeyTyped(KeyCode.ReturnKey) || SplashKit.KeyTyped(KeyCode.SpaceKey)) { 
                    showAlert = false;
                }
                gameWindow.Refresh(60);
            }
            drawBoard();
            int infectedCities = 0;
            InfectionCard flippedInfectionCard = null;
            InfectionCard infectionCardToFlip = board.nextInfectionCard;
            Rectangle infCardRect = drawInfectionCards(infectionCardToFlip, flippedInfectionCard);
            
            while(infectedCities < 6) {
                SplashKit.ProcessEvents();
                if(SplashKit.MouseClicked(MouseButton.LeftButton)){
                    Point2D mouseLoc = SplashKit.MousePosition();
                    if(SplashKit.PointInRectangle(mouseLoc, infCardRect)) {
                        flippedInfectionCard = infectionCardToFlip;
                        flippedInfectionCard.isFaceUp = true;
                        board.putInfectionCardIntoFlippedPile(flippedInfectionCard);
                        infectionCardToFlip = board.nextInfectionCard;
                        drawInfectionCards(infectionCardToFlip, flippedInfectionCard);
                        City cityToInfect = board.getCity(flippedInfectionCard.city);
                        Disease currentDisease = board.GetDisease(cityToInfect.type);
                        if(infectedCities < 2) {
                            for(int i = 0; i < 3; i++) {
                                cityToInfect.increaseInfection();
                                currentDisease.useCube(1);
                            }
                        } else if(infectedCities < 4) {
                            for(int i = 0; i < 2; i++) {
                                cityToInfect.increaseInfection();
                                currentDisease.useCube(1);
                            }
                        } else {
                            cityToInfect.increaseInfection();
                            currentDisease.useCube(1);
                        }
                        infectedCities++;
                    }
                }
                drawCityInfections();
                gameWindow.Refresh(60);
            }
            return flippedInfectionCard;
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

        // shows a board covering message
        private void showMessage(string message) {
            Rectangle messageRect = new Rectangle();
            messageRect.X = 150;
            messageRect.Y = 150;
            messageRect.Width = gameWindow.Width - 300;
            messageRect.Height = gameWindow.Height - 300;
            double textX = ((messageRect.Width - SplashKit.TextWidth(message, "roboto", 20)) / 2) + messageRect.X;
            double textY = ((messageRect.Height - SplashKit.TextHeight(message, "roboto", 20)) / 2) + messageRect.Y;
            gameWindow.FillRectangle(Color.BurlyWood, messageRect);
            gameWindow.DrawText(message, Color.Black, "roboto",  20, textX, textY);
            gameWindow.Refresh(60);
            while(true) {
                SplashKit.ProcessEvents();
                if(SplashKit.MouseClicked(MouseButton.LeftButton) || SplashKit.KeyTyped(KeyCode.ReturnKey) || SplashKit.KeyTyped(KeyCode.SpaceKey) || SplashKit.KeyTyped(KeyCode.EscapeKey)) { 
                    return;
                }
            }
        }

        // draws the infection squares on the cities
        private void drawCityInfections() {
            foreach(City city in board.cities) {
                for(int i = 0; i < city.infectionLevel; i++) {
                    Rectangle cubeRect = new Rectangle();
                    cubeRect.Width = INFECTION_CUBE_SIZE;
                    cubeRect.Height = INFECTION_CUBE_SIZE;
                    cubeRect.X = city.cityCentre.X - ((INFECTION_CUBE_SIZE + INFECTION_CUBE_BUFFER) * (1.5 - i));
                    cubeRect.Y = city.cityCentre.Y + INFECTION_CUBE_SIZE;
                    gameWindow.FillRectangle(city.cityColour, cubeRect);
                }
                
            }
        }

        // draws the board image
        private void drawBoard() {
            // here for drawing the board...
            double xOffset = (WINDOW_WIDTH - SplashKit.BitmapNamed("boardImage").Width) / 2;
            double yOffset = (WINDOW_HEIGHT - SplashKit.BitmapNamed("boardImage").Height) / 2;
            
            gameWindow.DrawBitmap(SplashKit.BitmapNamed("boardImage"), xOffset, yOffset, SplashKit.OptionScaleBmp(SCALAR, SCALAR));
        }

        // can this be generalised with getCard?
        private Player getPlayer() {
            Player chosenPlayer = null;
            while(chosenPlayer == null) {
                SplashKit.ProcessEvents();
                drawHUDWarning("Click the player to give your card to");
                if(SplashKit.MouseClicked(MouseButton.LeftButton)) {
                    Point2D mouseLoc = SplashKit.MousePosition();
                    foreach(Player player in board.players) {
                        Rectangle playerRect = new Rectangle();
                        playerRect.X = player.xloc;
                        playerRect.Y = player.yloc;
                        playerRect.Width = player.pawn.BoundingRectangle().Width;
                        playerRect.Height = player.pawn.BoundingRectangle().Height;
                        if(SplashKit.PointInRectangle(mouseLoc, playerRect)) {
                            return player;
                        }
                    }
                }
                gameWindow.Refresh(60);
            }
            return null;
        }

        // returns true if a city card is in the players hand
        private bool doesPlayerHaveCard(Player player) {
            foreach(PlayerCard card in player.cardsInHand) {
                if(card.city.ToLower() == player.location.name.ToLower()) {
                    return true;
                }
            }
            return false;
        }

        // allocates the drawing of the disease info
        private void drawDiseaseLevels(List<Disease> diseases) {
            double redRectX = WINDOW_WIDTH - 195;
            double rectY = WINDOW_HEIGHT - 65;
            double yellowRectX = redRectX + 65;
            double blueRectX = yellowRectX + 65;
            
        
            foreach(Disease disease in diseases) {
                switch(disease.type) {
                    case CityGroup.red:
                        drawDiseaseHUD(redRectX, rectY, SplashKit.ColorRed(), disease.cubes, disease.isCured);
                        break;
                    case CityGroup.yellow:
                        drawDiseaseHUD(yellowRectX, rectY, SplashKit.ColorYellow(), disease.cubes, disease.isCured);
                        break;
                    case CityGroup.blue:
                        drawDiseaseHUD(blueRectX, rectY, SplashKit.ColorBlue(), disease.cubes, disease.isCured);
                        break;
                }
            }
            
            gameWindow.Refresh(60);
        }

        // draws the disease info
        private void drawDiseaseHUD(double rectX, double rectY, Color diseaseColour, int diseaseCubesLeft, bool isCured) {
            string cubesLeft = "Cubes Left";
            string cubeNumber = diseaseCubesLeft.ToString();

            if(isCured) {
                diseaseColour = SplashKit.ColorGreen();
                cubesLeft = "";
                cubeNumber = "✔️";
            }

            // create the rectangle
            Rectangle diseaseRect = new Rectangle();
            diseaseRect.X = rectX;
            diseaseRect.Y = rectY;
            diseaseRect.Width = 40;
            diseaseRect.Height = 45;
            SplashKit.FillRectangle(diseaseColour, diseaseRect);

            // put the title in the rectangle
            int cubesWidth = SplashKit.TextWidth(cubesLeft, SplashKit.FontNamed("roboto"), 8);
            double cubesX = diseaseRect.X + ((diseaseRect.Width - cubesWidth) / 2);
            double cubesY = diseaseRect.Y + ((diseaseRect.Width - cubesWidth) / 2);
            SplashKit.DrawText(cubesLeft, SplashKit.ColorBlack(), SplashKit.FontNamed("roboto"), 8, cubesX, cubesY);

            // put the cube count in the rect
            int cubeNoWidth = SplashKit.TextWidth(cubeNumber, SplashKit.FontNamed("roboto"), 16);
            int cubeNoHeight = SplashKit.TextHeight(cubeNumber, SplashKit.FontNamed("roboto"), 16);
            double cubeNoX = diseaseRect.X + ((diseaseRect.Width - cubeNoWidth) / 2);
            double cubeNoY = diseaseRect.Y + ((diseaseRect.Height - cubeNoHeight) / 2);
            SplashKit.DrawText(cubeNumber, SplashKit.ColorBlack(), SplashKit.FontNamed("roboto"), 16, cubeNoX, cubeNoY);

        }
    }
}

    