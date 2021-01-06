using System;
using System.Collections.Concurrent;
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

            try
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("Starting server...");
                var messages = new List<Message>();
                var allBattles = new ConcurrentBag<Battle>();

                var pathToMessages = "/messages";
                var server = new HttpServer(IPAddress.Loopback, 10001, pathToMessages, ref messages, ref allBattles);
                server.Run();
            }
            catch (Exception ex)
            {
                Console.WriteLine("There was a problem: " + ex.Message);
            }
            


        }

    }

}
