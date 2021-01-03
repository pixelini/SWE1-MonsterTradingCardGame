using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Mtcg.Cards;

namespace Mtcg
{
    class Program
    {
        static void Main(string[] args)
        {




            Guid myhash = System.Guid.NewGuid();
            Guid myhash1 = System.Guid.NewGuid();
            Guid myhash2 = System.Guid.NewGuid();
            Console.WriteLine(myhash);
            Console.WriteLine(myhash1);
            Console.WriteLine(myhash2);






            



            //try
            //{
            //    Console.ForegroundColor = ConsoleColor.White;
            //    Console.WriteLine("Starting server...");
            //    List<Message> messages = new List<Message>();

            //    string pathToMessages = "/messages";
            //    HttpServer server = new HttpServer(IPAddress.Loopback, 10001, pathToMessages, ref messages);
            //    server.Run();
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine("There was a problem: " + ex.Message);
            //}


            

        }

    }
}
