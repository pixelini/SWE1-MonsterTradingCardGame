using System;
using System.Collections.Generic;

namespace Mtcg
{
    public class User
    {
        public string Username  { get; set; }
        public string Password { get; set; }
        public List<ICard> Deck { get; set; }
        public int Coins { get; set; }
        public Stats Stats { get; set; }
        public List<ICard> Stack { get; set; }
        //public List<ICard> DeckDuringGame { get; set; }

        public User() { }
        public User(string username, string password)
        {
            Username = username;
            Password = password;
        }
        public ICard PickCardFromDeck()
        {
            Random rand = new Random();
            int chosenCard = rand.Next(0, Deck.Count-1);

            return Deck[chosenCard];
        }

        public void AddCardToDeck(ICard card)
        {
            Deck.Add(card);
        }

        public void RemoveCardFromDeck(ICard card)
        {
            Deck.Remove(card);
        }



    }

}