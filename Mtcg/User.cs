using System.Collections.Generic;

namespace Mtcg
{
    public class User
    {
        public string Username  { get; set; }
        public string Password { get; set; }
        public Deck Deck { get; set; }
        public int Coins { get; set; }
        public int Stats { get; set; }
        public List<ICard> Stack { get; set; }
        public List<ICard> CardsDuringGame { get; set; }

        public User(string username, string password)
        {
            Username = username;
            Password = password;
        }
    }

}