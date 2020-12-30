using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Mtcg.Cards;
using HttpRestServer;

namespace Mtcg
{
    class Program
    {
        static void Main(string[] args)
        {


            Goblin gobo = new Goblin("WaterGoblin", (float)50.0); //"e85e3976-7c86-4d06-9a80-641c2019a79f"
            Spell spell = new Spell("RegularSpell", (float)50.0);

            Console.WriteLine(gobo);
            Console.WriteLine(gobo.Name);
            Console.WriteLine(gobo.Damage);
            Console.WriteLine(gobo.ElementType);
            Console.WriteLine(spell);
            Console.WriteLine(spell.Name);
            Console.WriteLine(spell.Damage);
            Console.WriteLine(spell.ElementType);


            Guid myhash = System.Guid.NewGuid();
            Guid myhash1 = System.Guid.NewGuid();
            Guid myhash2 = System.Guid.NewGuid();
            Console.WriteLine(myhash);
            Console.WriteLine(myhash1);
            Console.WriteLine(myhash2);



            User elisabeth = new User();
            Console.WriteLine(elisabeth);



            



            try
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("Starting server...");
                List<Message> messages = new List<Message>();

                string pathToMessages = "/messages";
                HttpServer server = new HttpServer(IPAddress.Loopback, 10001, pathToMessages, ref messages);
                server.Run();
            }
            catch (Exception ex)
            {
                Console.WriteLine("There was a problem: " + ex.Message);
            }


            

        }

    }
}
