﻿using System.Collections.Generic;

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
        public List<ICard> DeckDuringGame { get; set; }

        public User() { }
        public User(string username, string password)
        {
            Username = username;
            Password = password;
        }

        
    }

}