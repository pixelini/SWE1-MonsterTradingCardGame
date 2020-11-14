using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace SWE1_REST_HTTP_Webservices
{
    class EndpointHandler
    {
        public string MessagePath { get; set; }
        List<Message> Messages;
        private int Counter;

        public EndpointHandler(string messagePath, ref List<Message> messages)
        {
            MessagePath = messagePath;
            Messages = messages;
            Counter = 0;
        }
        
        public RequestContext ParseRequest(string data)
        {
            // get lines from whole input
            string[] lines = data.Split('\r', '\n');

            // check if client called a valid request (only first line, not the payload)
            // (with valid http method and path which is accessible for server)
            string firstLine = lines[0];
            string[] partsFirstLine = firstLine.Split(' ');
            string method = partsFirstLine[0]; // GET
            string resource = partsFirstLine[1]; // /messages/1
            string version = partsFirstLine[2]; // HTTP/1.1

            Action action = IsValidRequest(method, resource);

            if (action == Action.UNDEFINED)
            {
                // send status --> 400 BAD RESPONSE
                return null;
            }

            /*foreach (var partFirstLine in partsFirstLine)
            {
                Console.WriteLine($"<{partFirstLine}>");
            }*/


            // all headers (starting with index 2; every even index; if empty stop)
            Dictionary<string, string> headers = new Dictionary<string, string>();

            int i = 2;
            bool hasPayload = false;
            int indexPayload = -1; // helps to find out where payload starts
            while (lines[i] != "")
            {
                string[] splittedHeaders = lines[i].Split(':', 2);
                headers.Add(splittedHeaders[0], splittedHeaders[1]); // Exception?
                i += 2;

                if ((lines[i] == "") && (action == Action.ADD || action == Action.UPDATE))
                {
                    hasPayload = true;
                    indexPayload = i + 2;
                }
            }

            string payload = null;

            if (hasPayload)
            {
                // get the payload in one string
                StringBuilder sb = new StringBuilder();
                while (indexPayload < lines.Length)
                {
                    sb.Append(lines[indexPayload]);
                    sb.Append('\n');
                    indexPayload++;
                }
                payload = sb.ToString();
            }
            else
            {
                payload = null;
            }

            return new RequestContext(method, resource, version, headers, payload, action);

        }
        
        public void HandleRequest(RequestContext req)
        {
            if (req != null)
            {
                switch (req.Action)
                {
                    case Action.LIST:
                        HandleList(req);
                        break;
                    case Action.ADD:
                        HandleAdd(req);
                        break;
                    case Action.READ:
                        HandleRead(req);
                        break;
                    case Action.UPDATE:
                        HandleUpdate(req);
                        break;
                    case Action.DELETE:
                        HandleDelete(req);
                        break;
                    default:
                        Console.WriteLine("Request in not valid");
                        break;
                }
            } else
            {
                Console.WriteLine("Object is not initialised.");
            }
            

        }

        private void HandleList(RequestContext req)
        {
            Console.WriteLine("\nHandle List...\n");
            foreach (var message in Messages)
            {
                Console.WriteLine(message.ID + ": " + message.Content);
            }
        }

        private void HandleAdd(RequestContext req)
        {
            Console.WriteLine("\nHandle Add...\n");
            Counter++;
            Message inputMessage = new Message(Counter, req.Payload);
            Messages.Add(inputMessage);
            Console.WriteLine("New message added.\n");
        }

        private void HandleRead(RequestContext req)
        {
            Console.WriteLine("\nHandle Read...\n");
            bool msgFound = false;
            int msgID = GetMsgIDFromPath(req.ResourcePath); 

            // search in messages if message id exists
            foreach (var message in Messages)
            {
                if (message.ID == msgID)
                {
                    msgFound = true;
                    message.Print();
                }
            }

            if (!msgFound)
            {
                // message not found! 404?
                Console.WriteLine("Message not found! Reading not possible!\n");
            }

        }

        private void HandleUpdate(RequestContext req)
        {
            Console.WriteLine("\nHandle Update...\n");
            bool msgFound = false;
            int msgID = GetMsgIDFromPath(req.ResourcePath);

            // search in messages if message id exists
            foreach (var message in Messages)
            {
                if (message.ID == msgID)
                {
                    msgFound = true;
                    message.Update(req.Payload);
                }
            }

            if (!msgFound)
            {
                // message not found! 404?
                Console.WriteLine("Message not found! Updating not possible!\n");
            }

        }

        private void HandleDelete(RequestContext req)
        {
            Console.WriteLine("\nHandle Delete...\n");
            bool msgFound = false;
            int msgID = GetMsgIDFromPath(req.ResourcePath);

            // search in messages if message id exists
            for (int i = 0; i < Messages.Count; i++)
            {
                if (Messages[i].ID == msgID)
                {
                    //Console.WriteLine("TO DELETE: " + Messages[i].Content);
                    msgFound = true;
                    Messages.Remove(Messages[i]);
                }           
            }

            if (!msgFound)
            {
                Console.WriteLine("Message not found! Deleting not possible!\n");
            }
        
        }

        private Action IsValidRequest(string method, string resource)
        {
            // check if first line contains required information for all http methods
            if (method == "GET" && resource == MessagePath)
            {
                return Action.LIST;
            } else if (method == "POST" && resource == MessagePath)
            {
                return Action.ADD;
            } else if (method == "GET" && IsValidPathWithMsgID(resource))
            {
                return Action.READ;
            } else if (method == "PUT" && IsValidPathWithMsgID(resource))
            {
                return Action.UPDATE;
            } else if (method == "DELETE" && IsValidPathWithMsgID(resource))
            {
                return Action.DELETE;
            } else
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
            } else
            {
                Console.WriteLine("Path is not valid!");
            }

            return false;            

        }

        private int GetMsgIDFromPath(string path)
        {
            string msgName = System.IO.Path.GetFileName(path);
            int msgID = Int32.Parse(msgName); // is possible because regex has already validated path and message number
            return msgID;
        }



    }


   
}
