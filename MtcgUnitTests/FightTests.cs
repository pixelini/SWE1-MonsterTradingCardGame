using System.Text;
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

        // SpecialCases
        [Test]
        public void Test_HasPlayer1WonRound_GoblinP1_DragonP2_Return_Loss()
        {
            // Arrange
            var goblin = new Goblin("845f0dc7-37d0-426e-994e-43fc3ac83c08", "Name", 10, Element.Normal);
            var dragon = new Ork("99f8f8dc-e25e-4a95-aa2c-782823f36e2a", "Name", 15, Element.Normal);

            // Act
            int result = Mtcg.Battle.HasPlayer1WonRound(goblin, dragon);

            // Assert
            Assert.AreEqual(2, result);
        }

        [Test]
        public void Test_HasPlayer1WonRound_WizzardP1_OrkP2_Return_Win()
        {
            // Arrange
            var wizzard = new Wizzard("845f0dc7-37d0-426e-994e-43fc3ac83c08", "Name", 10, Element.Normal);
            var ork = new Ork("99f8f8dc-e25e-4a95-aa2c-782823f36e2a", "Name", 15, Element.Normal);

            // Act
            int result = Mtcg.Battle.HasPlayer1WonRound(wizzard, ork);

            // Assert
            Assert.AreEqual(1, result);
        }

        [Test]
        public void Test_HasPlayer1WonRound_KnightP1_WaterSpellP2_Return_Win()
        {
            // Arrange
            var knight = new Knight("845f0dc7-37d0-426e-994e-43fc3ac83c08", "Name", 10, Element.Normal);
            var waterSpell = new Spell("99f8f8dc-e25e-4a95-aa2c-782823f36e2a", "Name", 15, Element.Water);

            // Act
            int result = Mtcg.Battle.HasPlayer1WonRound(knight, waterSpell);

            // Assert
            Assert.AreEqual(2, result);
        }

        [Test]
        public void Test_HasPlayer1WonRound_KrakenP1_SpellP2_Return_Win()
        {
            // Arrange
            var kraken = new Kraken("845f0dc7-37d0-426e-994e-43fc3ac83c08", "Name", 10, Element.Normal);
            var spell = new Spell("99f8f8dc-e25e-4a95-aa2c-782823f36e2a", "Name", 15, Element.Water);

            // Act
            int result = Mtcg.Battle.HasPlayer1WonRound(kraken, spell);

            // Assert
            Assert.AreEqual(1, result);
        }

        [Test]
        public void Test_HasPlayer1WonRound_FireElfP1_DragonP2_Return_Win()
        {
            // Arrange
            var fireElf = new Elf("845f0dc7-37d0-426e-994e-43fc3ac83c08", "Name", 10, Element.Fire);
            var dragon = new Dragon("99f8f8dc-e25e-4a95-aa2c-782823f36e2a", "Name", 15, Element.Water);

            // Act
            int result = Mtcg.Battle.HasPlayer1WonRound(fireElf, dragon);

            // Assert
            Assert.AreEqual(1, result);
        }

        // Monster Fights
        [Test]
        public void Test_HasPlayer1WonRound_WaterGoblinP1_FireOrkP2_Return_Loss()
        {
            // Arrange
            var waterGoblinPlayer1 = new Goblin("845f0dc7-37d0-426e-994e-43fc3ac83c08", "Name", 10, Element.Water);
            var fireOrkPlayer2 = new Ork("99f8f8dc-e25e-4a95-aa2c-782823f36e2a", "Name", 15, Element.Fire);

            // Act
            int result = Mtcg.Battle.HasPlayer1WonRound(waterGoblinPlayer1, fireOrkPlayer2);

            // Assert
            Assert.AreEqual(2, result);
        }

        [Test]
        public void Test_HasPlayer1WonRound_FireOrkP1_WaterGoblinP2_Return_Win()
        {
            // Arrange
            var fireOrkPlayer1 = new Ork("99f8f8dc-e25e-4a95-aa2c-782823f36e2a", "Name", 15, Element.Fire);
            var waterGoblinPlayer2 = new Goblin("845f0dc7-37d0-426e-994e-43fc3ac83c08", "Name", 10, Element.Water);

            // Act
            int result = Mtcg.Battle.HasPlayer1WonRound(fireOrkPlayer1, waterGoblinPlayer2);

            // Assert
            Assert.AreEqual(1, result);
        }

        // Spell Fights
        [Test]
        public void Test_HasPlayer1WonRound_FireSpellP1_WaterSpellP2_Return_Loss()
        {
            // Arrange
            var fireSpellPlayer1 = new Spell("99f8f8dc-e25e-4a95-aa2c-782823f36e2a", "Name", 10, Element.Fire);
            var waterSpellPlayer2 = new Spell("845f0dc7-37d0-426e-994e-43fc3ac83c08", "Name", 20, Element.Water);

            // Act
            int result = Mtcg.Battle.HasPlayer1WonRound(fireSpellPlayer1, waterSpellPlayer2);

            // Assert
            Assert.AreEqual(2, result);
        }

        [Test]
        public void Test_HasPlayer1WonRound_FireSpellP1_WaterSpellP2_Return_Draw()
        {
            // Arrange
            var fireSpellPlayer1 = new Spell("99f8f8dc-e25e-4a95-aa2c-782823f36e2a", "Name", 20, Element.Fire);
            var waterSpellPlayer2 = new Spell("845f0dc7-37d0-426e-994e-43fc3ac83c08", "Name", 5, Element.Water);

            // Act
            int result = Mtcg.Battle.HasPlayer1WonRound(fireSpellPlayer1, waterSpellPlayer2);

            // Assert
            Assert.AreEqual(3, result);
        }

        [Test]
        public void Test_HasPlayer1WonRound_FireSpellP1_WaterSpellP2_Return_Win()
        {
            // Arrange
            var fireSpellPlayer1 = new Spell("99f8f8dc-e25e-4a95-aa2c-782823f36e2a", "Name", 90, Element.Fire);
            var waterSpellPlayer2 = new Spell("845f0dc7-37d0-426e-994e-43fc3ac83c08", "Name", 5, Element.Water);

            // Act
            int result = Mtcg.Battle.HasPlayer1WonRound(fireSpellPlayer1, waterSpellPlayer2);

            // Assert
            Assert.AreEqual(1, result);
        }

        // Mixed Fights
        [Test]
        public void Test_HasPlayer1WonRound_FireSpellP1_WaterGoblinP2_Return_Loss()
        {
            // Arrange
            var fireSpellPlayer1 = new Spell("99f8f8dc-e25e-4a95-aa2c-782823f36e2a", "Name", 10, Element.Fire);
            var waterGoblinPlayer2 = new Goblin("845f0dc7-37d0-426e-994e-43fc3ac83c08", "Name", 10, Element.Water);

            // Act
            int result = Mtcg.Battle.HasPlayer1WonRound(fireSpellPlayer1, waterGoblinPlayer2);

            // Assert
            Assert.AreEqual(2, result);
        }

        [Test]
        public void Test_HasPlayer1WonRound_WaterSpellP1_WaterGoblinP2_Return_Draw()
        {
            // Arrange
            var waterSpellPlayer1 = new Spell("99f8f8dc-e25e-4a95-aa2c-782823f36e2a", "Name", 10, Element.Water);
            var waterGoblinPlayer2 = new Goblin("845f0dc7-37d0-426e-994e-43fc3ac83c08", "Name", 10, Element.Water);

            // Act
            int result = Mtcg.Battle.HasPlayer1WonRound(waterSpellPlayer1, waterGoblinPlayer2);

            // Assert
            Assert.AreEqual(3, result);
        }

        [Test]
        public void Test_HasPlayer1WonRound_NormalSpellP1_WaterGoblinP2_Return_Win()
        {
            // Arrange
            var normalSpellPlayer1 = new Spell("99f8f8dc-e25e-4a95-aa2c-782823f36e2a", "Name", 10, Element.Normal);
            var waterGoblinPlayer2 = new Goblin("845f0dc7-37d0-426e-994e-43fc3ac83c08", "Name", 10, Element.Water);

            // Act
            int result = Mtcg.Battle.HasPlayer1WonRound(normalSpellPlayer1, waterGoblinPlayer2);

            // Assert
            Assert.AreEqual(1, result);
        }

        [Test]
        public void Test_HasPlayer1WonRound_NormalSpellP1_KnightP2_Return_Win()
        {
            // Arrange
            var normalSpellPlayer1 = new Spell("99f8f8dc-e25e-4a95-aa2c-782823f36e2a", "Name", 10, Element.Normal);
            var knightPlayer2 = new Goblin("845f0dc7-37d0-426e-994e-43fc3ac83c08", "Name", 15, Element.Normal);

            // Act
            int result = Mtcg.Battle.HasPlayer1WonRound(normalSpellPlayer1, knightPlayer2);

            // Assert
            Assert.AreEqual(2, result);
        }

        /*
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
        */

    }


}