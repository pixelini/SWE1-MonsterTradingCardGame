using System;
using System.Data.SqlTypes;
using System.Runtime.InteropServices.ComTypes;
using Mtcg;
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

        public NpgsqlConnection Connect()
        {
            var connString = $"Host={Host};Username={Username};Password={Password};Database={Name}";
            Console.WriteLine(connString);
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

            conn.Close();
        }

        public void RegisterUser(string username, string password)
        {

        }

    }

   
}