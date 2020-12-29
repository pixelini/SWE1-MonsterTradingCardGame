using System;

namespace Mtcg.Cards
{
    public class FireElve : AbstractMonster
    {
        public FireElve() : base("FireElve") { }

        // FireElves can evade attacks of Dragons
        public bool Attack(Dragon opponent)
        {
            Console.WriteLine("My opponent is a Dragon.");
            Console.WriteLine("I'm the winner!");
            return true;
        }
    }
}
