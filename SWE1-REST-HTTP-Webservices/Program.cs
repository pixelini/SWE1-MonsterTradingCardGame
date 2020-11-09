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

        // Program has HttpServer and messages resource

        static void Main(string[] args)
        {

            Console.WriteLine("Starting server...");
            HttpServer server = new HttpServer(IPAddress.Loopback, 6789);
            server.Run();

            Status myStatus = new Status(200, "OK");
            myStatus.Print();

            List<Message> resource = new List<Message>(); // all messages are stored in this list


        }
    }

}
