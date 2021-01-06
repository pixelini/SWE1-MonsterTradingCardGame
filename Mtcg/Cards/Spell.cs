using System;

namespace Mtcg.Cards
{
    public class Spell : ICard
    {
        public string Id { get; }
        public string Name { get; }
        public float Damage { get; }
        public Element ElementType { get; }

        public Spell(string id, string name, float damage, Element elementType)
        {
            this.Id = id;
            this.Name = name;
            this.Damage = damage;
            this.ElementType = elementType;
        }


    }
}
