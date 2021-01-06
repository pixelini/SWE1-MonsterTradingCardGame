using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Runtime.InteropServices;
using System.Text;
using HttpRestServer.DB_Connection;
using Mtcg;

namespace HttpRestServer
{
    class Program
    {

        static void Main(string[] args)
        {
            Console.WriteLine("Hello World");
            var myDB = new Database();
            //Console.WriteLine(myDb);
            //myDb.Testing();
            //myDb.RegisterUser("lisi", "glatz");
            
            try
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("Starting server...");
                List<Message> messages = new List<Message>();
                List<Battle> allBattles = new List<Battle>();

                string pathToMessages = "/messages";
                HttpServer server = new HttpServer(IPAddress.Loopback, 10001, pathToMessages, ref messages, ref allBattles);
                server.Run();
            }
            catch (Exception ex)
            {
                Console.WriteLine("There was a problem: " + ex.Message);
            }
            


        }

    }

}
