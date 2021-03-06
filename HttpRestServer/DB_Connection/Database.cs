﻿using System;
using System.Collections.Generic;
using Mtcg;
using Mtcg.Cards;
using Npgsql;

namespace HttpRestServer.DB_Connection
{
    public class Database
    {
        public string Host { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }

        public Database()
        {
            Host = "localhost";
            Username = "postgres";
            Password = "postgres";
            Name = "postgres";
        }

        public Database(string host, string username, string password, string name)
        {
            Host = host;
            Username = username;
            Password = password;
            Name = name;
        }

        // Register user with username and password and creates token for authentification. Method also checks if user already exists.
        public bool RegisterUser(string username, string password)
        {
            bool success = false;

            if (DoesUserAlreadyExist(username))
            {
                Console.WriteLine("Ein User mit diesem Usernamen existiert bereits.");
                return false;
            }

            Console.WriteLine("Registriere User...");
            string encryptedPassword = Encrypt(password);
            bool isAdmin = (username == "admin");
            var token = "Basic " + username + "-mtcgToken";
            var conn = Connect();
            var sql = "INSERT INTO swe1_mtcg.\"user\" (username, password, auth_token, is_admin, coins, elo) VALUES (@username, @password, @authToken, @isAdmin, @coins, @elo)";

            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.Add(new NpgsqlParameter("@username", username));
            cmd.Parameters.Add(new NpgsqlParameter("@password", encryptedPassword));
            cmd.Parameters.Add(new NpgsqlParameter("@authToken", token));
            cmd.Parameters.Add(new NpgsqlParameter("@isAdmin", isAdmin));
            cmd.Parameters.Add(new NpgsqlParameter("@coins", 20));
            cmd.Parameters.Add(new NpgsqlParameter("@elo", 100));
            cmd.Prepare();

            if (cmd.ExecuteNonQuery() == 1)
            {
                Console.WriteLine("User wurde erfolgreich registriert.");
                CreateUserProfile(username);
                success = true;
            }

            conn.Close();
            return success;
        }

        public bool Login(string username, string password)
        {
            if (!DoesUserAlreadyExist(username))
            {
                Console.WriteLine("Der Username existiert nicht.");
                return false;
            }

            bool success = false;
            string encrypedPassword = Encrypt(password);
            var conn = Connect();
            var sql = "SELECT * FROM swe1_mtcg.\"user\" WHERE username = @username AND password = @password";

            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.Add(new NpgsqlParameter("@username", username));
            cmd.Parameters.Add(new NpgsqlParameter("@password", encrypedPassword));
            cmd.Prepare();

            var reader = cmd.ExecuteReader();

            success = reader.HasRows;
            conn.Close();
            return success;
        }

        public bool AddPackage(string packageId, List<Card> cards)
        {
            bool success = false;

            // create new package and in table package
            string packageName = CreateRandomName("Package");

            // check if package already exists
            if (DoesPackageExist(packageId))
            {
                Console.WriteLine("Package existiert bereits.");
                return false;
            }

            success = CreatePackage(packageId, packageName);
            Console.WriteLine("Package '{0}' (ID: {1}) wurde erstellt.\n", packageName, packageId);

            foreach (var card in cards)
            {
                // extract elementType and cardType and save adds card to table card
                success = AddCard(card.Id, card.Name, card.Damage);
                if (!success) // is the case, if card couldn't be added even though she doesn't already exist
                {
                    Console.WriteLine("Erstellung der Karten wurde abgebrochen. Es wurden dem Package keine Karten zugeordnet.", packageName, packageId);
                    return false;
                }
            }

            foreach (var card in cards)
            {
                // adds card to package in table package_has_cards
                success = AddCardToPackage(packageId, card.Id, card.Name);
                if (!success)
                {
                    return false;
                }
            }

            return success;
        }

        public bool ValidateToken(string token, string username)
        {
            bool success = false;
            string correctToken = "";
            var conn = Connect();
            var sql = "SELECT auth_token FROM swe1_mtcg.\"user\" WHERE username = @username";

            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.Add(new NpgsqlParameter("@username", username));
            cmd.Prepare();

            var reader = cmd.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    correctToken = reader.GetString(0); // this is the correct token
                }
            }

            success = correctToken == token;
            conn.Close();
            return success;
        }

        public bool BuyPackage(string packageID, string username)
        {
            int userID = GetUserID(username);

            // check if package exists
            if (!DoesPackageExist(packageID))
            {
                Console.WriteLine("Package existiert nicht.");
                return false;
            }

            // check if packages is already in possession of user
            if (OwnsPackageAlready(packageID, userID))
            {
                Console.WriteLine("User besitzt dieses Package bereits.");
                return false;
            }

            // check if user can afford the package
            if (!CanAffordPackage(packageID, userID))
            {
                Console.WriteLine("User hat nicht genug Geld für Package.");
                return false;
            }

            // buy package and reduce money
            if (!ExecutePurchase(packageID, userID))
            {
                Console.WriteLine("Kauf war nicht erfolgreich.");
                return false;
            }

            return true;
        }

        public bool CheckIfUserIsAdmin(string username)
        {
            bool success = false;
            Console.WriteLine("Check if Admin...");
            var conn = Connect();
            var sql = "SELECT * FROM swe1_mtcg.\"user\" WHERE username = @username AND is_admin = 'true'";
            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.Add(new NpgsqlParameter("@username", username));
            cmd.Prepare();

            var reader = cmd.ExecuteReader();
            success = reader.HasRows;
            conn.Close();
            return success;

        }

        public List<ICard> GetAllCards(string username)
        {
            List<ICard> myCards = new List<ICard>();

            var conn = Connect();
            var sql = "SELECT card_id, c.name, damage, element, type FROM swe1_mtcg.user_owns_cards JOIN swe1_mtcg.card c on c.id = user_owns_cards.card_id WHERE user_id = @user_id";
            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.Add(new NpgsqlParameter("@user_id", GetUserID(username)));

            var reader = cmd.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    string cardId = reader.GetString(0);
                    string cardName = reader.GetString(1);
                    float damage = reader.GetFloat(2);
                    string elementType = reader.GetString(3);
                    string monsterType = reader.GetString(4);
                    var card = InitalizeCardAsObject(cardId, cardName, damage, elementType, monsterType);
                    myCards.Add(card);
                }
            }

            conn.Close();
            return myCards;

        }

        public ICard GetCard(string cardId, string username)
        {
            if (!DoesCardAlreadyExist(cardId) || !OwnsCardAlready(cardId, username))
            {
                Console.WriteLine("Karte exisitiert nicht oder gehoert dem User nicht.");
                return null;
            }

            var conn = Connect();
            var sql = "SELECT name, damage, element, type FROM swe1_mtcg.card WHERE id = @id";
            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.Add(new NpgsqlParameter("@id", cardId));

            var reader = cmd.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    string cardName = reader.GetString(0);
                    float damage = reader.GetFloat(1);
                    string elementType = reader.GetString(2);
                    string monsterType = reader.GetString(3);
                    ICard card = InitalizeCardAsObject(cardId, cardName, damage, elementType, monsterType);
                    conn.Close();
                    return card;
                }
                
            }

            conn.Close();
            return null;
        }

        public List<ICard> GetDeck(string username)
        {
            List<ICard> myCards = new List<ICard>();

            var conn = Connect();
            var sql = "SELECT card_id, name, damage, element, type FROM swe1_mtcg.deck JOIN swe1_mtcg.card c on deck.card_id = c.id WHERE user_id = @id";
            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.Add(new NpgsqlParameter("@id", GetUserID(username)));

            var reader = cmd.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    string cardId = reader.GetString(0);
                    string cardName = reader.GetString(1);
                    float damage = reader.GetFloat(2);
                    string elementType = reader.GetString(3);
                    string monsterType = reader.GetString(4);
                    var card = InitalizeCardAsObject(cardId, cardName, damage, elementType, monsterType);
                    myCards.Add(card);
                }
            }

            conn.Close();
            return myCards;

        }

        public bool AddCardToDeck(string card1, string username)
        {
            bool success = false;

            // check if user owns all these cards he want in his deck
            if(!OwnsCardAlready(card1, username))
            {
                return false;
            }

            var conn = Connect();
            var sql = "INSERT INTO swe1_mtcg.deck(user_id, card_id) VALUES (@user_id, @card_id)";

            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.Add(new NpgsqlParameter("@user_id", GetUserID(username)));
            cmd.Parameters.Add(new NpgsqlParameter("@card_id", card1));
            cmd.Prepare();

            if (cmd.ExecuteNonQuery() == 1)
            {
                Console.WriteLine("ID: {0}", card1);
                success = true;
            }

            conn.Close();
            return success;

        }

        public bool DeleteDeck(string username)
        {
            bool success = false;

            var conn = Connect();
            var sql = "DELETE FROM swe1_mtcg.deck WHERE user_id=@user_id";

            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.Add(new NpgsqlParameter("@user_id", GetUserID(username)));
            cmd.Prepare();

            if (cmd.ExecuteNonQuery() <= 0)
            {
                success = true;
            }

            conn.Close();
            return success;

        }

        public bool DeleteDeal(string username, string tradeId)
        {
            bool success = false;

            var conn = Connect();
            var sql = "DELETE FROM swe1_mtcg.tradings WHERE user_id=@user_id and id=@id";

            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.Add(new NpgsqlParameter("@user_id", GetUserID(username)));
            cmd.Parameters.Add(new NpgsqlParameter("@id", tradeId));
            cmd.Prepare();

            if (cmd.ExecuteNonQuery() == 1)
            {
                success = true;
            }

            conn.Close();
            return success;
        }

        public Stats GetStats(string username)
        {
            Stats stats = null;

            var conn = Connect();
            var sql = "SELECT username, elo, wins, losses FROM swe1_mtcg.\"user\" WHERE id = @id";
            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.Add(new NpgsqlParameter("@id", GetUserID(username)));

            var reader = cmd.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    stats = new Stats();
                    stats.Username = reader.GetString(0);
                    stats.Elo = reader.GetInt32(1);
                    stats.Wins = reader.GetInt32(2);
                    stats.Losses = reader.GetInt32(3);
                }
            }

            conn.Close();
            return stats;

        }

        public bool UpdateStatsAfterWin(string username)
        {
            var conn = Connect();
            var sql = "UPDATE swe1_mtcg.\"user\" SET elo=elo+3, wins=wins+1 WHERE id = @id";
            using var cmdUpdate = new NpgsqlCommand(sql, conn);
            cmdUpdate.Parameters.Add(new NpgsqlParameter("@id", GetUserID(username)));
            cmdUpdate.Prepare();

            if (cmdUpdate.ExecuteNonQuery() != 1)
            {
                conn.Close();
                return false;
            }

            conn.Close();
            return true;
        }

        public bool UpdateStatsAfterLoss(string username)
        {
            var conn = Connect();
            var sql = "UPDATE swe1_mtcg.\"user\" SET elo=elo-5, losses=losses+1 WHERE id = @id";
            using var cmdUpdate = new NpgsqlCommand(sql, conn);
            cmdUpdate.Parameters.Add(new NpgsqlParameter("@id", GetUserID(username)));
            cmdUpdate.Prepare();

            if (cmdUpdate.ExecuteNonQuery() != 1)
            {
                conn.Close();
                return false;
            }

            conn.Close();
            return true;
        }

        public List<Stats> GetScoreboard()
        {
            List<Stats> scoreboard = new List<Stats>();

            var conn = Connect();
            var sql = "SELECT username, elo, wins, losses FROM swe1_mtcg.\"user\" order by elo DESC";

            using var cmd = new NpgsqlCommand(sql, conn);

            var reader = cmd.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    var statsOfUser = new Stats()
                    {
                        Username = reader.GetString(0), 
                        Elo = reader.GetInt32(1), 
                        Wins = reader.GetInt32(2), 
                        Losses = reader.GetInt32(3)
                    };
                    scoreboard.Add(statsOfUser);
                }
            }

            conn.Close();
            return scoreboard;

        }

        public Profile GetUserProfile(string username)
        {
            Profile profile = null;

            if (!DoesUserAlreadyExist(username))
            {
                Console.WriteLine("User exisitiert nicht.");
                return null;
            }

            var conn = Connect();
            var sql = "SELECT profile_name, image, bio FROM swe1_mtcg.profil WHERE user_id = @id";
            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.Add(new NpgsqlParameter("@id", GetUserID(username)));

            var reader = cmd.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    profile = new Profile();
                    profile.Name = reader.GetString(0);
                    profile.Image = reader.GetString(1);
                    profile.Bio = reader.GetString(2);
                }
            }

            conn.Close();
            return profile;

        }

        public Trade GetTrade(string tradeId)
        {
            Trade trade = null;

            if (!DoesTradeAlreadyExist(tradeId))
            {
                Console.WriteLine("Trade exisitiert nicht.");
                return null;
            }

            var conn = Connect();
            var sql = "SELECT id, username, card_id, type, min_damage FROM swe1_mtcg.tradings WHERE id = @id";
            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.Add(new NpgsqlParameter("@id", tradeId));
            var reader = cmd.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    trade = new Trade()
                    {
                        Id = reader.GetString(0),
                        Username = reader.GetString(1),
                        CardToTrade = reader.GetString(2),
                        Type = reader.GetString(3),
                        MinimumDamage = reader.GetFloat(4)
                    };
                }
            }

            conn.Close();
            return trade;

        }

        public bool EditUserProfile(string username, string newName, string newBio, string newImage)
        {
            var conn = Connect();
            var sql = "UPDATE swe1_mtcg.profil SET profile_name=@name, bio=@bio, image=@image WHERE user_id = @user_id";
            using var cmdUpdate = new NpgsqlCommand(sql, conn);
            cmdUpdate.Parameters.Add(new NpgsqlParameter("@name", newName));
            cmdUpdate.Parameters.Add(new NpgsqlParameter("@bio", newBio));
            cmdUpdate.Parameters.Add(new NpgsqlParameter("@image", newImage));
            cmdUpdate.Parameters.Add(new NpgsqlParameter("@user_id", GetUserID(username)));
            cmdUpdate.Prepare();

            if (cmdUpdate.ExecuteNonQuery() != 1)
            {
                conn.Close();
                return false;
            }

            conn.Close();
            return true;
        }

        public bool AddTrade(string username, string tradeId, string cardToTradeId, string cardType, float minDamage)
        {
            if (DoesTradeAlreadyExist(tradeId))
            {
                Console.WriteLine("Trade existiert bereits.");
                return false;
            }

            var conn = Connect();
            var sql = "INSERT INTO swe1_mtcg.tradings(user_id, card_id, type, min_damage, id, username) VALUES (@user_id, @card_id, @type, @min_damage, @id, @username)";
            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.Add(new NpgsqlParameter("@id", tradeId));
            cmd.Parameters.Add(new NpgsqlParameter("@user_id", GetUserID(username)));
            cmd.Parameters.Add(new NpgsqlParameter("@card_id", cardToTradeId));
            cmd.Parameters.Add(new NpgsqlParameter("@type", cardType));
            cmd.Parameters.Add(new NpgsqlParameter("@min_damage", minDamage));
            cmd.Parameters.Add(new NpgsqlParameter("@username", username));
            cmd.Prepare();

            if (cmd.ExecuteNonQuery() != 1)
            {
                conn.Close();
                return false;
            }

            conn.Close();
            return true;

        }

        public bool ExecuteDeal(string tradeId, string usernameDealOwner, string cardIdDealOwner, string usernameTradeExecuter, string cardIdTradeExecuter)
        {
            Console.WriteLine(usernameDealOwner, cardIdDealOwner, usernameTradeExecuter, cardIdTradeExecuter);
            // trade executer gives his card to deal owner
            if (!TransferCard(usernameTradeExecuter, cardIdTradeExecuter, usernameDealOwner))
            {
                return false;
            }

            // deal owner gives his card to trade executer
            if (!TransferCard(usernameDealOwner, cardIdDealOwner, usernameTradeExecuter))
            {
                return false;
            }

            Console.WriteLine("Karten wurden erfolgreich ausgetauscht.");

            // delete deal from tradings
            if (!DeleteDeal(usernameDealOwner, tradeId))
            {
                Console.WriteLine("Deal konnte nicht entfernt werden.");
                return false;
            }

            return true;
        }

        public List<Trade> GetAllTrades()
        {
            List<Trade> allTrades = new List<Trade>();

            var conn = Connect();
            var sql = "SELECT id, username, card_id, type, min_damage FROM swe1_mtcg.tradings";
            using var cmd = new NpgsqlCommand(sql, conn);

            var reader = cmd.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    var trade = new Trade()
                    {
                        Id = reader.GetString(0),
                        Username = reader.GetString(1),
                        CardToTrade = reader.GetString(2),
                        Type = reader.GetString(3),
                        MinimumDamage = reader.GetFloat(4)
                    };
                    allTrades.Add(trade);
                }
            }

            conn.Close();

            return allTrades;
        }

        public bool CreateUserProfile(string username)
        {
            var conn = Connect();
            var sql = "INSERT INTO swe1_mtcg.profil(user_id, profile_name, image, bio) VALUES (@user_id, @username, @image, @bio)";
            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.Add(new NpgsqlParameter("@user_id", GetUserID(username)));
            cmd.Parameters.Add(new NpgsqlParameter("@username", username));
            cmd.Parameters.Add(new NpgsqlParameter("@image", ":-)"));
            cmd.Parameters.Add(new NpgsqlParameter("@bio", "Hey there! I am playing MTCG."));
            cmd.Prepare();

            if (cmd.ExecuteNonQuery() != 1)
            {
                Console.WriteLine("Profil erstellen nicht erfolgreich.");
                conn.Close();
                return false;
            }

            conn.Close();
            return true;

        }

        private ICard InitalizeCardAsObject(string id, string name, float damage, string elementType, string cardType)
        {
            if (cardType == CardType.Dragon.ToString())
            {
                var card = new Dragon(id, name, damage, (Element)Enum.Parse(typeof(Element), elementType));
                return card;
            } 
            else if (cardType == CardType.Elf.ToString())
            {
                var card = new Elf(id, name, damage, (Element)Enum.Parse(typeof(Element), elementType));
                return card;
            } 
            else if (cardType == CardType.Goblin.ToString())
            {
                var card = new Goblin(id, name, damage, (Element)Enum.Parse(typeof(Element), elementType));
                return card;
            }
            else if (cardType == CardType.Knight.ToString())
            {
                var card = new Knight(id, name, damage, (Element)Enum.Parse(typeof(Element), elementType));
                return card;
            }
            else if (cardType == CardType.Kraken.ToString())
            {
                var card = new Kraken(id, name, damage, (Element)Enum.Parse(typeof(Element), elementType));
                return card;
            }
            else if (cardType == CardType.Ork.ToString())
            {
                var card = new Ork(id, name, damage, (Element)Enum.Parse(typeof(Element), elementType));
                return card;
            }
            else if (cardType == CardType.Wizzard.ToString())
            {
                var card = new Wizzard(id, name, damage, (Element)Enum.Parse(typeof(Element), elementType));
                return card;
            }
            else if (cardType == CardType.Spell.ToString())
            {
                var card = new Spell(id, name, damage, (Element)Enum.Parse(typeof(Element), elementType));
                return card;
            }

            return null;

        }

        private string Encrypt(string value)
        {
            // ROT-13
            char[] array = value.ToCharArray();
            for (int i = 0; i < array.Length; i++)
            {
                int number = (int)array[i];

                if (number >= 'a' && number <= 'z')
                {
                    if (number > 'm')
                    {
                        number -= 13;
                    }
                    else
                    {
                        number += 13;
                    }
                }
                else if (number >= 'A' && number <= 'Z')
                {
                    if (number > 'M')
                    {
                        number -= 13;
                    }
                    else
                    {
                        number += 13;
                    }
                }
                array[i] = (char)number;
            }
            return new string(array);
        }

        private NpgsqlConnection Connect()
        {
            var connString = $"Host={Host};Username={Username};Password={Password};Database={Name}";
            var conn = new NpgsqlConnection(connString);
            conn.Open();
            return conn;
        }

        private bool DoesUserAlreadyExist(string username)
        {
            bool success = false;
            var conn = Connect();
            var sql = "SELECT * FROM swe1_mtcg.\"user\" WHERE username = @username";

            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.Add(new NpgsqlParameter("@username", username));
            cmd.Prepare();

            var reader = cmd.ExecuteReader();
            success = reader.HasRows;
            conn.Close();
            return success;
        }

        private bool DoesCardAlreadyExist(string id)
        {
            bool success = false;
            var conn = Connect();
            var sql = "SELECT * FROM swe1_mtcg.card WHERE id = @id";

            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.Add(new NpgsqlParameter("@id", id));
            cmd.Prepare();

            var reader = cmd.ExecuteReader();
            success = reader.HasRows;
            conn.Close();
            return success;
        }

        private bool DoesTradeAlreadyExist(string id)
        {
            bool success = false;
            var conn = Connect();
            var sql = "SELECT * FROM swe1_mtcg.tradings WHERE id = @id";

            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.Add(new NpgsqlParameter("@id", id));
            cmd.Prepare();

            var reader = cmd.ExecuteReader();
            success = reader.HasRows;
            conn.Close();
            return success;
        }

        private string ExtractElementType(string cardName)
        {
            var arr = Enum.GetValues(typeof(Element));

            for (var i = 0; i < arr.Length; i++)
            {
                var elementType = arr.GetValue(i)?.ToString();
                if (elementType != null && cardName.ToLower().Contains(elementType.ToLower()))
                {
                    return elementType;
                }
            }

            // if there is no specific element type given, the card will be defined as first element of enum-array (here: normal)
            return arr.GetValue(0)?.ToString();
        }

        private string ExtractCardType(string cardName)
        {
            var arr = Enum.GetValues(typeof(CardType));

            for (var i = 0; i < arr.Length; i++)
            {
                var cardType = arr.GetValue(i)?.ToString();
                if (cardType != null && cardName.ToLower().Contains(cardType.ToLower()))
                {
                    return cardType;
                }
            }

            // if there is no specific monster type given, the card will be defined as first element of enum-array (here: spell)
            return arr.GetValue(0)?.ToString();
        }

        private bool AddCard(string id, string name, float damage)
        {
            bool success = false;

            // check if card alreay exists
            if (DoesCardAlreadyExist(id))
            {
                Console.WriteLine("Karte (ID: {0}) existiert bereits.", id);
                success = true; // doesn't matter in this case
                return success;
            }

            string cardType = ExtractCardType(name); // spell or monster
            var elementType = ExtractElementType(name); // fire, water or normal

            var conn = Connect();
            var sql = "INSERT INTO swe1_mtcg.card (name, damage, element, type, id) VALUES (@name, @damage, @element, @type, @id)";

            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.Add(new NpgsqlParameter("@name", name));
            cmd.Parameters.Add(new NpgsqlParameter("@damage", damage));
            cmd.Parameters.Add(new NpgsqlParameter("@element", elementType));
            cmd.Parameters.Add(new NpgsqlParameter("@type", cardType));
            cmd.Parameters.Add(new NpgsqlParameter("@id", id));
            cmd.Prepare();

            if (cmd.ExecuteNonQuery() == 1)
            {
                //Console.WriteLine("Karte {0} wurde erfolgreich hinzugefügt.", name);
                success = true;
            }

            conn.Close();
            return success;
        }

        private bool CreatePackage(string packageId, string name)
        {
            // create ID for new package
            //string packageId = System.Guid.NewGuid().ToString();

            var conn = Connect();
            var sql = "INSERT INTO swe1_mtcg.package (id, name) VALUES (@id, @name)";

            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.Add(new NpgsqlParameter("@id", packageId));
            cmd.Parameters.Add(new NpgsqlParameter("@name", name));
            cmd.Prepare();

            if (cmd.ExecuteNonQuery() == 1)
            {
                //Console.WriteLine("Package {0} ({1}) wurde erfolgreich hinzugefügt.", packageId, packageName);
                return true;
            }

            conn.Close();
            return false;
        }

        private bool AddCardToPackage(string packageId, string cardId, string cardName)
        {
            bool success = false;

            var conn = Connect();
            var sql = "INSERT INTO swe1_mtcg.package_has_cards (pkg_id, card_id) VALUES (@pkg_id, @card_id)";

            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.Add(new NpgsqlParameter("@pkg_id", packageId));
            cmd.Parameters.Add(new NpgsqlParameter("@card_id", cardId));
            cmd.Prepare();

            if (cmd.ExecuteNonQuery() == 1)
            {
                Console.WriteLine("'{0}' (ID: {1})", cardName, cardId);
                success = true;
            }

            conn.Close();
            return success;
        }

        private string CreateRandomName(string type)
        {
            var attributes = new List<string> { "Cool", "Fancy", "Crazy", "Funny", "Strong", "Mighty", "Lazy", "Great", "Winner", "Loser" };
            var rand = new Random();
            int index = rand.Next(attributes.Count);
            string name = attributes[index] + " " + type;

            return name;
        }

        private bool OwnsPackageAlready(string packageId, int userId)
        {
            bool success = false;
            var conn = Connect();
            var sql = "SELECT * FROM swe1_mtcg.user_owns_packages WHERE user_id = @userID AND package_id = @packageID";

            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.Add(new NpgsqlParameter("@userID", userId));
            cmd.Parameters.Add(new NpgsqlParameter("@packageID", packageId));
            cmd.Prepare();

            var reader = cmd.ExecuteReader();
            success = reader.HasRows;
            conn.Close();
            return success;
        }

        private bool OwnsCardAlready(string cardId, string username)
        {
            bool success = false;
            var conn = Connect();
            var sql = "SELECT card_id, name, damage, element, type FROM swe1_mtcg.\"user\" JOIN swe1_mtcg.user_owns_packages uop on \"user\".id = uop.user_id JOIN swe1_mtcg.package_has_cards phc on uop.package_id = phc.pkg_id JOIN swe1_mtcg.card c on phc.card_id = c.id WHERE username = @username AND card_id = @card_id";


            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.Add(new NpgsqlParameter("@card_id", cardId));
            cmd.Parameters.Add(new NpgsqlParameter("@username", username));
            cmd.Prepare();

            var reader = cmd.ExecuteReader();
            success = reader.HasRows;
            conn.Close();
            return success;
        }

        private bool DoesPackageExist(string packageId)
        {
            bool success = false;
            var conn = Connect();
            var sql = "SELECT * FROM swe1_mtcg.package WHERE id = @id";

            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.Add(new NpgsqlParameter("@id", packageId));
            cmd.Prepare();

            var reader = cmd.ExecuteReader();
            success = reader.HasRows;
            conn.Close();
            return success;
        }

        private int GetUserID(string username)
        {
            int userID = -1;
            var conn = Connect();
            var sql = "SELECT id FROM swe1_mtcg.\"user\" WHERE username = @username";
            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.Add(new NpgsqlParameter("@username", username));
            cmd.Prepare();

            var reader = cmd.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    userID = reader.GetInt32(0);
                }
            }
            conn.Close();
            return userID;
        }

        private bool CanAffordPackage(string packageId, int userId)
        {
            int availableCoins = -1;
            var conn = Connect();
            var sql = "SELECT coins FROM swe1_mtcg.\"user\" WHERE id = @userID";

            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.Add(new NpgsqlParameter("@userID", userId));
            cmd.Prepare();

            var reader = cmd.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    availableCoins = reader.GetInt32(0);
                }
            }
            conn.Close();
            return availableCoins >= 5; // 5 is currently the price of every package

        }

        private bool ExecutePurchase(string packageId, int userId)
        {
            var conn = Connect();
            var sql = "INSERT INTO swe1_mtcg.user_owns_packages (user_id, package_id) VALUES (@user_id, @package_id)";

            using var cmdInsert = new NpgsqlCommand(sql, conn);
            cmdInsert.Parameters.Add(new NpgsqlParameter("@user_id", userId));
            cmdInsert.Parameters.Add(new NpgsqlParameter("@package_id", packageId));
            cmdInsert.Prepare();

            if (cmdInsert.ExecuteNonQuery() != 1)
            {
                Console.WriteLine("Kauf nicht erfolgreich.");
                conn.Close();
                return false;
            }

            // add cards from package to user stack
            var cardList = GetCardsFromPackage(packageId);
            foreach (var card in cardList)
            {
                AddCardToStack(card.Id, userId);
            }

            // update available coins
            sql = "UPDATE swe1_mtcg.\"user\" SET coins=coins-5 WHERE id = @id";
            using var cmdUpdate = new NpgsqlCommand(sql, conn);
            cmdUpdate.Parameters.Add(new NpgsqlParameter("@id", userId));
            cmdUpdate.Prepare();

            if (cmdUpdate.ExecuteNonQuery() != 1)
            {
                Console.WriteLine("Fehler bei Coins-Update.");
                conn.Close();
                return false;
            }

            conn.Close();
            return true;
        }

        private List<ICard> GetCardsFromPackage(string packageId)
        {
            List<ICard> myCards = new List<ICard>();

            var conn = Connect();
            var sql = "SELECT card_id, c.name, damage, element, type FROM swe1_mtcg.package JOIN swe1_mtcg.package_has_cards phc on package.id = phc.pkg_id JOIN swe1_mtcg.card c on phc.card_id = c.id WHERE pkg_id = @pkg_id";
            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.Add(new NpgsqlParameter("@pkg_id", packageId));

            var reader = cmd.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    string cardId = reader.GetString(0);
                    string cardName = reader.GetString(1);
                    float damage = reader.GetFloat(2);
                    string elementType = reader.GetString(3);
                    string monsterType = reader.GetString(4);
                    var card = InitalizeCardAsObject(cardId, cardName, damage, elementType, monsterType);
                    myCards.Add(card);
                }
            }

            conn.Close();
            return myCards;
        }

        private bool AddCardToStack(string cardId, int userId)
        {
            var conn = Connect();
            var sql = "INSERT INTO swe1_mtcg.user_owns_cards (user_id, card_id) VALUES (@user_id, @card_id)";

            using var cmdInsert = new NpgsqlCommand(sql, conn);
            cmdInsert.Parameters.Add(new NpgsqlParameter("@user_id", userId));
            cmdInsert.Parameters.Add(new NpgsqlParameter("@card_id", cardId));
            cmdInsert.Prepare();

            if (cmdInsert.ExecuteNonQuery() != 1)
            {
                conn.Close();
                return false;
            }

            conn.Close();
            return true;
        }

        private bool TransferCard(string usernameSender, string cardId, string usernameReceiver)
        {
            // add to receiver's cards
            var conn = Connect();
            var sql = "INSERT INTO swe1_mtcg.user_owns_cards (card_id, user_id) VALUES (@card_id, @user_id)";
            using var cmdInsert = new NpgsqlCommand(sql, conn);
            cmdInsert.Parameters.Add(new NpgsqlParameter("@card_id", cardId));
            cmdInsert.Parameters.Add(new NpgsqlParameter("@user_id", GetUserID(usernameReceiver)));
            cmdInsert.Prepare();

            if (cmdInsert.ExecuteNonQuery() != 1)
            {
                Console.WriteLine("User besitzt diese Karte bereits.");
                conn.Close();
                return false;
            }

            // delete from senders cards
            sql = "DELETE FROM swe1_mtcg.user_owns_cards WHERE card_id=@card_id AND user_id=@user_id";

            using var cmdDelete = new NpgsqlCommand(sql, conn);
            cmdDelete.Parameters.Add(new NpgsqlParameter("@card_id", cardId));
            cmdDelete.Parameters.Add(new NpgsqlParameter("@user_id", GetUserID(usernameSender)));
            cmdDelete.Prepare();

            if (cmdDelete.ExecuteNonQuery() <= 0)
            {
                conn.Close();
                return true;
            }

            return false;
        }

    }


}



