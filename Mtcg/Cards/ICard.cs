using Mtcg.Cards;

namespace Mtcg
{
    public interface ICard
    {
        string Name { get; }
        int Damage { get; }

        Element ElementType { get; }

        bool Attack(AbstractMonster opponent);
        bool Attack(Spell opponent);



    }
}