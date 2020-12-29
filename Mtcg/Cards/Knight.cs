using System;

namespace Mtcg.Cards
{
    public class Knight : AbstractMonster
    {
        public Knight() : base("Knight") { }

        // WaterSpells make me drown
        public bool Attack(Spell opponent)
        {
            Console.WriteLine("My opponent is a Spell. What kind of Spell?");

            // Special case is only  the water spell

            if (opponent.ElementType == Element.Water)
            {
                Console.WriteLine("I'm the looser");
                return false;
            } else
            {

                // case: no effect --> both have the same element type 
                if (this.ElementType == opponent.ElementType)
                {
                    Console.WriteLine("no effect"!);

                    if (haveWon(this.Damage, opponent.Damage))
                    {
                        Console.WriteLine("I'm the winner"!);
                        return true;
                    }
                    else
                    {
                        Console.WriteLine("I'm the loser"!);
                        return false;
                    }

                }


                // case: effective
                if ((this.ElementType == Element.Fire && opponent.ElementType == Element.Normal) ||
                      (this.ElementType == Element.Water && opponent.ElementType == Element.Fire) ||
                      (this.ElementType == Element.Normal && opponent.ElementType == Element.Water)
                   )
                {
                    Console.WriteLine("effective"!);

                    if (haveWon(this.Damage * 2, opponent.Damage))
                    {
                        Console.WriteLine("I'm the winner"!);
                        return true;
                    }
                    else
                    {
                        Console.WriteLine("I'm the loser"!);
                        return false;
                    }

                }


                // case: not effective
                if ((this.ElementType == Element.Normal && opponent.ElementType == Element.Fire) ||
                      (this.ElementType == Element.Fire && opponent.ElementType == Element.Water) ||
                      (this.ElementType == Element.Water && opponent.ElementType == Element.Normal)
                   )
                {
                    Console.WriteLine("not effective"!);

                    if (haveWon(this.Damage / 2, opponent.Damage))
                    {
                        Console.WriteLine("I'm the winner"!);
                        return true;
                    }
                    else
                    {
                        Console.WriteLine("I'm the loser"!);
                        return false;
                    }
                }


            }

            return false;


        }




    }
}
