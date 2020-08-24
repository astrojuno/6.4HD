// Board for the game Pandemic
// John Ryder 219466419

using System;
using System.Collections.Generic;
using System.Linq;
using SplashKitSDK;

namespace Pandemic {
    public class Board {
        // Variables
        private Stack<PlayerCard> _playerCards = new Stack<PlayerCard>();
        private Stack<InfectionCard> _infectionCards = new Stack<InfectionCard>();
        private string[] _blueCities = new string[] {"Atlanta", "Toronto", "Montreal", "Chicago", "Boston", "New York", "Washington", "Indianapolis"};
        private string[] _redCities = new string[] {"Los Angeles", "Phoenix", "Minneapolis", "San Francisco", "Seattle", "Calgary", "Denver", "Dallas"};
        private string[] _yellowCities = new string[] {"Monterrey", "Guadalajara", "Ciudad De Mexico", "New Orleans", "Tegucigalpa", "Havana", "Miami", "Santo Domingo"};
        private List<City> _cities = new List<City>();
        public PlayerCard nextPlayerCard { get { return _playerCards.Pop(); } }
        public InfectionCard nextInfectionCard { get { return _infectionCards.Pop(); } }


        // Constructor
        public Board() {
            createPlayerCards();
            createInfectionCards();
            loadCities();
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

        // public void drawRects() {
        //     foreach(City city in _cities) {
        //         SplashKit.DrawRectangle(Color.BrightGreen, city.boardLocation);
        //     }
        // }

        // Private Methods
        // create the player card stack
        private void createPlayerCards() {
            foreach(string city in _blueCities) {
                _playerCards.Push(new PlayerCard(city, CityGroup.blue));
            }
            foreach(string city in _redCities) {
                _playerCards.Push(new PlayerCard(city, CityGroup.red));
            }
            foreach(string city in _yellowCities) {
                _playerCards.Push(new PlayerCard(city, CityGroup.yellow));
            }
            // shuffle the cards
            PlayerCard[] cardList = _playerCards.ToArray();
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
            InfectionCard[] cardList = _infectionCards.ToArray();
            _infectionCards.Clear();
            Random rnd = new Random();
            foreach (InfectionCard card in cardList.OrderBy(i => rnd.Next())) {
                _infectionCards.Push(card);
            }
        }

        // load the cities 
        private void loadCities() {
            foreach(string city in _blueCities) {
                _cities.Add(new City(city, CityGroup.blue));
            }
            foreach(string city in _redCities) {
                _cities.Add(new City(city, CityGroup.red));
            }
            foreach(string city in _yellowCities) {
                _cities.Add(new City(city, CityGroup.yellow));
            }

            // only way I could figure this out was to brute force it...
            Rectangle cityLocationOnBoard = new Rectangle();
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
                        city.setBoardLocation(cityLocationOnBoard);
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
                        city.setBoardLocation(cityLocationOnBoard);
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
                        city.setBoardLocation(cityLocationOnBoard);
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
                        city.setBoardLocation(cityLocationOnBoard);
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
                        city.setBoardLocation(cityLocationOnBoard);
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
                        city.setBoardLocation(cityLocationOnBoard);
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
                        city.setBoardLocation(cityLocationOnBoard);
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
                        city.setBoardLocation(cityLocationOnBoard);
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
                        city.setBoardLocation(cityLocationOnBoard);
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
                        city.setBoardLocation(cityLocationOnBoard);
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
                        city.setBoardLocation(cityLocationOnBoard);
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
                        city.setBoardLocation(cityLocationOnBoard);
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
                        city.setBoardLocation(cityLocationOnBoard);
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
                        city.setBoardLocation(cityLocationOnBoard);
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
                        city.setBoardLocation(cityLocationOnBoard);
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
                        city.setBoardLocation(cityLocationOnBoard);
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
                        city.setBoardLocation(cityLocationOnBoard);
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
                        city.setBoardLocation(cityLocationOnBoard);
                        break;
                    case "Ciudad De Mexico":
                        foreach(City cityToAssess in _cities) {
                            if((cityToAssess.name == "Guadalajara" || cityToAssess.name == "Monterrey" || cityToAssess.name == "Tegucigalpa")) {
                                city.addConnectedCity(cityToAssess);
                            }
                        }
                        cityLocationOnBoard.X = 555;
                        cityLocationOnBoard.Y = 720;
                        cityLocationOnBoard.Width = 149;
                        cityLocationOnBoard.Height = 100;
                        city.setBoardLocation(cityLocationOnBoard);
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
                        city.setBoardLocation(cityLocationOnBoard);
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
                        city.setBoardLocation(cityLocationOnBoard);
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
                        city.setBoardLocation(cityLocationOnBoard);
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
                        city.setBoardLocation(cityLocationOnBoard);
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
                        city.setBoardLocation(cityLocationOnBoard);
                        break;
                }
            }
        }
    }
}