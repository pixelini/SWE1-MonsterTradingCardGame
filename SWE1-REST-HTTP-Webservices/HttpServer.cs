﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;

namespace SWE1_REST_HTTP_Webservices
{
    public class HttpServer
    {
        public bool Running = false;
        private readonly IListener _listener;
        private readonly IEndpointHandler _endpointHandler;
        public string MessagePath { get; set; }


        public HttpServer(IPAddress addr, int port, string messagePath, ref List<Message> messages)
        {
            try
            {
                _listener = new Listener(addr, port);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                Console.WriteLine("Problem with arguments" + ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("There was a problem: " + ex.Message);
            }

            MessagePath = messagePath;
            _endpointHandler = new EndpointHandler(ref messages);

        }

        // only for mocking purpose
        public HttpServer(IPAddress addr, int port, string messagePath, IEndpointHandler endpointHandler)
        {
            _listener = new Listener(addr, port);
            _endpointHandler = endpointHandler;
            MessagePath = messagePath;
        }

        public void Run()
        {
            try
            {
                _listener.Start();
                Running = true;

                while (Running)
                {
                    Console.WriteLine("\nWaiting for connection...");
                    IClient connection = _listener.AcceptTcpClient();
                    Console.WriteLine("Connected!\n");
                    ProcessRequest(connection);
                }

                Running = false;
                _listener.Stop();
            }
            catch (SocketException ex)
            {
                Console.WriteLine("Connection error occurred: " + ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("There was a problem: " + ex.Message);
            }
            
        }
        public void ProcessRequest(IClient connection)
        {
           
            RequestContext request = connection.ReceiveRequest();
            
            // Handle Request
            Action action = GetRequestedAction(request);

            if (request != null)
            {
                request.Action = action;
                request.Print(); // if not null, print! --> clever alternative: request?.Print(); :D
            }      
            Response response = _endpointHandler.HandleRequest(request);

            // Send Response
            connection.SendResponse(response);
        }

        public Action GetRequestedAction(RequestContext req)
        {
            if (req == null)
            {
                return Action.Undefined;
            }
            // check if first line contains required information for all http methods
            if (req.Method == HttpVerb.Get && req.ResourcePath == MessagePath)
            {
                return Action.List;
            }
            else if (req.Method == HttpVerb.Post && req.ResourcePath == MessagePath)
            {
                return Action.Add;
            }
            else if (req.Method == HttpVerb.Get && IsValidPathWithMsgID(req.ResourcePath))
            {
                return Action.Read;
            }
            else if (req.Method == HttpVerb.Put && IsValidPathWithMsgID(req.ResourcePath))
            {
                return Action.Update;
            }
            else if (req.Method == HttpVerb.Delete && IsValidPathWithMsgID(req.ResourcePath))
            {
                return Action.Delete;
            }
            else
            {
                Console.WriteLine("Not a valid request!");
                return Action.Undefined;
            }

        }

        private bool IsValidPathWithMsgID(string path)
        {
            // creating a regex pattern that looks like this, eg. path /messages --> "^\/messages\/\d+"
            StringBuilder regPattern = new StringBuilder();
            regPattern.Append(@"^\");
            regPattern.Append(MessagePath);
            regPattern.Append(@"\/\d+");

            string pattern = regPattern.ToString();
            Match m = Regex.Match(path, pattern, RegexOptions.IgnoreCase);
            if (m.Success)
            {
                //Console.WriteLine("Found regex '{0}'", m.Value);
                return true;
            }
            else
            {
                Console.WriteLine("Path is not valid!");
            }

            return false;

        }

    }
}
