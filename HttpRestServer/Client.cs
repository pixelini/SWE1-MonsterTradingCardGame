﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace HttpRestServer
{
    class Client : IClient
    {
        private TcpClient _connection { get; set; }


        public Client(TcpClient connection)
        {
            _connection = connection;
        }

        public RequestContext ReceiveRequest()
        {
            try
            {
                NetworkStream dataStream = _connection.GetStream();

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
            catch (Exception ex)
            {
                Console.WriteLine("There was a problem: " + ex.Message);
            }

            return null;

        }

        public void SendResponse(Response response)
        {
            try
            {
                NetworkStream dataStream = _connection.GetStream();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(response.ConvertToString());
                Console.ForegroundColor = ConsoleColor.White;

                byte[] responseData = Encoding.ASCII.GetBytes(response.ConvertToString());

                // Writing response to the client
                dataStream.Write(responseData, 0, responseData.Length);
                dataStream.Close();
                _connection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Sending response causes a problem: " + ex.Message);
            }

        }

    }
}
