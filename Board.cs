// Board for the game Pandemic
// John Ryder 219466419

using System;
using System.Collections.Generic;
using System.Linq;
using SplashKitSDK;

namespace Pandemic {
    public class Board : IObserver<City> {
        // Variables
        private IDisposable cancellation;
        private Stack<Card> _playerCards = new Stack<Card>();
        private Stack<Card> _discardedPlayerCards = new Stack<Card>();
        private Stack<Card> _infectionCards = new Stack<Card>();
        private Stack<Card> _flippedInfectionCards = new Stack<Card>();
        private string[] _blueCities = new string[] {"Atlanta", "Toronto", "Montreal", "Chicago", "Boston", "New York", "Washington", "Indianapolis"};
        private string[] _redCities = new string[] {"Los Angeles", "Phoenix", "Minneapolis", "San Francisco", "Seattle", "Calgary", "Denver", "Dallas"};
        private string[] _yellowCities = new string[] {"Monterrey", "Guadalajara", "Ciudad De Mexico", "New Orleans", "Tegucigalpa", "Havana", "Miami", "Santo Domingo"};
        private List<Disease> _diseases = new List<Disease>();
        private List<Player> _players;
        private List<City> _cities = new List<City>();
        private int[] _infectionRate = new int[] {2, 2, 3, 4};
        private int _infectionRateTracker = 0;
        private int _outbreakTracker = 0;
        public Card nextPlayerCard {  get { return _playerCards.Pop(); } }
        public InfectionCard nextInfectionCard { get { return (InfectionCard)_infectionCards.Pop(); } }
        public List<Player> players { get { return _players; } }
        public bool outOfPlayerCards { get { return _playerCards.Count <= 0; } }
        public bool outOfInfectionCards { get { return _infectionCards.Count <= 0; } }
        public List<City> cities { get { return _cities; } }
        public List<Disease> diseases { get { return _diseases; } }
        public List<Card> flippedInfectionCards { get { return _flippedInfectionCards.ToList(); } }
        public List<Card> discardedPlayerCards { get { return _discardedPlayerCards.ToList(); } }
        public int outbreakTracker { get { return _outbreakTracker; } }
        public Card lastPlayerCardFlipped { get { 
                                                    if(_discardedPlayerCards.Count == 0) {
                                                        return null;
                                                    } else
                                                        return _discardedPlayerCards.Peek(); 
                                                    } 
                                                }
        public InfectionCard lastInfectionCardFlipped { get { 
                                                                if(_flippedInfectionCards.Count == 0) {
                                                                    return null;
                                                                }
                                                                return (InfectionCard)_flippedInfectionCards.Peek(); 
                                                            } 
                                                        }
        public int currentInfectionRate { get { return _infectionRate[_infectionRateTracker]; } }
        public int infectionRateMarkerPosition { get { return _infectionRateTracker + 1; } }
        public bool wonGame { get { 
                                    int cured = 0;
                                    foreach(Disease disease in _diseases) {
                                        if(disease.isCured) {
                                            cured++;
                                        }
                                    }
                                    // 1 taken away for the epidemic group
                                    return cured >= Enum.GetNames(typeof(CityGroup)).Length - 1;
                                }
                            }



        // Constructor
        public Board() {
            createPlayerCards();
            createInfectionCards();
            loadCities();
            createDiseases();
            _players = new List<Player>();
        }

        // Public Methods
        // searches the cities to see if the point is in a city. 
        // returns the city if it finds one, otherwise returns null.
        public City isPointACity(Point2D point) {
            foreach(City city in _cities) {
                if(SplashKit.PointInRectangle(point, city.boardLocation)) {
                    return city;
                }
            }
            return null;
        }

        // gets and returns a city based on a name
        // returns null if no city found
        public City getCity(string cityName) {
            foreach(City city in _cities) {
                if(city.name.ToLower() == cityName.ToLower()) {
                    return city;
                }
            }
            return null;
        }

        // add a player to the game
        public void addPlayer(Player player) {
            _players.Add(player);
        }

        // get a player from the list of players
        public Player GetPlayer(playerType type) {
            foreach(Player player in _players) {
                if(player.type == type) {
                    return player;
                }
            }
            return null;
        }

        // put a card into the flipped infection card pile
        public void putInfectionCardIntoFlippedPile(InfectionCard card) {
            _flippedInfectionCards.Push(card);
        }

        // puts a player card in the discarded pile
        public void discardPlayerCard(Card card) {
            _discardedPlayerCards.Push(card);
            card.isFaceUp = true;
        }

        // transfer a card from one player to another
        public void transferCard(Player from, Player to, PlayerCard card) {
            from.DiscardCard(card);
            to.AddCardToHand(card);
        }

        // return the disease given the type
        public Disease GetDisease(CityGroup type) {
            foreach(Disease disease in _diseases) {
                if(disease.type == type) {
                    return disease;
                }
            }
            return null;
        }

        // increases the infection rate
        public void increaseInfectionRate() {
            _infectionRateTracker++;
            if(_infectionRateTracker >= _infectionRate.Length) {
                _infectionRateTracker = _infectionRate.Length - 1;
            }
        }

        // create the epidemic cards and shuffle them into the deck. 1 per third.
        public void insertEpidemicCards() {
            Random rnd = new Random();
            List<Card> shuffleEpidemic = _playerCards.ToList();
            _playerCards.Clear();
            // create the three pandemic cards and randomly put them in each third
            for(int i = 0; i < 3; i++) {
                Card cardToPush = new EpidemicCard(i);
                int lower = (shuffleEpidemic.Count / 3) * i;
                int upper = (shuffleEpidemic.Count / 3) * (i + 1);
                shuffleEpidemic.Insert(rnd.Next(lower, upper), cardToPush);
            }
            _playerCards = new Stack<Card>(shuffleEpidemic);
        }

        // returns the last infection card. Used in Epidemics
        public Card lastInfectionCard() {
            List<Card> tempCardList = _infectionCards.ToList();
            _infectionCards.Clear();
            Card cardToReturn = tempCardList.Last();
            tempCardList.RemoveAt(tempCardList.Count - 1);
            _infectionCards = new Stack<Card>(tempCardList);
            return cardToReturn;
        }

        // shuffle and restack the drawn infection cards
        public void shuffleAndRestackDrawnInfectionCards() {
            Random rnd = new Random();
            foreach (Card card in _flippedInfectionCards.OrderBy(i => rnd.Next())) {
                card.isFaceUp = false;
                _infectionCards.Push(card);
            }
            _flippedInfectionCards.Clear();
        }

        // Subscriber Methods
        public virtual void Subscribe(City provider) {
            cancellation = provider.Subscribe(this);
        }

        public virtual void Unsubscribe() {
            cancellation.Dispose();
        }

        // needed but not implimented
        public virtual void OnCompleted() {
            
        }
        // These are needed to conform, but we don't need them so no implimentation is
        // actually needed
        public virtual void OnError(Exception e) {

        }

        public virtual void OnNext(City outbreak) {
            _outbreakTracker++;
        }

        // Private Methods
        // create the player card stack
        private void createPlayerCards() {
            foreach(string city in _blueCities) {
                Card cardToPush = new PlayerCard(city, CityGroup.blue);
                _playerCards.Push(cardToPush);
            }
            foreach(string city in _redCities) {
                Card cardToPush = new PlayerCard(city, CityGroup.red);
                _playerCards.Push(cardToPush);
            }
            foreach(string city in _yellowCities) {
                Card cardToPush = new PlayerCard(city, CityGroup.yellow);
                _playerCards.Push(cardToPush);
            }
            // shuffle the cards
            Card[] cardList = _playerCards.ToArray();
            _playerCards.Clear();
            Random rnd = new Random();
            foreach (PlayerCard card in cardList.OrderBy(i => rnd.Next())) {
                _playerCards.Push(card);
            }
        }

        // create the infection card stack
        private void createInfectionCards() {
            foreach(string city in _blueCities) {
                _infectionCards.Push(new InfectionCard(city, CityGroup.blue));
            }
            foreach(string city in _redCities) {
                _infectionCards.Push(new InfectionCard(city, CityGroup.red));
            }
            foreach(string city in _yellowCities) {
                _infectionCards.Push(new InfectionCard(city, CityGroup.yellow));
            }
            // shuffle the cards
            Card[] cardList = _infectionCards.ToArray();
            _infectionCards.Clear();
            Random rnd = new Random();
            foreach (InfectionCard card in cardList.OrderBy(i => rnd.Next())) {
                _infectionCards.Push(card);
            }
        }

        // create the diseases for the game
        private void createDiseases() {
            foreach(CityGroup diseaseColour in Enum.GetValues(typeof(CityGroup))) {
                if(diseaseColour != CityGroup.epidemic) {
                    _diseases.Add(new Disease(diseaseColour));
                }
            }
        }

        // load the cities 
        private void loadCities() {
            foreach(string city in _blueCities) {
                City cityToAdd = new City(city, CityGroup.blue);
                _cities.Add(cityToAdd);
                this.Subscribe(cityToAdd);
            }
            foreach(string city in _redCities) {
                City cityToAdd = new City(city, CityGroup.red);
                _cities.Add(cityToAdd);
                this.Subscribe(cityToAdd);
            }
            foreach(string city in _yellowCities) {
                City cityToAdd = new City(city, CityGroup.yellow);
                _cities.Add(cityToAdd);
                this.Subscribe(cityToAdd);
            }

            // only way I could figure this out was to brute force it...
            Rectangle cityLocationOnBoard = new Rectangle();
            Point2D cityCentre = new Point2D();
            foreach(City city in _cities) {
                switch(city.name) {
                    case "Atlanta":
                        foreach(City cityToAssess in _cities) {
                            if((cityToAssess.name == "Indianapolis" || cityToAssess.name == "Washington" || cityToAssess.name == "Miami" || cityToAssess.name == "Dallas")) {
                                city.addConnectedCity(cityToAssess);
                            }
                        }
                        cityLocationOnBoard.X = 781;
                        cityLocationOnBoard.Y = 394;
                        cityLocationOnBoard.Width = 89;
                        cityLocationOnBoard.Height = 92;
                        cityCentre.X = 821;
                        cityCentre.Y = 426;
                        city.setBoardLocation(cityLocationOnBoard, cityCentre);
                        break;
                    case "Toronto":
                        foreach(City cityToAssess in _cities) {
                            if((cityToAssess.name == "Chicago" || cityToAssess.name == "New York" || cityToAssess.name == "Montreal")) {
                                city.addConnectedCity(cityToAssess);
                            }
                        }
                        cityLocationOnBoard.X = 838;
                        cityLocationOnBoard.Y = 64;
                        cityLocationOnBoard.Width = 98;
                        cityLocationOnBoard.Height = 101;
                        cityCentre.X = 890;
                        cityCentre.Y = 124;
                        city.setBoardLocation(cityLocationOnBoard, cityCentre);
                        break;
                    case "Montreal":
                        foreach(City cityToAssess in _cities) {
                            if((cityToAssess.name == "Toronto" || cityToAssess.name == "New York" || cityToAssess.name == "Boston")) {
                                city.addConnectedCity(cityToAssess);
                            }
                        }
                        cityLocationOnBoard.X = 991;
                        cityLocationOnBoard.Y = 16;
                        cityLocationOnBoard.Width = 103;
                        cityLocationOnBoard.Height = 92;
                        cityCentre.X = 1041;
                        cityCentre.Y = 77;
                        city.setBoardLocation(cityLocationOnBoard, cityCentre);
                        break;
                    case "Chicago":
                        foreach(City cityToAssess in _cities) {
                            if((cityToAssess.name == "Toronto" || cityToAssess.name == "Washington" || cityToAssess.name == "Indianapolis" || cityToAssess.name == "Minneapolis")) {
                                city.addConnectedCity(cityToAssess);
                            }
                        }
                        cityLocationOnBoard.X = 765;
                        cityLocationOnBoard.Y = 190;
                        cityLocationOnBoard.Width = 169;
                        cityLocationOnBoard.Height = 89;
                        cityCentre.X = 812;
                        cityCentre.Y = 228;
                        city.setBoardLocation(cityLocationOnBoard, cityCentre);
                        break;
                    case "Boston":
                        foreach(City cityToAssess in _cities) {
                            if((cityToAssess.name == "Montreal" || cityToAssess.name == "New York")) {
                                city.addConnectedCity(cityToAssess);
                            }
                        }
                        cityLocationOnBoard.X = 1097;
                        cityLocationOnBoard.Y = 109;
                        cityLocationOnBoard.Width = 92;
                        cityLocationOnBoard.Height = 90;
                        cityCentre.X = 1143;
                        cityCentre.Y = 162;
                        city.setBoardLocation(cityLocationOnBoard, cityCentre);
                        break;
                    case "New York":
                        foreach(City cityToAssess in _cities) {
                            if((cityToAssess.name == "Boston" || cityToAssess.name == "Montreal" || cityToAssess.name == "Toronto" || cityToAssess.name == "Washington")) {
                                city.addConnectedCity(cityToAssess);
                            }
                        }
                        cityLocationOnBoard.X = 1002;
                        cityLocationOnBoard.Y = 209;
                        cityLocationOnBoard.Width = 174;
                        cityLocationOnBoard.Height = 70;
                        cityCentre.X = 1049;
                        cityCentre.Y = 245;
                        city.setBoardLocation(cityLocationOnBoard, cityCentre);
                        break;
                    case "Washington":
                        foreach(City cityToAssess in _cities) {
                            if((cityToAssess.name == "New York" || cityToAssess.name == "Chicago" || cityToAssess.name == "Atlanta")) {
                                city.addConnectedCity(cityToAssess);
                            }
                        }
                        cityLocationOnBoard.X = 978;
                        cityLocationOnBoard.Y = 335;
                        cityLocationOnBoard.Width = 202;
                        cityLocationOnBoard.Height = 70;
                        cityCentre.X = 1019;
                        cityCentre.Y = 369;
                        city.setBoardLocation(cityLocationOnBoard, cityCentre);
                        break;
                    case "Indianapolis":
                        foreach(City cityToAssess in _cities) {
                            if((cityToAssess.name == "Chicago" || cityToAssess.name == "Atlanta" || cityToAssess.name == "Dallas")) {
                                city.addConnectedCity(cityToAssess);
                            }
                        }
                        cityLocationOnBoard.X = 669;
                        cityLocationOnBoard.Y = 304;
                        cityLocationOnBoard.Width = 202;
                        cityLocationOnBoard.Height = 70;
                        cityCentre.X = 714;
                        cityCentre.Y = 340;
                        city.setBoardLocation(cityLocationOnBoard, cityCentre);
                        break;
                    case "Los Angeles":
                        foreach(City cityToAssess in _cities) {
                            if((cityToAssess.name == "San Francisco" || cityToAssess.name == "Phoenix" || cityToAssess.name == "Guadalajara")) {
                                city.addConnectedCity(cityToAssess);
                            }
                        }
                        cityLocationOnBoard.X = 51;
                        cityLocationOnBoard.Y = 438;
                        cityLocationOnBoard.Width = 170;
                        cityLocationOnBoard.Height = 81;
                        cityCentre.X = 184;
                        cityCentre.Y = 478;
                        city.setBoardLocation(cityLocationOnBoard, cityCentre);
                        break;
                    case "Phoenix":
                        foreach(City cityToAssess in _cities) {
                            if((cityToAssess.name == "Denver" || cityToAssess.name == "Dallas" || cityToAssess.name == "Monterrey" || cityToAssess.name == "Los Angeles")) {
                                city.addConnectedCity(cityToAssess);
                            }
                        }
                        cityLocationOnBoard.X = 337;
                        cityLocationOnBoard.Y = 411;
                        cityLocationOnBoard.Width = 141;
                        cityLocationOnBoard.Height = 74;
                        cityCentre.X = 378;
                        cityCentre.Y = 447;
                        city.setBoardLocation(cityLocationOnBoard, cityCentre);
                        break; 
                    case "Minneapolis":
                        foreach(City cityToAssess in _cities) {
                            if((cityToAssess.name == "Calgary" || cityToAssess.name == "Denver" || cityToAssess.name == "Chicago")) {
                                city.addConnectedCity(cityToAssess);
                            }
                        }
                        cityLocationOnBoard.X = 603;
                        cityLocationOnBoard.Y = 151;
                        cityLocationOnBoard.Width = 136;
                        cityLocationOnBoard.Height = 85;
                        cityCentre.X = 640;
                        cityCentre.Y = 205;
                        city.setBoardLocation(cityLocationOnBoard, cityCentre);
                        break;
                    case "San Francisco":
                        foreach(City cityToAssess in _cities) {
                            if((cityToAssess.name == "Seattle" || cityToAssess.name == "Denver" || cityToAssess.name == "Los Angeles")) {
                                city.addConnectedCity(cityToAssess);
                            }
                        }
                        cityLocationOnBoard.X = 124;
                        cityLocationOnBoard.Y = 304;
                        cityLocationOnBoard.Width = 192;
                        cityLocationOnBoard.Height = 72;
                        cityCentre.X = 279;
                        cityCentre.Y = 336;
                        city.setBoardLocation(cityLocationOnBoard, cityCentre);
                        break;
                    case "Seattle":
                        foreach(City cityToAssess in _cities) {
                            if((cityToAssess.name == "Calgary" || cityToAssess.name == "San Francisco")) {
                                city.addConnectedCity(cityToAssess);
                            }
                        }
                        cityLocationOnBoard.X = 244;
                        cityLocationOnBoard.Y = 152;
                        cityLocationOnBoard.Width = 140;
                        cityLocationOnBoard.Height = 75;
                        cityCentre.X = 286;
                        cityCentre.Y = 189;
                        city.setBoardLocation(cityLocationOnBoard, cityCentre);
                        break;
                    case "Calgary":
                        foreach(City cityToAssess in _cities) {
                            if((cityToAssess.name == "Seattle" || cityToAssess.name == "Denver" || cityToAssess.name == "Minneapolis")) {
                                city.addConnectedCity(cityToAssess);
                            }
                        }
                        cityLocationOnBoard.X = 389;
                        cityLocationOnBoard.Y = 102;
                        cityLocationOnBoard.Width = 145;
                        cityLocationOnBoard.Height = 71;
                        cityCentre.X = 430;
                        cityCentre.Y = 139;
                        city.setBoardLocation(cityLocationOnBoard, cityCentre);
                        break;
                    case "Denver":
                        foreach(City cityToAssess in _cities) {
                            if((cityToAssess.name == "Calgary" || cityToAssess.name == "Minneapolis" || cityToAssess.name == "Phoenix" || cityToAssess.name == "San Francisco")) {
                                city.addConnectedCity(cityToAssess);
                            }
                        }
                        cityLocationOnBoard.X = 457;
                        cityLocationOnBoard.Y = 280;
                        cityLocationOnBoard.Width = 139;
                        cityLocationOnBoard.Height = 74;
                        cityCentre.X = 496;
                        cityCentre.Y = 320;
                        city.setBoardLocation(cityLocationOnBoard, cityCentre);
                        break;
                    case "Dallas":
                        foreach(City cityToAssess in _cities) {
                            if((cityToAssess.name == "Phoenix" || cityToAssess.name == "Indianapolis" || cityToAssess.name == "Atlanta" || cityToAssess.name == "New Orleans")) {
                                city.addConnectedCity(cityToAssess);
                            }
                        }
                        cityLocationOnBoard.X = 518;
                        cityLocationOnBoard.Y = 377;
                        cityLocationOnBoard.Width = 84;
                        cityLocationOnBoard.Height = 100;
                        cityCentre.X = 556;
                        cityCentre.Y = 435;
                        city.setBoardLocation(cityLocationOnBoard, cityCentre);
                        break;
                    case "Monterrey":
                        foreach(City cityToAssess in _cities) {
                            if((cityToAssess.name == "Phoenix" || cityToAssess.name == "New Orleans" || cityToAssess.name == "Ciudad De Mexico" || cityToAssess.name == "Guadalajara")) {
                                city.addConnectedCity(cityToAssess);
                            }
                        }
                        cityLocationOnBoard.X = 450;
                        cityLocationOnBoard.Y = 547;
                        cityLocationOnBoard.Width = 167;
                        cityLocationOnBoard.Height = 83;
                        cityCentre.X = 487;
                        cityCentre.Y = 587;
                        city.setBoardLocation(cityLocationOnBoard, cityCentre);
                        break;
                    case "Guadalajara":
                        foreach(City cityToAssess in _cities) {
                            if((cityToAssess.name == "Ciudad De Mexico" || cityToAssess.name == "Monterrey" || cityToAssess.name == "Los Angeles")) {
                                city.addConnectedCity(cityToAssess);
                            }
                        }
                        cityLocationOnBoard.X = 391;
                        cityLocationOnBoard.Y = 718;
                        cityLocationOnBoard.Width = 118;
                        cityLocationOnBoard.Height = 108;
                        cityCentre.X = 454;
                        cityCentre.Y = 763;
                        city.setBoardLocation(cityLocationOnBoard, cityCentre);
                        break;
                    case "Ciudad De Mexico":
                        foreach(City cityToAssess in _cities) {
                            if((cityToAssess.name == "Guadalajara" || cityToAssess.name == "Monterrey" || cityToAssess.name == "Tegucigalpa" || cityToAssess.name == "Havana")) {
                                city.addConnectedCity(cityToAssess);
                            }
                        }
                        cityLocationOnBoard.X = 555;
                        cityLocationOnBoard.Y = 720;
                        cityLocationOnBoard.Width = 149;
                        cityLocationOnBoard.Height = 100;
                        cityCentre.X = 611;
                        cityCentre.Y = 759;
                        city.setBoardLocation(cityLocationOnBoard, cityCentre);
                        break;
                    case "New Orleans":
                        foreach(City cityToAssess in _cities) {
                            if((cityToAssess.name == "Dallas" || cityToAssess.name == "Monterrey" || cityToAssess.name == "Miami")) {
                                city.addConnectedCity(cityToAssess);
                            }
                        }
                        cityLocationOnBoard.X = 669;
                        cityLocationOnBoard.Y = 545;
                        cityLocationOnBoard.Width = 198;
                        cityLocationOnBoard.Height = 83;
                        cityCentre.X = 709;
                        cityCentre.Y = 587;
                        city.setBoardLocation(cityLocationOnBoard, cityCentre);
                        break;
                    case "Tegucigalpa":
                        foreach(City cityToAssess in _cities) {
                            if((cityToAssess.name == "Havana" || cityToAssess.name == "Ciudad De Mexico")) {
                                city.addConnectedCity(cityToAssess);
                            }
                        }
                        cityLocationOnBoard.X = 763;
                        cityLocationOnBoard.Y = 771;
                        cityLocationOnBoard.Width = 208;
                        cityLocationOnBoard.Height = 79;
                        cityCentre.X = 810;
                        cityCentre.Y = 807;
                        city.setBoardLocation(cityLocationOnBoard, cityCentre);
                        break;
                    case "Havana":
                        foreach(City cityToAssess in _cities) {
                            if((cityToAssess.name == "Miami" || cityToAssess.name == "Ciudad De Mexico" || cityToAssess.name == "Tegucigalpa" || cityToAssess.name == "Santo Domingo")) {
                                city.addConnectedCity(cityToAssess);
                            }
                        }
                        cityLocationOnBoard.X = 853;
                        cityLocationOnBoard.Y = 649;
                        cityLocationOnBoard.Width = 153;
                        cityLocationOnBoard.Height = 78;
                        cityCentre.X = 895;
                        cityCentre.Y = 689;
                        city.setBoardLocation(cityLocationOnBoard, cityCentre);
                        break;
                    case "Miami":
                        foreach(City cityToAssess in _cities) {
                            if((cityToAssess.name == "Atlanta" || cityToAssess.name == "New Orleans" || cityToAssess.name == "Havana")) {
                                city.addConnectedCity(cityToAssess);
                            }
                        }
                        cityLocationOnBoard.X = 961;
                        cityLocationOnBoard.Y = 539;
                        cityLocationOnBoard.Width = 143;
                        cityLocationOnBoard.Height = 80;
                        cityCentre.X = 1010;
                        cityCentre.Y = 582;
                        city.setBoardLocation(cityLocationOnBoard, cityCentre);
                        break;
                    case "Santo Domingo":
                        foreach(City cityToAssess in _cities) {
                            if((cityToAssess.name == "Havana")) {
                                city.addConnectedCity(cityToAssess);
                            }
                        }
                        cityLocationOnBoard.X = 1097;
                        cityLocationOnBoard.Y = 629;
                        cityLocationOnBoard.Width = 146;
                        cityLocationOnBoard.Height = 97;
                        cityCentre.X = 1167;
                        cityCentre.Y = 668;
                        city.setBoardLocation(cityLocationOnBoard, cityCentre);
                        break;
                }
            }
        }
    }
}