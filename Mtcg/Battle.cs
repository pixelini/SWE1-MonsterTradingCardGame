using System;
using System.Data;
using System.Diagnostics;
using System.Text;
using Castle.Core.Internal;
using Mtcg.Cards;

namespace Mtcg
{
    public class Battle
    {
        public User Player1 { get; set; }
        public User Player2 { get; set; }
        public Logger Gamelog { get; set; }
        public int MaxRounds { get; set; }
        public int UserCount { get; set; }
        public bool Running { get; set; }
        private bool _started { get; set; }
        public User Winner { get; set; }

        public Battle()
        {
            UserCount = 1;
            _started = false;
            Running = false;
            MaxRounds = 100;
        }

        private bool IsSlotAvailable()
        {
            return UserCount < 2;
        }

        public bool AddUserToBattle(User player2)
        {
            if (this.IsSlotAvailable())
            {
                this.Player2 = player2;
                this.UserCount++;
                return true;
            }

            return false;

        }

        public void StartGame()
        {
            _started = true;
            Running = true;
            Play();
        }

        public bool HasStarted()
        {
            return _started;
        }

        public bool IsRunning()
        {
            return Running;
        }

        
        private void Play()
        {
            Gamelog = new Logger("LOG von Spiel zwischen " + Player1.Username + " und " + Player2.Username);

            for (int i = 0; i < MaxRounds; i++)
            {
                Console.WriteLine("-------------------------------------------------\n");
                Gamelog.AddEntry(String.Format("Spielrunde: {0}", i+1));
                Console.WriteLine("Spielrunde: {0}", i+1);
                Console.WriteLine("Anzahl Karten Spieler 1: " + Player1.Deck.Count);
                Console.WriteLine("Anzahl Karten Spieler 2: " + Player2.Deck.Count);
                ICard chosenCardP1 = Player1.PickCardFromDeck();
                ICard chosenCardP2 = Player2.PickCardFromDeck();

                Console.WriteLine("\nGewählte Karten: ");
                Console.WriteLine("Spieler 1: {0}, {1}, {2}", chosenCardP1.Name, chosenCardP1.Damage, chosenCardP1.ElementType);
                Console.WriteLine("Spieler 2: {0}, {1}, {2}", chosenCardP2.Name, chosenCardP2.Damage, chosenCardP2.ElementType);

                Gamelog.AddEntry(String.Format("{0} (Spieler 1): {1}, {2}, {3}", Player1.Username, chosenCardP1.Name, chosenCardP1.Damage, chosenCardP1.ElementType));
                Gamelog.AddEntry(String.Format("{0} (Spieler 2): {1}, {2}, {3}", Player2.Username, chosenCardP2.Name, chosenCardP2.Damage, chosenCardP2.ElementType));

                //HasPlayer1WonRound?
                // YES: player1 bekommt card p2 von player2! (zu deck von player1 hinzufügen: card p2), (aus deck von player2 entfernen: card p2)
                // NO: player2 bekommt card p1 von player1! (zu deck von player2 hinzufügen: card p1), (aus deck von player1 entfernen: card p1)

                // can return 3 cases: 3 values present that: if 1 returns
                int result = HasPlayer1WonRound(chosenCardP1, chosenCardP2);

                switch (result)
                {
                    case 1:
                        Gamelog.AddEntry("" + Player1.Username + " gewinnt.");
                        Console.WriteLine("Spieler 1 gewinnt.");
                        Player1.AddCardToDeck(chosenCardP2);
                        Player2.RemoveCardFromDeck(chosenCardP2);
                        break;
                    case 2:
                        Gamelog.AddEntry("" + Player2.Username + " gewinnt.");
                        Console.WriteLine("Spieler 2 gewinnt.");
                        Player2.AddCardToDeck(chosenCardP1);
                        Player1.RemoveCardFromDeck(chosenCardP1);
                        break;
                    case 3:
                        Gamelog.AddEntry("Unentschieden.");
                        Console.WriteLine("Unentschieden.");
                        break;
                }

                Gamelog.AddEntry("\n");

                // Deck is updated

                // Game ends if one user has no cards
                if (Player1.Deck.IsNullOrEmpty() || Player2.Deck.IsNullOrEmpty())
                {
                    if (Player2.Deck.IsNullOrEmpty())
                    {
                        Gamelog.AddEntry("SPIELENDE. " + Player2.Username + " hat keine Karten mehr. "+ Player1.Username + " hat das Battle gewonnen.");
                        Console.WriteLine("Spieler 1 hat das Spiel gewonnen.");
                        Winner = Player1;
                    }
                    else
                    {
                        Gamelog.AddEntry("SPIELENDE. " + Player1.Username + " hat keine Karten mehr. " + Player2.Username + " hat das Battle gewonnen.");
                        Console.WriteLine("SPIELENDE. Spieler 1 hat keine Karten mehr. Spieler 2 hat das Spiel gewonnen.");
                        Winner = Player2;
                    }
                    EndGame();
                    Console.WriteLine("\n-------------------------------------------------\n");
                    return;
                }
                Console.WriteLine("\n-------------------------------------------------\n");

            }

            // if there is no final result after maxRound --> DRAW!

            EndGame();
            // Winner is null --> DRAW
            Gamelog.AddEntry("Das Spiel ist unentschieden ausgegangen!");
        }


        // this method returns 1, 2 or 3 (1 -> Player1 has won, 2 -> Player1 has lost, 3 -> Unentschieden)
        public static int HasPlayer1WonRound(ICard cardP1, ICard cardP2)
        {
            // Evaluate type of fight (Monster Fight, Spell Fight or Mixed Fight)
            // Monster fight: pure monster fights are not affected by the element type
            // Mixed fight: element type has an effect on the damage calculation of this single round
            // Spell fight: as soon as 1 spell card is played the element type has an effect on the damage calculation of this single round

            int result = -1;

            if (cardP1 is Monster && cardP2 is Monster)
            {
                Console.WriteLine("Monster Fight");
                result = HasPlayer1WonMonsterFight(cardP1, cardP2);
            } else if (cardP1 is Spell && cardP2 is Spell)
            {
                Console.WriteLine("Spell Fight");
                result = HasPlayer1WonSpellandMixedFight(cardP1, cardP2);
            }
            else if (cardP1 is Monster && cardP2 is Spell || cardP1 is Spell && cardP2 is Monster)
            {
                Console.WriteLine("Mixed Fight");
                result = HasPlayer1WonSpellandMixedFight(cardP1, cardP2);
            }

            return result;
        }

        private static int HasPlayer1WonMonsterFight(ICard cardP1, ICard cardP2)
        {
            // Elementtype has no impact on calculation (except FireElf!)

            // specialties
            if ((cardP1 is Goblin && cardP2 is Dragon) || 
                (cardP1 is Dragon && cardP2 is Goblin))
            {
                Console.WriteLine("Goblin hat Angst vor Dragon.");
                return cardP1 is Dragon ? 1 : 2;

            }

            if ((cardP1 is Wizzard && cardP2 is Ork) || 
                (cardP1 is Ork && cardP2 is Wizzard))
            {
                Console.WriteLine("Wizzard kann Ork kontrollieren. Der Ork kann ihm nichts anhaben.");
                return cardP1 is Wizzard ? 1 : 2;
            }

            if ((cardP1 is Elf && cardP1.ElementType == Element.Fire && cardP2 is Dragon) || 
                (cardP1 is Dragon && cardP2 is Elf && cardP2.ElementType == Element.Fire))
            {
                Console.WriteLine("FireElf und Drache kennen sich schon lange. FireElf kann den Angriff abwehren");
                return cardP1 is Dragon ? 2 : 1;
            }

            return HaveWon(cardP1.Damage, cardP2.Damage);
            
        }


        private static int HasPlayer1WonSpellandMixedFight(ICard cardP1, ICard cardP2)
        {
            // special cases: (Knights vs. WaterSpells, Kraken vs. Spells)
            if ((cardP1 is Knight && cardP2 is Spell && cardP2.ElementType == Element.Water) ||
                (cardP1 is Spell && cardP1.ElementType == Element.Water && cardP2 is Knight))
            {
                Console.WriteLine("MixedFight");
                Console.WriteLine("WaterSpells lassen Knights ertrinken.");
                return cardP1 is Knight ? 2 : 1;
            }

            if ((cardP1 is Kraken && cardP2 is Spell) ||
                (cardP1 is Spell && cardP2 is Kraken))
            {
                Console.WriteLine("Kraken sind immun gegen Spells.");
                return cardP1 is Kraken ? 1 : 2;
            }



            // NORMAL vs. NORMAL
            if (cardP1.ElementType == Element.Normal && cardP2.ElementType == Element.Normal)
            {
                return HaveWon(cardP1.Damage, cardP2.Damage);
            }

            // WATER vs. FIRE
            if ((cardP1.ElementType == Element.Water && cardP2.ElementType == Element.Fire) || 
                (cardP1.ElementType == Element.Fire && cardP2.ElementType == Element.Water))
            {
                Console.WriteLine("Wasser ist effektiv gegen Feuer.");
                if (cardP1.ElementType == Element.Water)
                {
                    float newDamageP1 = cardP1.Damage * 2;
                    float newDamageP2 = cardP1.Damage / 2;
                    return HaveWon(newDamageP1, newDamageP2);
                }
                else
                {
                    float newDamageP1 = cardP1.Damage / 2;
                    float newDamageP2 = cardP2.Damage * 2;
                    return HaveWon(newDamageP1, newDamageP2);
                }

            }

            // FIRE vs. NORMAL
            if ((cardP1.ElementType == Element.Fire && cardP2.ElementType == Element.Normal) ||
                (cardP1.ElementType == Element.Normal && cardP2.ElementType == Element.Fire))
            {
                Console.WriteLine("Feuer ist effektiv gegen Normal.");
                if (cardP1.ElementType == Element.Fire)
                {
                    float newDamageP1 = cardP1.Damage * 2;
                    float newDamageP2 = cardP1.Damage / 2;
                    return HaveWon(newDamageP1, newDamageP2);
                }
                else
                {
                    float newDamageP1 = cardP1.Damage / 2;
                    float newDamageP2 = cardP1.Damage * 2;
                    return HaveWon(newDamageP1, newDamageP2);
                }

            }

            // NORMAL vs. WATER
            if ((cardP1.ElementType == Element.Normal && cardP2.ElementType == Element.Water) ||
                (cardP1.ElementType == Element.Water && cardP2.ElementType == Element.Normal))
            {
                Console.WriteLine("Normal ist effektiv gegen Wasser.");
                if (cardP1.ElementType == Element.Normal)
                {
                    float newDamageP1 = cardP1.Damage * 2;
                    float newDamageP2 = cardP1.Damage / 2;
                    return HaveWon(newDamageP1, newDamageP2);
                }
                else
                {
                    float newDamageP1 = cardP1.Damage / 2;
                    float newDamageP2 = cardP1.Damage * 2;
                    return HaveWon(newDamageP1, newDamageP2);
                }

            }

            // else draw
            return 3;
        }

        private void EndGame()
        {
            Running = false;
        }

        private static int HaveWon(float damageP1, float damageP2)
        {
            if (damageP1 > damageP2)
            {
                //Player 1 wins
                return 1;
            }
            else if (damageP1 < damageP2)
            {
                //Player 2 wins
                return 2;
            }
            else
            {
                //Draw
                return 3;
            }
        }
    }
}