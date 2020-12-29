using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using NUnit.Framework;
using Mtcg;
using Mtcg.Cards;

namespace UnitTest
{
    public class MemberTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void TestMonsterHasValidName()
        {
            // Arrange
            Dragon monster = new Dragon();

            // Act
            var name = monster.Name;

            // Assert
            Assert.IsNotNull(name);
        }


        [Test]
        public void TestMonsterHasValidDamage()
        {
            // Arrange
            Dragon monster = new Dragon();

            // Act
            var damage = monster.Damage;

            // Assert
            Assert.That(damage >= 20 && damage <= 120);
        }


        [Test]
        public void TestMonsterHasValidElement()
        {
            // Arrange
            Dragon monster = new Dragon();

            // Act
            var element = monster.ElementType;

            // Assert
            Assert.That(element == Element.Fire || element == Element.Water || element == Element.Normal);
        }


        [Test]
        public void TestSpellcardHasValidName()
        {
            // Arrange
            Spell spellcard = new Spell();

            // Act
            var name = spellcard.Name;

            // Assert
            Assert.IsNotNull(name);
        }

        [Test]
        public void TestSpellcardHasValidDamage()
        {
            // Arrange
            Spell spellcard = new Spell();

            // Act
            var damage = spellcard.Damage;

            // Assert
            Assert.That(damage >= 20 && damage <= 120);
        }

        [Test]
        public void TestSpellcardHasValidElement()
        {
            // Arrange
            Spell spellcard = new Spell();

            // Act
            var element = spellcard.ElementType;

            // Assert
            Assert.That(element == Element.Fire || element == Element.Water || element == Element.Normal);
        }


    }



}