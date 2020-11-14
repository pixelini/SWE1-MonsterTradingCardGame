using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace SWE1_REST_HTTP_Webservices
{
    class HttpServer
    {
        public bool Running = false;
        private TcpListener Listener;
        private EndpointHandler EndpointHandler;
        

        public HttpServer(IPAddress addr, int port, string messagePath, ref List<Message> messages)
        {
            Listener = new TcpListener(addr, port);
            EndpointHandler = new EndpointHandler(messagePath, ref messages);
        }

        public void Run()
        {
            Listener.Start();
            Running = true;

            while (Running)
            {
                Console.WriteLine("\nWaiting for connection...");
                TcpClient connection = Listener.AcceptTcpClient();
                Console.WriteLine("Connected!\n");

                ReceiveRequests(connection);
                WriteResponse(connection);
                connection.Close();
            }

            Running = false;
            Listener.Stop();
        }


        private void ReceiveRequests(TcpClient connection)
        {
            NetworkStream dataStream = connection.GetStream();

            byte[] buffer = new byte[2048];
            int bufferRead = dataStream.Read(buffer, 0, buffer.Length);
            string input = null;
            input = Encoding.ASCII.GetString(buffer, 0, bufferRead); // get string from byte array

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Received Data: \n" + input);
            Console.ForegroundColor = ConsoleColor.White;

            RequestContext req = EndpointHandler.ParseRequest(input);
            EndpointHandler.HandleRequest(req);

            Console.ForegroundColor = ConsoleColor.Green;
            req.Print();
            Console.ForegroundColor = ConsoleColor.White;

        }

        private void WriteResponse(TcpClient connection)
        {
            NetworkStream dataStream = connection.GetStream();

            // Writing response to the client
            string text = "This is my response"; // later: create Response!

            byte[] response = Encoding.ASCII.GetBytes(text);

            try
            {
                dataStream.Write(response, 0, response.Length);
                dataStream.Close();
                connection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

        }
    }
}
