using System;
using System.Collections.Concurrent;
using System.Net;
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
                var allBattles = new ConcurrentBag<Battle>();
                var server = new HttpServer(IPAddress.Loopback, 10001, ref allBattles);
                server.Run();
            }
            catch (Exception ex)
            {
                Console.WriteLine("There was a problem: " + ex.Message);
            }
            


        }

    }

}
