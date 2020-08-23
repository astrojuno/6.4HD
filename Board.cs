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
        private List<City> cities = new List<City>();
        public PlayerCard nextPlayerCard { get { return _playerCards.Pop(); } }
        public InfectionCard nextInfectionCard { get { return _infectionCards.Pop(); } }

        // Constructor
        public Board() {
            createPlayerCards();
            createInfectionCards();
            loadCities();
        }

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
                cities.Add(new City(city, CityGroup.blue));
            }
            foreach(string city in _redCities) {
                cities.Add(new City(city, CityGroup.red));
            }
            foreach(string city in _yellowCities) {
                cities.Add(new City(city, CityGroup.yellow));
            }

            // only way I could figure this out was to brute force it...
            Point2D cityLocationOnBoard = new Point2D();
            foreach(City city in cities) {
                switch(city.name) {
                    case "Atlanta":
                        foreach(City cityToAssess in cities) {
                            if((cityToAssess.name == "Indianapolis" || cityToAssess.name == "Washington" || cityToAssess.name == "Miami" || cityToAssess.name == "Dallas")) {
                                city.addConnectedCity(cityToAssess);
                            }
                        }
                        cityLocationOnBoard.X = 820;
                        cityLocationOnBoard.Y = 428;
                        city.setBoardLocation(cityLocationOnBoard);
                        break;
                    case "Toronto":
                        foreach(City cityToAssess in cities) {
                            if((cityToAssess.name == "Chicago" || cityToAssess.name == "New York" || cityToAssess.name == "Montreal")) {
                                city.addConnectedCity(cityToAssess);
                            }
                        }
                        cityLocationOnBoard.X = 890;
                        cityLocationOnBoard.Y = 124;
                        city.setBoardLocation(cityLocationOnBoard);
                        break;
                    case "Montreal":
                        foreach(City cityToAssess in cities) {
                            if((cityToAssess.name == "Toronto" || cityToAssess.name == "New York" || cityToAssess.name == "Boston")) {
                                city.addConnectedCity(cityToAssess);
                            }
                        }
                        cityLocationOnBoard.X = 1042;
                        cityLocationOnBoard.Y = 76;
                        city.setBoardLocation(cityLocationOnBoard);
                        break;
                    case "Chicago":
                        foreach(City cityToAssess in cities) {
                            if((cityToAssess.name == "Toronto" || cityToAssess.name == "Washington" || cityToAssess.name == "Indianapolis" || cityToAssess.name == "Minneapolis")) {
                                city.addConnectedCity(cityToAssess);
                            }
                        }
                        cityLocationOnBoard.X = 810;
                        cityLocationOnBoard.Y = 230;
                        city.setBoardLocation(cityLocationOnBoard);
                        break;
                    case "Boston":
                        foreach(City cityToAssess in cities) {
                            if((cityToAssess.name == "Montreal" || cityToAssess.name == "New York")) {
                                city.addConnectedCity(cityToAssess);
                            }
                        }
                        cityLocationOnBoard.X = 1144;
                        cityLocationOnBoard.Y = 162;
                        city.setBoardLocation(cityLocationOnBoard);
                        break;
                    case "New York":
                        foreach(City cityToAssess in cities) {
                            if((cityToAssess.name == "Boston" || cityToAssess.name == "Montreal" || cityToAssess.name == "Toronto" || cityToAssess.name == "Washington")) {
                                city.addConnectedCity(cityToAssess);
                            }
                        }
                        cityLocationOnBoard.X = 1047;
                        cityLocationOnBoard.Y = 243;
                        city.setBoardLocation(cityLocationOnBoard);
                        break;
                    case "Washington":
                        foreach(City cityToAssess in cities) {
                            if((cityToAssess.name == "New York" || cityToAssess.name == "Chicago" || cityToAssess.name == "Atlanta")) {
                                city.addConnectedCity(cityToAssess);
                            }
                        }
                        cityLocationOnBoard.X = 1020;
                        cityLocationOnBoard.Y = 368;
                        city.setBoardLocation(cityLocationOnBoard);
                        break;
                    case "Indianapolis":
                        foreach(City cityToAssess in cities) {
                            if((cityToAssess.name == "Chicago" || cityToAssess.name == "Atlanta" || cityToAssess.name == "Dallas")) {
                                city.addConnectedCity(cityToAssess);
                            }
                        }
                        cityLocationOnBoard.X = 714;
                        cityLocationOnBoard.Y = 340;
                        city.setBoardLocation(cityLocationOnBoard);
                        break;
                    case "Los Angeles":
                        foreach(City cityToAssess in cities) {
                            if((cityToAssess.name == "San Francisco" || cityToAssess.name == "Phoenix" || cityToAssess.name == "Guadalajara")) {
                                city.addConnectedCity(cityToAssess);
                            }
                        }
                        cityLocationOnBoard.X = 186;
                        cityLocationOnBoard.Y = 479;
                        city.setBoardLocation(cityLocationOnBoard);
                        break;
                    case "Phoenix":
                        foreach(City cityToAssess in cities) {
                            if((cityToAssess.name == "Denver" || cityToAssess.name == "Dallas" || cityToAssess.name == "Monterrey" || cityToAssess.name == "Los Angeles")) {
                                city.addConnectedCity(cityToAssess);
                            }
                        }
                        cityLocationOnBoard.X = 378;
                        cityLocationOnBoard.Y = 446;
                        city.setBoardLocation(cityLocationOnBoard);
                        break; 
                    case "Minneapolis":
                        foreach(City cityToAssess in cities) {
                            if((cityToAssess.name == "Calgary" || cityToAssess.name == "Denver" || cityToAssess.name == "Chicago")) {
                                city.addConnectedCity(cityToAssess);
                            }
                        }
                        cityLocationOnBoard.X = 640;
                        cityLocationOnBoard.Y = 203;
                        city.setBoardLocation(cityLocationOnBoard);
                        break;
                    case "San Francisco":
                        foreach(City cityToAssess in cities) {
                            if((cityToAssess.name == "Seattle" || cityToAssess.name == "Denver" || cityToAssess.name == "Los Angeles")) {
                                city.addConnectedCity(cityToAssess);
                            }
                        }
                        cityLocationOnBoard.X = 279;
                        cityLocationOnBoard.Y = 336;
                        city.setBoardLocation(cityLocationOnBoard);
                        break;
                    case "Seattle":
                        foreach(City cityToAssess in cities) {
                            if((cityToAssess.name == "Calgary" || cityToAssess.name == "San Francisco")) {
                                city.addConnectedCity(cityToAssess);
                            }
                        }
                        cityLocationOnBoard.X = 285;
                        cityLocationOnBoard.Y = 188;
                        city.setBoardLocation(cityLocationOnBoard);
                        break;
                    case "Calgary":
                        foreach(City cityToAssess in cities) {
                            if((cityToAssess.name == "Seattle" || cityToAssess.name == "Denver" || cityToAssess.name == "Minneapolis")) {
                                city.addConnectedCity(cityToAssess);
                            }
                        }
                        cityLocationOnBoard.X = 431;
                        cityLocationOnBoard.Y = 139;
                        city.setBoardLocation(cityLocationOnBoard);
                        break;
                    case "Denver":
                        foreach(City cityToAssess in cities) {
                            if((cityToAssess.name == "Calgary" || cityToAssess.name == "Minneapolis" || cityToAssess.name == "Phoenix" || cityToAssess.name == "San Francisco")) {
                                city.addConnectedCity(cityToAssess);
                            }
                        }
                        cityLocationOnBoard.X = 496;
                        cityLocationOnBoard.Y = 320;
                        city.setBoardLocation(cityLocationOnBoard);
                        break;
                    case "Dallas":
                        foreach(City cityToAssess in cities) {
                            if((cityToAssess.name == "Phoenix" || cityToAssess.name == "Indianapolis" || cityToAssess.name == "Atlanta" || cityToAssess.name == "New Orleans")) {
                                city.addConnectedCity(cityToAssess);
                            }
                        }
                        cityLocationOnBoard.X = 556;
                        cityLocationOnBoard.Y = 435;
                        city.setBoardLocation(cityLocationOnBoard);
                        break;
                    case "Monterrey":
                        foreach(City cityToAssess in cities) {
                            if((cityToAssess.name == "Phoenix" || cityToAssess.name == "New Orleans" || cityToAssess.name == "Ciudad De Mexico" || cityToAssess.name == "Guadalajara")) {
                                city.addConnectedCity(cityToAssess);
                            }
                        }
                        cityLocationOnBoard.X = 487;
                        cityLocationOnBoard.Y = 588;
                        city.setBoardLocation(cityLocationOnBoard);
                        break;
                    case "Guadalajara":
                        foreach(City cityToAssess in cities) {
                            if((cityToAssess.name == "Ciudad De Mexico" || cityToAssess.name == "Monterrey" || cityToAssess.name == "Los Angeles")) {
                                city.addConnectedCity(cityToAssess);
                            }
                        }
                        cityLocationOnBoard.X = 454;
                        cityLocationOnBoard.Y = 762;
                        city.setBoardLocation(cityLocationOnBoard);
                        break;
                    case "Ciudad De Mexico":
                        foreach(City cityToAssess in cities) {
                            if((cityToAssess.name == "Guadalajara" || cityToAssess.name == "Monterrey" || cityToAssess.name == "Tegucigalpa")) {
                                city.addConnectedCity(cityToAssess);
                            }
                        }
                        cityLocationOnBoard.X = 611;
                        cityLocationOnBoard.Y = 760;
                        city.setBoardLocation(cityLocationOnBoard);
                        break;
                    case "New Orleans":
                        foreach(City cityToAssess in cities) {
                            if((cityToAssess.name == "Dallas" || cityToAssess.name == "Monterrey" || cityToAssess.name == "Miami")) {
                                city.addConnectedCity(cityToAssess);
                            }
                        }
                        cityLocationOnBoard.X = 707;
                        cityLocationOnBoard.Y = 584;
                        city.setBoardLocation(cityLocationOnBoard);
                        break;
                    case "Tegucigalpa":
                        foreach(City cityToAssess in cities) {
                            if((cityToAssess.name == "Havana" || cityToAssess.name == "Ciudad De Mexico")) {
                                city.addConnectedCity(cityToAssess);
                            }
                        }
                        cityLocationOnBoard.X = 810;
                        cityLocationOnBoard.Y = 805;
                        city.setBoardLocation(cityLocationOnBoard);
                        break;
                    case "Havana":
                        foreach(City cityToAssess in cities) {
                            if((cityToAssess.name == "Miami" || cityToAssess.name == "Ciudad De Mexico" || cityToAssess.name == "Tegucigalpa" || cityToAssess.name == "Santo Domingo")) {
                                city.addConnectedCity(cityToAssess);
                            }
                        }
                        cityLocationOnBoard.X = 894;
                        cityLocationOnBoard.Y = 689;
                        city.setBoardLocation(cityLocationOnBoard);
                        break;
                    case "Miami":
                        foreach(City cityToAssess in cities) {
                            if((cityToAssess.name == "Atlanta" || cityToAssess.name == "New Orleans" || cityToAssess.name == "Havana")) {
                                city.addConnectedCity(cityToAssess);
                            }
                        }
                        cityLocationOnBoard.X = 1008;
                        cityLocationOnBoard.Y = 580;
                        city.setBoardLocation(cityLocationOnBoard);
                        break;
                    case "Santo Domingo":
                        foreach(City cityToAssess in cities) {
                            if((cityToAssess.name == "Havana")) {
                                city.addConnectedCity(cityToAssess);
                            }
                        }
                        cityLocationOnBoard.X = 1167;
                        cityLocationOnBoard.Y = 667;
                        city.setBoardLocation(cityLocationOnBoard);
                        break;
                }
            }
        }
    }
}