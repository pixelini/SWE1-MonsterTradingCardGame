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

        public HttpServer(IPAddress addr, int port)
        {
            Listener = new TcpListener(addr, port);
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

            Console.WriteLine("Request: \n" + input);

            RequestContext req = RequestContext.ParseRequest(input);
            req.Print();

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
