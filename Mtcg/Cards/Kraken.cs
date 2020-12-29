using System;
using System.Collections.Generic;
using System.Text;
using Mtcg.Cards;

namespace Mtcg
{
    public class Kraken : AbstractMonster
    {
        public Kraken() : base("Kraken") { }

        // Kraken is immune against spells
        public bool Attack(Spell opponent)
        {
            Console.WriteLine("My opponent is a Spell.");
            Console.WriteLine("I'm the winner!");
            return true;
        }
    }
}
