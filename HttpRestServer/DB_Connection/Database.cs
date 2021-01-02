using System;
using System.Data.SqlTypes;
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

            /*
            var sql = "select * from swe1_mtcg.card";
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
            */

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

    }

   
}