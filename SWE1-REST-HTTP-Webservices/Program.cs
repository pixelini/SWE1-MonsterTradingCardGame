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
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Starting server...");
            List<Message> messages = new List<Message>();

            string pathToMessages = "/messages";
            HttpServer server = new HttpServer(IPAddress.Loopback, 6789, pathToMessages, ref messages);
            server.Run();


        }

    }

}
