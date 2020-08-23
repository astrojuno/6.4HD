// Board for the game Pandemic
// John Ryder 219466419

using System;
using System.Collections.Generic;
using System.Linq;

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
            //loadCities();

            // Console.WriteLine("starting test...");
            // City testCity = null;
            // foreach(City city in cities) {
            //     //Console.WriteLine("in first foreach");
            //     if(city.name == "Toronto") {
            //         Console.WriteLine("found city");
            //         testCity = city;
            //     }
            // }
            // foreach(City city in testCity.connectedCities) {
            //     city.increaseInfection();
            //     Console.WriteLine(city.name + " infection increased to " + city.infectionLevel);
            // }
            // foreach(City city in cities) {
            //     if(city.name == "Montreal") {
            //         Console.WriteLine("found NY");
            //         testCity = city;
            //     }
            // }
            // Console.WriteLine("test city connections: " + testCity.connectedCities.Count);
            // foreach(City city in testCity.connectedCities) {
            //     Console.WriteLine(testCity.name + " connected to: " + city.name + " infection: " + city.infectionLevel);
            // }

            // Console.WriteLine("test over");

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
                // InfectionCard card = new InfectionCard(city, CityGroup.blue);
                // _infectionCards.Push(card);
                // SplashKit.DrawBitmap(card.cardImage, 10, 10);
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
            foreach(City city in cities) {
                switch(city.name) {
                    case "Atlanta":
                        foreach(City cityToAssess in cities) {
                            if((cityToAssess.name == "Indianapolis" || cityToAssess.name == "Washington" || cityToAssess.name == "Miami" || cityToAssess.name == "Dallas")) {
                                city.addConnectedCity(cityToAssess);
                            }
                        }
                        break;
                    case "Toronto":
                        foreach(City cityToAssess in cities) {
                            if((cityToAssess.name == "Chicago" || cityToAssess.name == "New York" || cityToAssess.name == "Montreal")) {
                                city.addConnectedCity(cityToAssess);
                            }
                        }
                        break;
                    case "Montreal":
                        foreach(City cityToAssess in cities) {
                            if((cityToAssess.name == "Toronto" || cityToAssess.name == "New York" || cityToAssess.name == "Boston")) {
                                city.addConnectedCity(cityToAssess);
                            }
                        }
                        break;
                    case "Chicago":
                        break;
                    case "Boston":
                        break;
                    case "New York":
                        break;
                    case "Washington":
                        break;
                    case "Indianapolis":
                        break;
                    case "Los Angeles":
                        break;
                    case "Phoenix":
                        break; 
                    case "Minneapolis":
                        break;
                    case "San Francisco":
                        break;
                    case "Seattle":
                        break;
                    case "Calgary":
                        break;
                    case "Denver":
                        break;
                    case "Dallas":
                        break;
                    case "Monterrey":
                        break;
                    case "Guadalajara":
                        break;
                    case "Ciudad De Mexico":
                        break;
                    case "New Orleans":
                        break;
                    case "Tegucigalpa":
                        break;
                    case "Havana":
                        break;
                    case "Miami":
                        break;
                    case "Santo Domingo":
                        break;
                }
            }
        }
    }
}