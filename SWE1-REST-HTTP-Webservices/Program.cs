using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Runtime.InteropServices;
using System.Text;

namespace SWE1_REST_HTTP_Webservices
{
    class Program
    {

        static void Main(string[] args)
        {

            Console.WriteLine("Starting server...");
            List<Message> messages = new List<Message>(); // all messages are stored in this list

            Message msg1 = new Message(1, "Hi du Ei!");
            Message msg2 = new Message(2, "Hi du Frau!");
            Message msg3 = new Message(3, "Hi du Mann!");

            //messages.Add(msg1);
            //messages.Add(msg2);
            //messages.Add(msg3);


            string pathToMessages = "/messages";
            HttpServer server = new HttpServer(IPAddress.Loopback, 6789, pathToMessages, ref messages);
            server.Run();

            

            Response res = new Response();
            res.Status = 200;
            res.StatusMessage = "OK";
            res.Print();

            


        }
    }

}
