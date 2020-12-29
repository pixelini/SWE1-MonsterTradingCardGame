using System;

namespace Mtcg.Cards
{
    public class Ork : AbstractMonster
    {
        public Ork() : base("Ork") { }

        // Wizzard couldn't be damaged by Orks
        public bool Attack(Wizzard opponent)
        {
            Console.WriteLine("My opponent is a Wizzard. I can't damage him.");
            Console.WriteLine("I'm the loser!");
            return false;
        }
    }
}
