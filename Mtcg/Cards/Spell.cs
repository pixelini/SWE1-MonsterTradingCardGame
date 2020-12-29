using System;
using System.Collections.Generic;
using System.Text;

namespace Mtcg.Cards
{
    public class Spell : ICard
    {
        public string Name { get; }
        public int Damage { get; }
        public Element ElementType { get; }

        public Spell()
        {
            Random myRandom = new Random();

            int randomDamage = myRandom.Next(20, 121); // between 20 and 120
            this.Damage = randomDamage;

            int randomHelper = myRandom.Next(1, 4);
            switch (randomHelper)
            {
                case 1:
                    this.ElementType = Element.Fire;
                    this.Name = "Firespell";
                    break;
                case 2:
                    this.ElementType = Element.Water;
                    this.Name = "Waterspell";
                    break;
                case 3:
                    this.ElementType = Element.Normal;
                    this.Name = "Normalspell";
                    break;
                default:
                    break;
            }

        }

        public bool haveWon(int myCalcDamage, int opponentDamage)
        {
            return myCalcDamage > opponentDamage;
        }


        public bool Attack(AbstractMonster opponent)
        {

            Console.WriteLine("My opponent is a Monster!");
            // element is important here

            // case: no effect --> both have the same element type 
            if (this.ElementType == opponent.ElementType)
            {
                Console.WriteLine("no effect"!);

                if (haveWon(this.Damage, opponent.Damage))
                {
                    Console.WriteLine("I'm the winner!");
                    return true;
                }
                else
                {
                    Console.WriteLine("I'm the loser!");
                    return true;
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
                    Console.WriteLine("I'm the winner!");
                    return true;
                }
                else
                {
                    Console.WriteLine("I'm the loser!");
                    return false;
                }

            }


            // case: not effective
            if ((this.ElementType == Element.Normal && opponent.ElementType == Element.Fire) ||
                  (this.ElementType == Element.Fire && opponent.ElementType == Element.Water) ||
                  (this.ElementType == Element.Water && opponent.ElementType == Element.Normal)
               )
            {
                Console.WriteLine("not effective!");

                if (haveWon(this.Damage / 2, opponent.Damage))
                {
                    Console.WriteLine("I'm the winner!");
                    return true;
                }
                else
                {
                    Console.WriteLine("I'm the loser!");
                    return false;
                }
            }

            return false;

        }

        public bool Attack(Spell opponent)
        {

            Console.WriteLine("My opponent is a Spell!");
            // element is important here

            // case: no effect --> both have the same element type 
            if (this.ElementType == opponent.ElementType)
            {
                Console.WriteLine("no effect!");

                if (haveWon(this.Damage, opponent.Damage))
                {
                    Console.WriteLine("I'm the winner!");
                    return true;
                }
                else
                {
                    Console.WriteLine("I'm the loser!");
                    return false;
                }

            }


            // case: effective
            if ((this.ElementType == Element.Fire && opponent.ElementType == Element.Normal) ||
                  (this.ElementType == Element.Water && opponent.ElementType == Element.Fire) ||
                  (this.ElementType == Element.Normal && opponent.ElementType == Element.Water)
               )
            {
                Console.WriteLine("effective!");

                if (haveWon(this.Damage * 2, opponent.Damage))
                {
                    Console.WriteLine("I'm the winner!");
                    return true;
                }
                else
                {
                    Console.WriteLine("I'm the loser!");
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
                    Console.WriteLine("I'm the winner!");
                    return true;
                }
                else
                {
                    Console.WriteLine("I'm the loser!");
                    return false;
                }
            }

            return false;
        }



        public bool Attack(Knight opponent)
        {
            Console.WriteLine("My opponent is a Knight!");

            if (this.ElementType == Element.Water)
            {
                Console.WriteLine("I'm the winner!.");
                return true;
            } else
            {

                // case: no effect --> both have the same element type 
            if (this.ElementType == opponent.ElementType)
            {
                Console.WriteLine("no effect!");

                if (haveWon(this.Damage, opponent.Damage))
                {
                    Console.WriteLine("I'm the winner!");
                        return true;
                }
                else
                {
                    Console.WriteLine("I'm the loser!");
                        return false;
                }

            }


            // case: effective
            if ((this.ElementType == Element.Fire && opponent.ElementType == Element.Normal) ||
                  (this.ElementType == Element.Water && opponent.ElementType == Element.Fire) ||
                  (this.ElementType == Element.Normal && opponent.ElementType == Element.Water)
               )
                {
                    Console.WriteLine("effective!");

                    if (haveWon(this.Damage * 2, opponent.Damage))
                    {
                        Console.WriteLine("I'm the winner!");
                        return true;
                    }
                    else
                    {
                        Console.WriteLine("I'm the loser!");
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
                        Console.WriteLine("I'm the winner!");
                        return true;
                    }
                    else
                    {
                        Console.WriteLine("I'm the loser!");
                        return false;
                    }
                }

            }

            return false;
        }



        public bool Attack(Kraken opponent)
        {
            Console.WriteLine("My opponent is a Kraken!");
            Console.WriteLine("I'm the loser."!);
            return false;
        }



    }
}
