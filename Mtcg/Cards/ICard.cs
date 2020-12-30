using Mtcg.Cards;

namespace Mtcg
{
    public interface ICard
    {
        string Name { get; }
        float Damage { get; }

        Element ElementType { get; }


    }
}