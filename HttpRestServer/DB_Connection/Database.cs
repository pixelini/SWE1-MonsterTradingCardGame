using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Globalization;
using System.Runtime.InteropServices.ComTypes;
using Mtcg;
using Npgsql;
using NpgsqlTypes;
using NUnit.Framework;

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

        public NpgsqlConnection Connect()
        {
            var connString = $"Host={Host};Username={Username};Password={Password};Database={Name}";
            var conn = new NpgsqlConnection(connString);
            conn.Open();
            return conn;
        }

        public void Close(NpgsqlConnection conn)
        {
            conn.Close();
        }

        public void Testing()
        {
            var conn = Connect();
            var sql = "SELECT * FROM swe1_mtcg.card";
            using var cmd = new NpgsqlCommand(sql, conn);

            var reader = cmd.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    Console.WriteLine(reader.GetString(0));
                    Console.WriteLine(reader.GetFloat(1));
                    Console.WriteLine(reader.GetString(2));
                    Console.WriteLine(reader.GetString(3));
                    Console.WriteLine(reader.GetString(4));
                    //Console.Write("{0}\t{1} \n", reader[0], reader[1]);
                }
            }

            conn.Close();
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
            bool isAdmin = (username == "admin");
            var token = "Basic " + username + "-mtcgToken";
            var conn = Connect();
            var sql = "INSERT INTO swe1_mtcg.\"user\" (username, password, auth_token, is_admin) VALUES (@username, @password, @authToken, @isAdmin)";

            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.Add(new NpgsqlParameter("@username", username));
            cmd.Parameters.Add(new NpgsqlParameter("@password", password));
            cmd.Parameters.Add(new NpgsqlParameter("@authToken", token));
            cmd.Parameters.Add(new NpgsqlParameter("@isAdmin", isAdmin));
            cmd.Prepare();

            if (cmd.ExecuteNonQuery() == 1)
            {
                Console.WriteLine("User wurde erfolgreich registriert.");
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

            var conn = Connect();
            var sql = "SELECT * FROM swe1_mtcg.\"user\" WHERE username = @username AND password = @password";

            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.Add(new NpgsqlParameter("@username", username));
            cmd.Parameters.Add(new NpgsqlParameter("@password", password));
            cmd.Prepare();

            var reader = cmd.ExecuteReader();

            success = reader.HasRows;
            conn.Close();
            return success;
        }

        public bool AddPackage(dynamic cards)
        {
            bool success = false;

            // create new package and in table package
            string packageName = CreateRandomName("Package");
            string packageID = CreatePackage(packageName);
            Console.WriteLine("Package '{0}' (ID: {1}) wurde erstellt.\nFolgende Karten wurden hinzugefügt:", packageName, packageID);

            // extract elementType and cardType and save package in database
            foreach (var card in cards)
            {
                // adds card to table card
                success = AddCard(card.Id, card.Name, card.Damage);

                if (!success)
                {
                    return false;
                }

                // adds card to package in table package_has_cards
                success = AddCardToPackage(packageID, card.Id, card.Name);

                if (!success)
                {
                    return false;
                }
            }

            return success;
        }

        private bool DoesUserAlreadyExist(string username)
        {
            var conn = Connect();
            var sql = "SELECT * FROM swe1_mtcg.\"user\" WHERE username = @username";

            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.Add(new NpgsqlParameter("@username", username));
            cmd.Prepare();

            var reader = cmd.ExecuteReader();
            return reader.HasRows;
        }

        private bool DoesCardAlreadyExist(string id)
        {
            var conn = Connect();
            var sql = "SELECT * FROM swe1_mtcg.card WHERE id = @id";

            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.Add(new NpgsqlParameter("@id", id));
            cmd.Prepare();

            var reader = cmd.ExecuteReader();
            return reader.HasRows;
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

        private bool AddCard(string id, string name, string damage)
        {
            bool success = false;

            // check if card alreay exists
            if (DoesCardAlreadyExist(id))
            {
                Console.WriteLine("Karte (ID: {0}) existiert bereits", id);
                return success;
            }

            string cardType = ExtractCardType(name); // spell or monster
            var elementType = ExtractElementType(name); // fire, water or normal

            // casts damage-string to float
            float damageAsFloat;
            if (!float.TryParse(damage, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out damageAsFloat))
            {
                Console.WriteLine("Unable to parse '{0}' to numeric value float.", damage);
                success = false;
                return success;
            }

            var conn = Connect();
            var sql = "INSERT INTO swe1_mtcg.card (name, damage, element, type, id) VALUES (@name, @damage, @element, @type, @id)";

            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.Add(new NpgsqlParameter("@name", name));
            cmd.Parameters.Add(new NpgsqlParameter("@damage", damageAsFloat));
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

        private string CreatePackage(string name)
        {
            // create ID for new package
            string packageId = System.Guid.NewGuid().ToString();

            var conn = Connect();
            var sql = "INSERT INTO swe1_mtcg.package (id, name) VALUES (@id, @name)";

            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.Add(new NpgsqlParameter("@id", packageId));
            cmd.Parameters.Add(new NpgsqlParameter("@name", name));
            cmd.Prepare();

            if (cmd.ExecuteNonQuery() == 1)
            {
                //Console.WriteLine("Package {0} ({1}) wurde erfolgreich hinzugefügt.", packageId, packageName);
                return packageId;
            }

            conn.Close();

            return null;
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

    }






}



