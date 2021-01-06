using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Mtcg;

namespace HttpRestServer
{
    public class HttpServer
    {
        public bool Running = false;
        private readonly IListener _listener;
        private readonly IEndpointHandler _endpointHandler;
        public string MessagePath { get; set; }


        public HttpServer(IPAddress addr, int port, string messagePath, ref List<Message> messages, ref List<Battle> allBattles)
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
            _endpointHandler = new EndpointHandler(ref messages, ref allBattles);

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
                    Task taskA = new Task(() => ProcessRequest(connection));
                    // Start the task.
                    taskA.Start();
                    Console.WriteLine("Connected!\n");
                    
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
            else if (req.Method == HttpVerb.Get && req.ResourcePath == "/message/")
            {
                return Action.Read;
            }
            else if (req.Method == HttpVerb.Put && req.ResourcePath == "/message/5")
            {
                return Action.Update;
            }
            else if (req.Method == HttpVerb.Delete && req.ResourcePath == "/message/")
            {
                return Action.Delete;
            } // HERE IS THE NEW IMPLEMENTATION
            else if (req.Method == HttpVerb.Post) // POST
            {
                switch (req.ResourcePath)
                {
                    case "/users":
                        return Action.Registration;
                    case "/sessions":
                        return Action.Login;
                    case "/packages":
                        return Action.AddPackage;
                    case "/transactions/packages":
                        return Action.BuyPackage;
                    case "/tradings":
                        return Action.CreateDeal;
                    case "/battles":
                        return Action.JoinBattle;
                    default:
                        return Action.Undefined;
                }
            }
            else if (req.Method == HttpVerb.Get)
            {
                switch (req.ResourcePath)
                {
                    case "/cards":
                        return Action.ShowCards;
                    case "/deck":
                        return Action.ShowDeck;
                    case "/deck?format=plain":
                        return Action.ShowDeckInPlainText;
                    case "/stats":
                        return Action.ShowStats;
                    case "/score":
                        return Action.ShowScoreboard;
                    case "/tradings":
                        return Action.ShowDeals;
                    default:
                        // is it: /users/<username> ? does the username exist? check!
                        if (true)
                        {
                            return Action.ShowProfile;
                        }
                        return Action.Undefined;
                }
            }
            else if (req.Method == HttpVerb.Put)
            {
                switch (req.ResourcePath)
                {
                    case "/deck":
                        return Action.ConfigureDeck;
                    default:
                        // is it: /users/<username> ? does the username exist? check!
                        if (true)
                        {
                            return Action.EditProfile;
                        }
                        return Action.Undefined;
                }
            }
            else if (req.Method == HttpVerb.Delete)
            {
                // is it: /tradings/<card-id> ?
                if (true)
                {
                    return Action.DeleteDeal;
                }
                return Action.Undefined;
            }
            else
            {
                Console.WriteLine("Not a valid request!");
                return Action.Undefined;
            }

        }

        private bool IsValidPathWithMsgID(string path)
        {
            // zwischenloesung
            return true;


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
