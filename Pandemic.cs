// Simple version of the Pandemic game
// John Ryder 219466419

/****************************************************************************
* Hi! Thanks for evaluating my program. There are som things you should     *
* know before we continue. First, if you launch the game and the title bar  *
* is greyed out or dim, the game isn't running properly. As far as I can    *
* tell it's a problem with the SplashKit library and how Windows are        *
* created. Just quit and relaunch and you should be good to go.             *
*                                                                           *
* Second, there's a brief tutorial at the beginning, but the game is        *
* designed for people who have read the rules. The rules are available here:*
* https://images.zmangames.com/filer_public/48/20/482039b2-4b30-4fe9-9cf8-63ba8badc306/pandemic_hotzonena_printplay.pdf
*                                                                           *
* Lambdas are used, you can see examples of that in the constants section.  *
* Generics are used, and you can see and example of that in the function    *
* drawCards.                                                                *
*                                                                           *
* Other than that, I hope you like my game!                                 *
****************************************************************************/

using System;
using System.Collections.Generic;
using SplashKitSDK;

namespace Pandemic {
    // entry point for the program
    public class Pandemic {
        static void Main(string[] args) {
            // create an instance of the game and run it
            PandemicGame game = new PandemicGame();
            game.playPandemic();
        }
    }

    // This is the main interface to the game
    public class PandemicGame {
        // Constants
        // The window that will show the game. There seems to be a problem with Splashkit,
        // if your game launches with a greyed out or dim titlebar, quit and restart the game.
        private Window gameWindow;
        // An instance of the Board, which will handle a lot of the mechanics of the game
        private Board board;
        // to create a good sized window. tested on 15" laptop. 
        // if you change this then you'll need to change ALL the city locations on the board also!
        private const double SCALAR = 0.25F;
        private const int WINDOW_HEIGHT = 873; 
        // constants for drawing the cards
        private const int WINDOW_CARD_BUFFER = 250;
        private const int WINDOW_WIDTH = 1316 + WINDOW_CARD_BUFFER;
        private const int INFECTION_X = 23;
        private const int INFECTION_Y = 23;
        private const int FLIPPED_INFECTION_X = 23;
        private const int FLIPPED_INFECTION_Y = 165;
        private const int PLAYER_CARD_X = 23;
        private const int PLAYER_CARD_Y = 650;
        private const int FLIPPED_PLAYER_CARD_X = 158;
        private const int FLIPPED_PLAYER_CARD_Y = 650;
        // constants for the pawns offset
        private const double PLAYER_PAWN_SPACING = 30;
        // constants for the infection cubes
        private const double INFECTION_CUBE_SIZE = 20;
        private const double INFECTION_CUBE_BUFFER = 5;
        // the number of cards to flip for the initial infection
        private const int INITIAL_INFECTION_NUMBER = 6;
        // the player hand limit
        private const int HAND_LIMIT = 6;
        // constants for the markers
        private const double INFECTION_MARKER_X = 165;
        private const double INFECTION_MARKER_Y = 20;
        private const double INFECTION_MARKER_OFFSET = 65; 
        private const double OUTBREAK_MARKER_X = 1227;
        private const double OUTBREAK_MARKER_Y = 25;
        private const double OUTBREAK_MARKER_OFFSET = 75;
        // constants for the cube counters (bottom right)
        private const double RED_CUBE_COUNTER_OFFSET = 195;
        private const double CUBE_COUNTER_OFFSET = 65;
        // lambda to compare two strings
        private Func<string, string, bool> compareString = (s, c) => s.ToLower() == c.ToLower();
        // lambda to get the X and Y for a deck of cards
        private Func<Card, (double, double)> getDeckCardXY = c => {
                    if(c.GetType() == typeof(InfectionCard)) {
                        return (INFECTION_X, INFECTION_Y);
                    }
                    return (PLAYER_CARD_X, PLAYER_CARD_Y);
                };
        // lambda to get the X and Y for the flipped cards
        private Func<Card, (double, double)> getFlippedCardXY = c => {
                    if(c.GetType() == typeof(InfectionCard)) {
                        return (FLIPPED_INFECTION_X, FLIPPED_INFECTION_Y);
                    }
                    return (FLIPPED_PLAYER_CARD_X, FLIPPED_PLAYER_CARD_Y);
        };
        
        // This is the main loop that will play the game. There's setup first, then the loop.
        public void playPandemic() {
            // load resources needed during the game
            loadResources();
            
            // initiate the game window
            gameWindow = new Window("Pandemic", WINDOW_WIDTH, WINDOW_HEIGHT);
            
            // initiate the board
            board = new Board();

            // draw the board on the window
            drawBoard();

            // create the players
            MedicPlayer medic = new MedicPlayer();
            medic.Move(board.getCity("Atlanta"));
            board.addPlayer(medic);

            GeneralistPlayer generalist = new GeneralistPlayer();
            generalist.Move(board.getCity("Atlanta"));
            board.addPlayer(generalist);

            ResearcherPlayer researcher = new ResearcherPlayer();
            researcher.Move(board.getCity("Atlanta"));
            board.addPlayer(researcher);

            DispatchPlayer dispatcher = new DispatchPlayer();
            dispatcher.Move(board.getCity("Atlanta"));
            board.addPlayer(dispatcher);

            // set the current player
            Player currentPlayer = board.players[0];

            // a counter to track how many actions the current player has taken
            int actionsTaken = 0;

            // Infection card pile
            InfectionCard infectionCardToFlip = board.nextInfectionCard;
            
            // Player card pile
            PlayerCard playerCardToFlip = (PlayerCard)board.nextPlayerCard;
            
            // Rects to keep track of important spaces on the board
            // position 0 is the deck to flip
            // position 1 is the flipped pile
            Rectangle[] infCardRect = new Rectangle[2];
            Rectangle[] playerCardRect = new Rectangle[2];
            infCardRect = drawCards(infectionCardToFlip, board.lastInfectionCardFlipped);
            playerCardRect = drawCards(playerCardToFlip, board.lastPlayerCardFlipped);

            // teach the player
            string[] teaching = new string[] { "Welcome to John's Pandemic game! (click to continue)",
                                            "It is advisable to read the rules before playing. They can be found at zmangames.com",
                                            "Top left is your infection card pile", 
                                            "Next to that is your infection rate, that's how many infection cards you have to flip each round",
                                            "Bottom left is your player card pile. You have two of these cards already, and you'll draw two more each turn",
                                            "Top right is your outbreak tracker",
                                            "The current player, and their turn options are here on the right",
                                            "You can lose in many ways...",
                                            "If you run out of cubes for a disease, you'll lose",
                                            "If you outbreak 4 times, you'll lose",
                                            "If you run out of player cards in the pile, you'll lose",
                                            "You win if you cure all diseases. Cure a disease by handing in 4 cards of that colour in Atlanta"};
            
            gameWindow.Refresh(60);
            for(int i = 0; i < teaching.Length; i++) {
                showMessage(teaching[i]);
                gameWindow.Refresh(60);
            }
 
            // initial infection
            initialInfectBoard();

            showMessage("OK, now it's time to play. The current player and options are here on the right ->");
            
            // deal two cards to each player
            foreach(Player player in board.players) {
                player.AddCardToHand(board.nextPlayerCard as PlayerCard);
                player.AddCardToHand(board.nextPlayerCard as PlayerCard);
            }

            // this has to happen after the players have been dealt their cards
            board.insertEpidemicCards();
            
            // the main game loop
            while(!gameWindow.CloseRequested) {
                SplashKit.ProcessEvents();
                // draw all the board items
                drawGame(infectionCardToFlip, playerCardToFlip, currentPlayer);

                if(board.lastInfectionCardFlipped != null) {
                    infCardRect[1] = getCardRect(FLIPPED_INFECTION_X, FLIPPED_INFECTION_Y, board.lastInfectionCardFlipped.cardImage.Width, board.lastInfectionCardFlipped.cardImage.Height);
                }
                if(board.lastPlayerCardFlipped != null) {
                    playerCardRect[1] = getCardRect(FLIPPED_PLAYER_CARD_X, FLIPPED_PLAYER_CARD_Y, board.lastPlayerCardFlipped.cardImage.Width, board.lastPlayerCardFlipped.cardImage.Height);
                } 

                // check for game ending conditions
                // see if we're out of cubes
                bool outOfCubes = false;
                foreach(Disease disease in board.diseases) {
                    if(disease.hasLostGame()) {
                        outOfCubes = true;
                    }
                }
                // if we lost, present why we lost and quit
                if(board.outOfInfectionCards || board.outOfPlayerCards || outOfCubes || board.outbreakTracker >= 4) {
                    showMessage("Oh no, you lost!");
                    string message = "You ran out of ";
                    if(board.outOfInfectionCards) {
                        message += "infection cards; ";
                    }
                    if(board.outOfPlayerCards) {
                        message += "player cards; ";
                    }
                    if(outOfCubes) {
                        message += "cubes; ";
                    }
                    if(board.outbreakTracker >= 4) {
                        message += "outbreaks; ";
                    }

                    showMessage(message);
                    
                    SplashKit.QuitRequested();
                }
                // Yay, we won!
                if(board.wonGame) {
                    showMessage("YAY, you won!");
                    SplashKit.QuitRequested();
                }

                // click on the flipped cards to show that pile
                if(SplashKit.MouseClicked(MouseButton.LeftButton)) {
                    // check for the infection card pile
                    Point2D mouseLoc = SplashKit.MousePosition();
                    // show whichever card pile was clicked on
                    if(SplashKit.PointInRectangle(mouseLoc, infCardRect[1])) {
                        showCards(board.flippedInfectionCards);
                    } else if(SplashKit.PointInRectangle(mouseLoc, playerCardRect[1])) {
                        showCards(board.discardedPlayerCards);
                    }
                }

                // Here are the player actions that can be taken.
                // move to an adjoining city
                if(SplashKit.KeyTyped(KeyCode.Num1Key)) {
                    if(movePlayerToCity(currentPlayer)) {
                        actionsTaken++;
                    }
                }
                // fly to a city whose card you discard
                if(SplashKit.KeyTyped(KeyCode.Num2Key)) {
                    // get the player to choose one of their cards
                    PlayerCard chosenCard = playerChosenCard(currentPlayer, "Choose city card to move to that city");
                    if(chosenCard != null) {
                        // discard the card
                        currentPlayer.DiscardCard(chosenCard);
                        board.discardPlayerCard(chosenCard);

                        // move the player
                        currentPlayer.Move(board.getCity(chosenCard.city));
                        actionsTaken++;

                        // do the medic check
                        if(currentPlayer.type == playerType.Medic) {
                            clearDiseaseIfCured(currentPlayer);
                        }
                    }        
                }
                // fly to any city by discarding your current city card
                if(SplashKit.KeyTyped(KeyCode.Num3Key)) {
                    // get the player chosen card
                    PlayerCard chosenCard = playerChosenCard(currentPlayer, "Choose your current city card");
                    drawGame(infectionCardToFlip, playerCardToFlip, currentPlayer);

                    if(chosenCard != null) {
                        // make sure it is the card for the players current city
                        if(currentPlayer.location == board.getCity(chosenCard.city)) {
                            City cityToMoveTo = null;
                            while(cityToMoveTo == null) {
                                // get the city the player wants to move to
                                cityToMoveTo = getCityToMoveTo("Click any city to move there");
                                if(cityToMoveTo != null) {
                                    // move the player
                                    currentPlayer.Move(cityToMoveTo);
                                    currentPlayer.DiscardCard(chosenCard);
                                    board.discardPlayerCard(chosenCard);
        
                                    actionsTaken++;

                                    // do the medic check
                                    if(currentPlayer.type == playerType.Medic) {
                                        clearDiseaseIfCured(currentPlayer);
                                    }
                                }
                            } 
                        }
                    }
                    
                }
                // remove a disease cube
                if(SplashKit.KeyTyped(KeyCode.Num4Key)) {
                    // calculate how many cubes to put back
                    Disease diseaseAtCity = board.GetDisease(currentPlayer.location.type);
                    if(diseaseAtCity != null) {
                        int previousInfectionLevel = currentPlayer.location.infectionLevel;
                        // treat the infection
                        currentPlayer.TreatInfection(diseaseAtCity.isCured);
                        int newInfectionLevel = currentPlayer.location.infectionLevel;
                        int cubesRemoved = previousInfectionLevel - newInfectionLevel;
                        // return the cubes
                        diseaseAtCity.returnCube(cubesRemoved);
                        // don't take a move off if they didn't remove any cubes
                        if(cubesRemoved > 0) {
                            actionsTaken++;
                        }
                    }
                }
                // share knowledge. transfer the city card for the city you are in to another
                // player who is also in that city
                if(SplashKit.KeyTyped(KeyCode.Num5Key)) {
                    // give or take a city card
                    // you must be sharing the city with someone
                    bool locationShared = false;
                    bool userHasEscaped = false;
                    foreach(Player player in board.players) {
                        if(player.location == currentPlayer.location && player != currentPlayer) {
                            locationShared = true;
                        }
                    }
                    if(locationShared) {
                        // check if we have the card
                        if(doesPlayerHaveCard(currentPlayer) || currentPlayer.type == playerType.Researcher) {
                            PlayerCard chosenCard = null;
                            bool acceptableCardChosen = false;
                            // if we have the card, get the user to select it
                            while(chosenCard == null || acceptableCardChosen || userHasEscaped) {
                                string userMessage = "You can share your current city card";
                                if(currentPlayer.type == playerType.Researcher) {
                                    userMessage = "You can share any city card";
                                }
                                chosenCard = playerChosenCard(currentPlayer, userMessage);
                                // getting a null card here suggests the user has hit escape when choosing a card
                                if(chosenCard == null) {
                                    userHasEscaped = true;
                                    break;
                                }
                                // check to make sure the card chosen is OK
                                if(compareString(chosenCard.city, currentPlayer.location.name) || currentPlayer.type == playerType.Researcher) {
                                    acceptableCardChosen = true;
                                } else if(!compareString(chosenCard.city, currentPlayer.location.name)) {
                                    showMessage("You can only give the card for the city you are in");
                                    
                                    drawGame(infectionCardToFlip, playerCardToFlip, currentPlayer);
                                }
                            }
                            if(!userHasEscaped) {
                                drawGame(infectionCardToFlip, playerCardToFlip, currentPlayer);
                                
                                // get the player to give the card to
                                Player playerToGiveCardTo = getPlayer();
                                board.transferCard(currentPlayer, playerToGiveCardTo, chosenCard);
                                actionsTaken++;
                            }
                        // otherwise, we're taking a card from another player
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
                                        if(compareString(card.city, playerWithCard.location.name)) {
                                            cardToTransfer = card;
                                        }
                                    }
                                    string alert = "Taking " + currentPlayer.location + " card from " +  playerWithCard.type;
                                    showMessage(alert);
                                    // transfer the card
                                    board.transferCard(playerWithCard, currentPlayer, cardToTransfer);
                                    actionsTaken++;
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
                            // cure the disease
                            foreach(CityGroup diseaseColour in diseasesPlayerCanCure) {
                                Disease diseaseToCheck = board.GetDisease(diseaseColour);
                                if(diseaseToCheck != null && !diseaseToCheck.isCured) {
                                    diseaseToCheck.cureDisease();
                                    hasCured = true;
                                    // discard the four cards
                                    foreach(PlayerCard card in currentPlayer.removeFourCardsToCureDisease(diseaseColour)) {
                                        board.discardPlayerCard(card);
                                    }
                                    actionsTaken++;
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
                }
                // show your player cards
                if(SplashKit.KeyTyped(KeyCode.Num7Key)) {
                    showCards(currentPlayer.cardsInHand);
                }
                // Dispatcher options
                // move a pawn to a city with another pawn
                if(SplashKit.KeyTyped(KeyCode.Num8Key) && currentPlayer.type == playerType.Dispatcher) {
                    // get the player you want to move
                    Player playerToMove = getPlayer();
                    bool chosenCityIsOK = false;
                    City cityToMoveTo = null;
                    // get the city to move to
                    while(!chosenCityIsOK) {
                        cityToMoveTo = getCityToMoveTo("Select a city with another player in it");
                        foreach(Player player in board.players) {
                            if(player.location == cityToMoveTo) {
                                chosenCityIsOK = true;
                                break;
                            }
                        }
                    }
                    
                    // move the player
                    playerToMove.Move(cityToMoveTo);
                    actionsTaken++;
                }
                // move another pawn to a connected city
                if(SplashKit.KeyTyped(KeyCode.Num9Key) && currentPlayer.type == playerType.Dispatcher) {
                    // get the other player
                    Player playerToMove = getPlayer();
                    // move them
                    if(movePlayerToCity(playerToMove)) {
                        actionsTaken++;
                    }
                }

                gameWindow.Refresh(60);

                // check to see if the player has finished their turn
                if(actionsTaken >= currentPlayer.turns) {
                    // draw two player cards
                    playerDrawsCityCards(currentPlayer, infectionCardToFlip);
                    // you can only hold so many cards
                    while(currentPlayer.cardsInHand.Count > HAND_LIMIT) {
                        discarCards(currentPlayer);
                    }
                    // infect the new cities
                    playerDrawsInfectionCards(currentPlayer, playerCardToFlip);
                    // incriment the player
                    currentPlayer = nextPlayer(currentPlayer);
                    // reset the action count
                    actionsTaken = 0;
                }
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
            SplashKit.LoadBitmap("researcher", "researcherPawn.png");
            SplashKit.LoadBitmap("dispatcher", "dispatcherPawn.png");
            SplashKit.LoadBitmap("infectionMarker", "infectionMarker.png");
            SplashKit.LoadBitmap("outbreakMarker", "outbreakMarker.png");
            SplashKit.LoadBitmap("generalistHUD", "generalistHUD.png");
            SplashKit.LoadBitmap("researcherHUD", "researcherHUD.png");
            SplashKit.LoadBitmap("medicHUD", "medicHUD.png");
            SplashKit.LoadBitmap("dispatcherHUD", "dispatcherHUD.png");
        }

        // returns the rectangles for the given cards
        private Rectangle getCardRect(double xLoc, double yLoc, double width, double height) {
            Rectangle rectToReturn = new Rectangle();
            rectToReturn = SplashKit.RectangleFrom(xLoc, yLoc, width, height);
            return rectToReturn;
        }

        // draws the card pile and returns the rect where the pile is
        // generic function to deal with any type of Card
        private Rectangle[] drawCards<T>(T cardToFlip, T flippedCard) where T : Card {
            Rectangle[] rectsToReturn = new Rectangle[2];
            // get the tuple from the lambda
            var (xLoc, yLoc) = getDeckCardXY(cardToFlip);
            // do the drawing
            gameWindow.DrawBitmap(cardToFlip.cardImage, xLoc, yLoc);
            rectsToReturn[0] = getCardRect(xLoc, yLoc, cardToFlip.cardImage.Width, cardToFlip.cardImage.Height);
            if(flippedCard != null) {
                (xLoc, yLoc) = getFlippedCardXY(flippedCard);
                gameWindow.DrawBitmap(flippedCard.cardImage, xLoc, yLoc);
                rectsToReturn[1] = getCardRect(xLoc, yLoc, flippedCard.cardImage.Width, flippedCard.cardImage.Height);
            }

            return rectsToReturn;
        }

        // draws the players
        private void drawPlayers(List<Player> players) {
            // players are offset from the centre of their city so they don't overlap
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
        private void initialInfectBoard() {
            // show the warning
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
            // do the infections
            infectBoard(INITIAL_INFECTION_NUMBER, null, null);    
        }

        // the mechanics behind infecting the board
        private void infectBoard(int numberOfInfections, Card playerCardToFlip, Player currentPlayer) {
            int infectedCities = 0;
            InfectionCard flippedInfectionCard = board.lastInfectionCardFlipped;
            InfectionCard infectionCardToFlip = board.nextInfectionCard;
            Rectangle infCardRect = drawCards(infectionCardToFlip, flippedInfectionCard)[0];
            drawGame(infectionCardToFlip, playerCardToFlip, currentPlayer);
            
            // infect with the specified number of infections
            while(infectedCities < numberOfInfections) {
                if(playerClickedInRectangle(infCardRect)) {
                    flippedInfectionCard = infectionCardToFlip;
                    flippedInfectionCard.isFaceUp = true;
                    board.putInfectionCardIntoFlippedPile(flippedInfectionCard);
                    infectionCardToFlip = board.nextInfectionCard;
                    drawCards(infectionCardToFlip, flippedInfectionCard);
                    
                    City cityToInfect = board.getCity(flippedInfectionCard.city);
                    Disease currentDisease = board.GetDisease(cityToInfect.type);
                    // if number of infections is 6, then this is the initial infection
                    if(numberOfInfections == INITIAL_INFECTION_NUMBER) {
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
                    } else {
                        // otherwise it's a players turn that is infecting the city
                        cityToInfect.increaseInfection();
                        currentDisease.useCube(1);
                    }
                    infectedCities++;
                }
                
                // draw the infections we've just done
                drawCityInfections();
                gameWindow.Refresh(60);
            }
        }

        // returns true if player clicks in given rectangle
        private bool playerClickedInRectangle(Rectangle rect) {
            while(true) { 
                SplashKit.ProcessEvents();
                if(SplashKit.MouseClicked(MouseButton.LeftButton)){
                    Point2D mouseLoc = SplashKit.MousePosition();
                    if(SplashKit.PointInRectangle(mouseLoc, rect)) {
                        return true;
                    }
                }
            }
        }

        // draws the HUD that informs players
        private void drawHUD(Player player) {
            string bitmapName = "";
            switch(player.type) {
                case playerType.Dispatcher:
                    bitmapName = "dispatcherHUD";
                    break;
                case playerType.Generalist:
                    bitmapName = "generalistHUD";
                    break;
                case playerType.Medic:
                    bitmapName = "medicHUD";
                    break;
                case playerType.Researcher:
                    bitmapName = "researcherHUD";
                    break;    
            }
            SplashKit.DrawBitmap(SplashKit.BitmapNamed(bitmapName), WINDOW_WIDTH - WINDOW_CARD_BUFFER, 0);
            
        }

        // Get and return a city based on user mouse click
        private City getCityToMoveTo(string message) {
            City citySelected = null;
            while(citySelected == null) {
                drawHUDWarning(message);
                gameWindow.Refresh(60);
                SplashKit.ProcessEvents();
                if(SplashKit.MouseClicked(MouseButton.LeftButton)){
                    Point2D mouseLoc = SplashKit.MousePosition();
                    citySelected = board.isPointACity(mouseLoc);
                } 
                if(SplashKit.KeyTyped(KeyCode.EscapeKey)) {
                    Console.WriteLine("esc pressed");
                    break;
                }
            }

            return citySelected;
        }

        // draws a hud warning with specified text
        private void drawHUDWarning(string message) {
            Rectangle hudRect = SplashKit.RectangleFrom(1096, 418, 200, 145);
            SplashKit.FillRectangle(Color.Black, hudRect);
            SplashKit.DrawText(message, Color.White, "roboto", 10, hudRect.X+5, hudRect.Y+35);
        }

        // shows the players cards on screen
        private void showCards(List<Card> cards) {
            while(true) {
                SplashKit.ProcessEvents();
                displayCards(cards);
                // ways to dismiss the display of cards
                if(SplashKit.MouseClicked(MouseButton.LeftButton) || SplashKit.KeyTyped(KeyCode.ReturnKey) || SplashKit.KeyTyped(KeyCode.SpaceKey) || SplashKit.KeyTyped(KeyCode.Num7Key) || SplashKit.KeyTyped(KeyCode.EscapeKey)) { 
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
                displayCards(player.cardsInHand);
                drawHUDWarning(message);
                if(SplashKit.MouseClicked(MouseButton.LeftButton)) {
                    Point2D mouseLoc = SplashKit.MousePosition();
                    // check to see if the click was in a card
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
                if(SplashKit.KeyTyped(KeyCode.EscapeKey)) {
                    break;
                }
                gameWindow.Refresh(60);
            }
            // needed to silence compiler warnings
            return null;
        }

        // the mechanics behind showing the cards
        private void displayCards(List<Card> cards) {
            double cardSpacing = 15;
            double cardX;
            double cardY = cardSpacing;
            int split = 0;
            for(int i = 0; i < cards.Count; i++) {
                Card card = cards[i];
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
            // keep showing the message until clicked, space, enter, etc..
            while(true) {
                SplashKit.ProcessEvents();
                if(SplashKit.MouseClicked(MouseButton.LeftButton) || SplashKit.KeyTyped(KeyCode.ReturnKey) || SplashKit.KeyTyped(KeyCode.SpaceKey) || SplashKit.KeyTyped(KeyCode.EscapeKey)) { 
                    drawGame(null, null, null);
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
            double xOffset = (WINDOW_WIDTH - SplashKit.BitmapNamed("boardImage").Width - WINDOW_CARD_BUFFER) / 2;
            double yOffset = (WINDOW_HEIGHT - SplashKit.BitmapNamed("boardImage").Height) / 2;
            
            gameWindow.DrawBitmap(SplashKit.BitmapNamed("boardImage"), xOffset, yOffset, SplashKit.OptionScaleBmp(SCALAR, SCALAR));
        }

        // get a chosen player from the user
        private Player getPlayer() {
            Player chosenPlayer = null;
            while(chosenPlayer == null) {
                SplashKit.ProcessEvents();
                drawHUDWarning("Choose player pawn");
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

        // moves the player to a chosen city
        private bool movePlayerToCity(Player playerToMove) {
            City cityToMoveTo = null;
                
            cityToMoveTo = getCityToMoveTo("Click on an adjoining city to select it");
            if(cityToMoveTo != null) {
                if(playerToMove.location.connectedCities.Contains(cityToMoveTo)) {
                    playerToMove.Move(cityToMoveTo);
                    
                    // do the medic check
                    if(playerToMove.type == playerType.Medic) {
                        clearDiseaseIfCured(playerToMove);
                    }
                    return true;
                } else {
                    cityToMoveTo = null;
                }
            }
            return false;
        }

        // returns true if a city card is in the players hand
        private bool doesPlayerHaveCard(Player player) {
            foreach(PlayerCard card in player.cardsInHand) {
                if(compareString(card.city, player.location.name)) {
                    return true;
                }
            }
            return false;
        }

        // allocates the drawing of the disease info
        private void drawDiseaseLevels(List<Disease> diseases) {
            double redRectX = WINDOW_WIDTH - RED_CUBE_COUNTER_OFFSET - WINDOW_CARD_BUFFER;
            double rectY = WINDOW_HEIGHT - CUBE_COUNTER_OFFSET;
            double yellowRectX = redRectX + CUBE_COUNTER_OFFSET;
            double blueRectX = yellowRectX + CUBE_COUNTER_OFFSET;
            
        
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

        // the player draws two city cards at the end of their turn
        private void playerDrawsCityCards(Player currentPlayer, InfectionCard infectionCardToFlip) {
            string[] drawCardMessages = new string[] {"You need to draw two city cards after your actions",
                                                    "I'll display your hand, click the city card pile to draw more cards"};
            for(int i = 0; i < drawCardMessages.Length; i++) {
                SplashKit.ProcessEvents();
                showMessage(drawCardMessages[i]);
                gameWindow.Refresh(60);
            }
            
            int drawnCards = 0;
            Card flippedCard = null;
            Card cityCardToFlip = board.nextPlayerCard;

            drawGame(infectionCardToFlip, cityCardToFlip, currentPlayer);
            displayCards(currentPlayer.cardsInHand);
            gameWindow.Refresh(60);
            
            Rectangle cityCardRect = drawCards(cityCardToFlip, flippedCard)[0];
            
            while(drawnCards < 2) {
                
                SplashKit.ProcessEvents();
                if(SplashKit.MouseClicked(MouseButton.LeftButton)){
                    Point2D mouseLoc = SplashKit.MousePosition();
                    if(SplashKit.PointInRectangle(mouseLoc, cityCardRect)) {
                        flippedCard = cityCardToFlip;
                        flippedCard.isFaceUp = true;
                        if(flippedCard.group == CityGroup.epidemic) {
                            doEpidemic(cityCardToFlip, currentPlayer);
                        } else {
                            currentPlayer.AddCardToHand(flippedCard as PlayerCard);
                        }
                        
                        cityCardToFlip = board.nextPlayerCard;
                        drawGame(infectionCardToFlip, cityCardToFlip, currentPlayer);
                        drawCards(cityCardToFlip, board.lastPlayerCardFlipped);
                        displayCards(currentPlayer.cardsInHand);
                        gameWindow.Refresh(60);
                        drawnCards++;
                    }
                }
            }
        }

        // the player draws more infection cards to infect the board
        private void playerDrawsInfectionCards(Player currentPlayer, Card playerCardToFlip) {
            string messageString = "You now need to infect the new cities. Turn over " + board.currentInfectionRate.ToString() + " infection cards.";
            showMessage(messageString);
            infectBoard(board.currentInfectionRate, playerCardToFlip, currentPlayer);
        }

        // incriments to the next player
        private Player nextPlayer(Player currentPlayer) {
            int index = board.players.IndexOf(currentPlayer) + 1;
            if(index >= board.players.Count) {
                index = 0;
            }
            return board.players[index];
        }

        // runs the epidemic 
        private void doEpidemic(Card playerCardToFlip, Player currentPlayer) {
            showMessage("Oh no, you got an Epidemic!");
            board.increaseInfectionRate();
            string infectionRateMessage = "First, we increase your infection rate. You now flip " + board.currentInfectionRate.ToString() + " cards each turn";
            showMessage(infectionRateMessage);
            showMessage("Next we take the bottom card from the infection pile (click the pile to get it)");
            
            InfectionCard flippedInfectionCard = board.lastInfectionCardFlipped;
            InfectionCard infectionCardToFlip = (InfectionCard)board.lastInfectionCard();
            Rectangle infCardRect = drawCards(infectionCardToFlip, flippedInfectionCard)[0];
            
            drawGame(infectionCardToFlip, playerCardToFlip, currentPlayer);
            
            if(playerClickedInRectangle(infCardRect)) {
                flippedInfectionCard = infectionCardToFlip;
                flippedInfectionCard.isFaceUp = true;
                board.putInfectionCardIntoFlippedPile(flippedInfectionCard);

                infectionCardToFlip = board.nextInfectionCard;
                drawCards(infectionCardToFlip, flippedInfectionCard);
                    
                City cityToInfect = board.getCity(flippedInfectionCard.city);
                Disease currentDisease = board.GetDisease(cityToInfect.type);
                
                // epidemic cities get 3 infection cubes
                showMessage("The Epidemic city gets three infection cubes");

                for(int i = 0; i < 3; i++) {
                    cityToInfect.increaseInfection();
                    currentDisease.useCube(1);
                }

                drawCityInfections();
                
                gameWindow.Refresh(60);

                showMessage("Now we shuffle the drawn infection cards and put them back on the deck");

                board.shuffleAndRestackDrawnInfectionCards();
                drawGame(infectionCardToFlip, playerCardToFlip, currentPlayer);
            }
        }

        // draws the game
        private void drawGame(InfectionCard infectionCardToFlip, Card playerCardToFlip, Player currentPlayer) {
            gameWindow.Clear(Color.White);
            // draw all the board items
            drawBoard();
            if(infectionCardToFlip != null) {
                _ = drawCards(infectionCardToFlip, board.lastInfectionCardFlipped);
            }
            if(playerCardToFlip != null) {
                _ = drawCards(playerCardToFlip, board.lastPlayerCardFlipped);
            }
            
            // ALL BOARD DRAWING NEEDS TO HAPPEN BELOW HERE

            if(currentPlayer != null) {
                drawHUD(currentPlayer);
            }

            drawMarkers();
            drawPlayers(board.players);
            drawCityInfections();
            drawDiseaseLevels(board.diseases);
            
            gameWindow.Refresh(50);
        }

        // checks to see if the current player is the medic, and if the disease is cured
        // the medic clears all cured cubes just by entering a city
        private void clearDiseaseIfCured(Player currentPlayer) {
            // only the medic has this ability
            if(currentPlayer.type != playerType.Medic) {
                return;
            }
            Disease diseaseAtCity = board.GetDisease(currentPlayer.location.type);
            if(diseaseAtCity != null && diseaseAtCity.isCured) {
                currentPlayer.TreatInfection(true);
            }
        }

        // gets the player to discard cards
        private void discarCards(Player currentPlayer) {
            showMessage("You have to discard a card, please choose one to discard.");
            PlayerCard cardToDiscard = playerChosenCard(currentPlayer, "select card to discard");
            if(cardToDiscard != null) {
                currentPlayer.DiscardCard(cardToDiscard);
                    
                board.discardPlayerCard(cardToDiscard);
            }
        }

        // draws the markers
        private void drawMarkers() {
            // infection marker
            double infectionMarkerX = INFECTION_MARKER_X + (board.infectionRateMarkerPosition * INFECTION_MARKER_OFFSET);
            gameWindow.DrawBitmap(SplashKit.BitmapNamed("infectionMarker"), infectionMarkerX, INFECTION_MARKER_Y);

            // outbreak marker
            double outbreakMarkerY = OUTBREAK_MARKER_Y + (board.outbreakTracker * OUTBREAK_MARKER_OFFSET);
            gameWindow.DrawBitmap(SplashKit.BitmapNamed("outbreakMarker"), OUTBREAK_MARKER_X, outbreakMarkerY);
        }
    }
}

    