using System;

namespace Mtcg.Cards
{
    public abstract class AbstractMonster : ICard
    {
        public string Name { get; }
        public int Damage { get; }
        public Element ElementType { get; }


        public AbstractMonster(string name)
        {
            this.Name = name;

            Random myRandom = new Random();

            int randomDamage = myRandom.Next(20, 121); // between 20 and 120
            this.Damage = randomDamage;

            int randomHelper = myRandom.Next(1, 4);
            switch (randomHelper)
            {
                case 1:
                    this.ElementType = Element.Fire;
                    break;
                case 2:
                    this.ElementType = Element.Water;
                    break;
                case 3:
                    this.ElementType = Element.Normal;
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

            Console.WriteLine("My opponent is a Monster"!);

            // if opponent is monster (higher damage wins)
            if (haveWon(this.Damage, opponent.Damage))
            {
                Console.WriteLine("I'm the winner"!);
                return true;
            } else
            {
                Console.WriteLine("I'm the loser"!);
                return false;
            }

        }

        public bool Attack(Spell opponent)
        {

            Console.WriteLine("My opponent is a Spell"!);
            // element is important here

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
            if (  (this.ElementType == Element.Fire && opponent.ElementType == Element.Normal)   ||
                  (this.ElementType == Element.Water && opponent.ElementType == Element.Fire)    ||
                  (this.ElementType == Element.Normal && opponent.ElementType == Element.Water)
               )
            {
                Console.WriteLine("effective"!);

                if (haveWon(this.Damage*2, opponent.Damage))
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

            return false;
        }


    }
}