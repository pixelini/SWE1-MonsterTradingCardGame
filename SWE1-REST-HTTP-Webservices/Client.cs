using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace SWE1_REST_HTTP_Webservices
{
    class Client : IClient
    {
        private TcpClient Connection { get; set; }

        public Client(TcpClient connection)
        {
            Connection = connection;
        }

        public RequestContext ReceiveRequest()
        {
            NetworkStream dataStream = Connection.GetStream();

            byte[] buffer = new byte[2048];
            int bufferRead = dataStream.Read(buffer, 0, buffer.Length);
            string input = null;
            input = Encoding.ASCII.GetString(buffer, 0, bufferRead); // get string from byte array

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Received Data: \n" + input + "\n");
            Console.ForegroundColor = ConsoleColor.White;

            RequestContext myRequest = IEndpointHandler.ParseRequest(input);

            return myRequest;
        }

        public void SendResponse(Response response)
        {
            NetworkStream dataStream = Connection.GetStream();

            // Writing response to the client
            Console.ForegroundColor = ConsoleColor.Green;
            //response.Print();
            Console.WriteLine(response.ToString());
            Console.ForegroundColor = ConsoleColor.White;

            byte[] responseData = Encoding.ASCII.GetBytes(response.ToString());

            try
            {
                dataStream.Write(responseData, 0, responseData.Length);
                dataStream.Close();
                Connection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

        }



    }
}
