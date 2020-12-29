using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using NUnit.Framework;
using Mtcg;
using Mtcg.Cards;

namespace UnitTest
{
    public class FightTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void GoblinAttacksDragon()
        {
            // Arrange
            Goblin goblin = new Goblin();
            Dragon dragon = new Dragon();

            // Act
            bool attackSuccessful = goblin.Attack(dragon);

            // Assert
            Assert.IsFalse(attackSuccessful);
        }

        [Test]
        public void DragonAttacksGoblin()
        {
            // Arrange
            Goblin goblin = new Goblin();
            Dragon dragon = new Dragon();

            // Act
            bool attackSuccessful = dragon.Attack(goblin);

            // Assert
            Assert.IsTrue(attackSuccessful);
        }

        [Test]
        public void WizzardAttacksOrk()
        {
            // Arrange
            Wizzard wizzard = new Wizzard();
            Ork ork = new Ork();

            // Act
            bool attackSuccessful = wizzard.Attack(ork);

            // Assert
            Assert.IsTrue(attackSuccessful);
        }

        [Test]
        public void OrkAttacksWizzard()
        {
            // Arrange
            Wizzard wizzard = new Wizzard();
            Ork ork = new Ork();

            // Act
            bool attackSuccessful = ork.Attack(wizzard);

            // Assert
            Assert.IsFalse(attackSuccessful);
        }

        [Test]
        public void KnightAttacksWaterspell()
        {
            // Arrange
            Knight knight = new Knight();
            Spell spell = new Spell();

            // Act
            bool attackSuccessful = knight.Attack(spell);

            if (spell.ElementType == Element.Water)
            {
                // Assert
                Assert.IsFalse(attackSuccessful);
            }
           
        }

        [Test]
        public void WaterspellAttacksKnight()
        {
            // Arrange
            Knight knight = new Knight();
            Spell spell = new Spell();

            // Act
            bool attackSuccessful = spell.Attack(knight);

            if (spell.ElementType == Element.Water)
            {
                // Assert
                Assert.IsTrue(attackSuccessful);
            }

        }

        [Test]
        public void KrakenAttacksSpell()
        {
            // Arrange
            Kraken kraken = new Kraken();
            Spell spell = new Spell();

            // Act
            bool attackSuccessful = kraken.Attack(spell);
   
            // Assert
            Assert.IsTrue(attackSuccessful);      

        }

        [Test]
        public void SpellAttacksKraken()
        {
            // Arrange
            Kraken kraken = new Kraken();
            Spell spell = new Spell();

            // Act
            bool attackSuccessful = spell.Attack(kraken);

            // Assert
            Assert.IsFalse(attackSuccessful);

        }

        [Test]
        public void FireElveAttacksDragon()
        {
            // Arrange
            FireElve fireElve = new FireElve();
            Dragon dragon = new Dragon();

            // Act
            bool attackSuccessful = fireElve.Attack(dragon);

            // Assert
            Assert.IsTrue(attackSuccessful);

        }

        [Test]
        public void DragonAttacksFireElve()
        {
            // Arrange
            FireElve fireElve = new FireElve();
            Dragon dragon = new Dragon();

            // Act
            bool attackSuccessful = dragon.Attack(fireElve);

            // Assert
            Assert.IsFalse(attackSuccessful);

        }

    }


}