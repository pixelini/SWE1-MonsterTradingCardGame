using System;

namespace Mtcg.Cards
{
    public class Dragon : AbstractMonster
    {
        public Dragon() : base("Dragon") { }

        // Goblin is too afraid to attack Dragon
        public bool Attack(Goblin opponent)
        {
            Console.WriteLine("My opponent is a Goblin. He is too afraid to attack!");
            Console.WriteLine("I'm the winner!");
            return true;
        }

        // FireElves can evade attacks of Dragons
        public bool Attack(FireElve opponent)
        {
            Console.WriteLine("My opponent is a FireElve.");
            Console.WriteLine("I'm the loser!");
            return false;
        }

    }
}