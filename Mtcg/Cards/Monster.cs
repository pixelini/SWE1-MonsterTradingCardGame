using System;

namespace Mtcg.Cards
{
    public abstract class Monster : ICard
    {
        public string Name { get; }
        public float Damage { get; }
        public Element ElementType { get; }


        public Monster(string name, float damage)
        {
            this.Name = name;
            this.Damage = damage;

            // check if damage is between 20 and 120

            switch (1)
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


    }
}