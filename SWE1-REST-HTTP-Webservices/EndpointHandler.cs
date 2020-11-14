using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace SWE1_REST_HTTP_Webservices
{
    class EndpointHandler
    {
        RequestContext Requestdata;
        public string MessagePath { get; set; }
        List<Message> Messages;
        private int Counter;

        public EndpointHandler(string messagePath, ref List<Message> messages)
        {
            MessagePath = messagePath;
            Messages = messages;
            Counter = 0;
            RequestContext Requestdata = null;
        }

        public bool HandleRequest(RequestContext req)
        {
            HttpVerb reqType = req.Method;

            if (reqType == HttpVerb.GET && req.ResourcePath == MessagePath && req.Payload == null)
            {
                // List
                HandleList(req);

            } else if (reqType == HttpVerb.POST && req.Payload != null)
            {
                // Add
                HandleAdd(req);

            } else if (reqType == HttpVerb.GET && req.ResourcePath != MessagePath && req.Payload == null)
            {
                // Read
                HandleRead(req);
            }
            else if (reqType == HttpVerb.PUT && req.Payload != null)
            {
                // Update
                HandleUpdate(req);
            }
            else if (reqType == HttpVerb.DELETE && req.Payload == null)
            {
                // Delete
                HandleDelete(req);

            } else
            {
                Console.WriteLine("Request in not valid");
                return false;
            }

            return true;


        }

        private void HandleList(RequestContext req)
        {
            // List means: GET /messages
            if (req.ResourcePath == MessagePath)
            {
                Console.WriteLine("Handle list");

                foreach (var message in Messages)
                {
                    Console.WriteLine(message.ID + ": " + message.Content);
                }
            }

        }

        private void HandleAdd(RequestContext req)
        {
            // List means: POST /messages & Payload
            if (req.ResourcePath == MessagePath)
            {
                Counter++;
                Message inputMessage = new Message(Counter, req.Payload);
                Messages.Add(inputMessage);
                Console.WriteLine("New message added.");
            }

        }

        private void HandleRead(RequestContext req)
        {
            
            string filename = System.IO.Path.GetFileName(req.ResourcePath);
            string dirname = req.ResourcePath.Substring(0, 9);

            //string dirname = System.IO.Path.GetDirectoryName(req.ResourcePath);
            //string filename = test2.Substring(10, test2.Length - 10);

            //Console.WriteLine("Dirname: " + dirname);
            //Console.WriteLine("Filename: " + filename);

            // must be a number - exception?
            int msgID = Int32.Parse(filename);

            if (dirname == MessagePath)
            {
                Console.WriteLine("Handle read");

                // search in messages if message id exists
                foreach (var message in Messages)
                {
                    if (message.ID == msgID)
                    {
                        // found msg!
                        message.Print();
                    } else
                    {
                        // message not found! 404?
                    }
                }

            }

        }

        private void HandleUpdate(RequestContext req)
        {

            string filename = System.IO.Path.GetFileName(req.ResourcePath);
            string dirname = req.ResourcePath.Substring(0, 9);

            // must be a number - exception?
            int msgID = Int32.Parse(filename);

            if (dirname == MessagePath)
            {
                Console.WriteLine("Handle Update");

                // search in messages if message id exists
                foreach (var message in Messages)
                {
                    if (message.ID == msgID)
                    {
                        // found msg!
                        message.Update(req.Payload);
                    }
                    else
                    {
                        // message not found! 404?
                    }
                }

            }
        }

        private void HandleDelete(RequestContext req)
        {
            string filename = System.IO.Path.GetFileName(req.ResourcePath);
            string dirname = req.ResourcePath.Substring(0, 9);

            // must be a number - exception?
            int msgID = Int32.Parse(filename);

            if (dirname == MessagePath)
            {
                Console.WriteLine("Handle Delete");

                // search in messages if message id exists
                for (int i = 0; i < Messages.Count; i++)
                {
                    if (Messages[i].ID == msgID)
                    {
                        //Console.WriteLine("TO DELETE: " + Messages[i].Content);
                        Messages.Remove(Messages[i]);
                    }
                    
                }

            }
        }


        private bool IsValidRequest(string method, string resource)
        {

            // check if first line contains required information for all http methods

            if (method == "GET" && resource == MessagePath) // --> LIST ALL MESSAGES
            {
                return true;
            } else if (method == "POST" && resource == MessagePath) // --> ADD MESSAGE
            {
                return true;
            } else if (method == "GET" && isValidPathWithMsgID(resource)) // --> READ MESSAGE
            {
                return true;
            } else if (method == "PUT" && isValidPathWithMsgID(resource)) // --> UPDATE MESSAGE
            {
                return true;
            } else if (method == "DELETE" && isValidPathWithMsgID(resource)) // --> DELETE MESSAGE
            {
                return true;
            } else
            {
                Console.WriteLine("Not a valid request!");
                return false;
            }

        }

        private bool isValidPathWithMsgID(string path)
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
                Console.WriteLine("Found '{0}' at position {1}.", m.Value, m.Index);
                return true;
            }

            return false;            

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

            bool isValid = IsValidRequest(method, resource);

            if (!isValid)
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
            int indexPayload = -1;
            while (lines[i] != "")
            {
                string[] splittedHeaders = lines[i].Split(':', 2);
                headers.Add(splittedHeaders[0], splittedHeaders[1]); // Exception?
                i += 2;

                if (lines[i] == "")
                {
                    indexPayload = i + 2;
                }
            }

            // get the payload in one string
            StringBuilder sb = new StringBuilder();
            while (indexPayload < lines.Length)
            {
                sb.Append(lines[indexPayload]);
                sb.Append('\n');
                indexPayload++;
            }

            string payload = sb.ToString();

            return new RequestContext(method, resource, version, headers, payload);

        }



    }


   
}
