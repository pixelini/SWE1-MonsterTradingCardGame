using System;

namespace Mtcg.Cards 
{
    public class Wizzard : AbstractMonster
    {
        public Wizzard() : base("Wizzard") {}

        // Wizzard couldn't be damaged by Orks
        public bool Attack(Ork opponent)
        {
            Console.WriteLine("My opponent is an Ork. He couldn't damage me.");
            Console.WriteLine("I'm the winner!");
            return true;
        }
    }
}
