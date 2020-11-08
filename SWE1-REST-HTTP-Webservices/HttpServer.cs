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
        public bool running = false;
        private TcpListener listener;

        public HttpServer(IPAddress addr, int port)
        {
            listener = new TcpListener(addr, port);
        }

        public void Run()
        {
            listener.Start();
            running = true;

            while (running)
            {
                Console.WriteLine("\nWaiting for connection...");
                TcpClient connection = listener.AcceptTcpClient();
                Console.WriteLine("Connected!\n");

                ReceiveRequests(connection);
                WriteResponse(connection);
                connection.Close();
            }

            running = false;
            listener.Stop();
        }

        private void ReceiveRequests(TcpClient connection)
        {
            NetworkStream dataStream = connection.GetStream();

            byte[] buffer = new byte[2048];
            int bufferRead = dataStream.Read(buffer, 0, buffer.Length);
            string input = null;
            input = Encoding.ASCII.GetString(buffer, 0, bufferRead); // get string from byte array

            Console.WriteLine("Request: \n" + input);

            //RequestContext req = RequestContext.GetRequest(msg);
            //Response res = Response.From(req);
            //res.Post(client.GetStream());
        }

        private void WriteResponse(TcpClient connection)
        {
            NetworkStream dataStream = connection.GetStream();

            // Writing response to the client
            string text = "This is my response";
            byte[] response = Encoding.ASCII.GetBytes(text);

            try
            {
                dataStream.Write(response, 0, response.Length);
                dataStream.Close();
                connection.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            //RequestContext req = RequestContext.GetRequest(msg);
            //Response res = Response.From(req);
            //res.Post(client.GetStream());
        }
    }
}
