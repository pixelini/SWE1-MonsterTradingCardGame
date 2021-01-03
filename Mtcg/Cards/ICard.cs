using Mtcg.Cards;

namespace Mtcg
{
    public interface ICard
    {
        string Id { get; }
        string Name { get; }
        float Damage { get; }

        Element ElementType { get; }


    }
}