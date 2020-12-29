using System;

namespace Mtcg.Cards
{
    public class Goblin : AbstractMonster
    {
        public Goblin() : base("Goblin") { }

        // Goblin is too afraid to attack Dragon
        public bool Attack(Dragon opponent)
        {
            Console.WriteLine("My opponent is a Dragon. I'm too afraid to attack!");
            Console.WriteLine("I'm the loser!");
            return false;
        }

    }
}