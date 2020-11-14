using System;
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
        private IListener Listener;
        private IEndpointHandler EndpointHandler;
        public string MessagePath { get; set; }


        public HttpServer(IPAddress addr, int port, string messagePath, ref List<Message> messages)
        {
            Listener = new Listener(addr, port);
            MessagePath = messagePath;
            EndpointHandler = new EndpointHandler(ref messages);
        }

        // for mocking
        public HttpServer(IPAddress addr, int port, string messagePath, IEndpointHandler endpointHandler)
        {
            Listener = new Listener(addr, port);
            EndpointHandler = endpointHandler;
            MessagePath = messagePath;
        }

        public void Run()
        {
            Listener.Start();
            Running = true;

            while (Running)
            {
                Console.WriteLine("\nWaiting for connection...");
                IClient connection = Listener.AcceptTcpClient();
                Console.WriteLine("Connected!\n");
                ProcessRequest(connection);
            }

            Running = false;
            Listener.Stop();
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
            Response response = EndpointHandler.HandleRequest(request);

            // Send Response
            connection.SendResponse(response);
        }


        public Action GetRequestedAction(RequestContext req)
        {
            if (req == null)
            {
                return Action.UNDEFINED;
            }
            // check if first line contains required information for all http methods
            if (req.Method == HttpVerb.GET && req.ResourcePath == MessagePath)
            {
                return Action.LIST;
            }
            else if (req.Method == HttpVerb.POST && req.ResourcePath == MessagePath)
            {
                return Action.ADD;
            }
            else if (req.Method == HttpVerb.GET && IsValidPathWithMsgID(req.ResourcePath))
            {
                return Action.READ;
            }
            else if (req.Method == HttpVerb.PUT && IsValidPathWithMsgID(req.ResourcePath))
            {
                return Action.UPDATE;
            }
            else if (req.Method == HttpVerb.DELETE && IsValidPathWithMsgID(req.ResourcePath))
            {
                return Action.DELETE;
            }
            else
            {
                Console.WriteLine("Not a valid request!");
                return Action.UNDEFINED;
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
