using System;
using System.Collections.Concurrent;
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


        public HttpServer(IPAddress addr, int port, string messagePath, ref List<Message> messages, ref ConcurrentBag<Battle> allBattles)
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

        public async void Run()
        {
            try
            {
                _listener.Start();
                Running = true;

                while (Running)
                {
                    Console.WriteLine("\nWaiting for connection...");
                    List<Task> tasks = new List<Task>();
                    if (tasks.Count > 10)
                    {
                        var array = tasks.ToArray();
                        int taskIndex = Task.WaitAny((array));
                        await array[taskIndex];
                        tasks.Remove(array[taskIndex]);
                    }

                    
                    
                    IClient connection = _listener.AcceptTcpClient();
                    Task clientTask = Task.Run(() => ProcessRequest(connection));
                    tasks.Add(clientTask);
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
            if (req.Method == HttpVerb.Post) // POST
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
                        return (req.ResourcePath.Substring(0, 9) == "/tradings" && req.ResourcePath.Length == 46) ? Action.DoTrading : Action.Undefined;
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
                        return req.ResourcePath.Substring(0, 6) == "/users" ? Action.ShowProfile : Action.Undefined;
                }
            }
            else if (req.Method == HttpVerb.Put)
            {
                switch (req.ResourcePath)
                {
                    case "/deck":
                        return Action.ConfigureDeck;
                    default:
                        return req.ResourcePath.Substring(0, 6) == "/users" ? Action.EditProfile : Action.Undefined;
                }
            }
            else if (req.Method == HttpVerb.Delete)
            {
                if (req.ResourcePath.Substring(0, 9) == "/tradings" && req.ResourcePath.Length == 46)
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

    }
}
