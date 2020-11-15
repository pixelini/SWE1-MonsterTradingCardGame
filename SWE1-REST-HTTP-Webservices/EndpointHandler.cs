using System;
using System.Collections.Generic;
using System.Text;

namespace SWE1_REST_HTTP_Webservices
{
    public class EndpointHandler : IEndpointHandler
    {
        List<Message> Messages;
        private int Counter;


        public EndpointHandler(ref List<Message> messages)
        {
            Messages = messages;
            Counter = 0;
        }
        
        public Response HandleRequest(RequestContext req)
        {
            Response response = null;

            if (req == null)
            {
                Console.WriteLine("Object is not initialised.");
                response = new Response(400, "Bad Request");
                return response;
            }

            switch (req.Action)
            {
                case Action.List:
                    response = HandleList(req);
                    break;
                case Action.Add:
                    response = HandleAdd(req);
                    break;
                case Action.Read:
                    response = HandleRead(req);
                    break;
                case Action.Update:
                    response = HandleUpdate(req);
                    break;
                case Action.Delete:
                    response = HandleDelete(req);
                    break;
                default:
                    Console.WriteLine("Request in not valid");
                    response = new Response(400, "Bad Request");
                    return response;
            }
          
            return response;

        }

        private Response HandleList(RequestContext req)
        {
            if (Messages.Count == 0)
            {
                return new Response(200, "OK", "No messages have been sent yet.");
            }

            StringBuilder data = new StringBuilder();

            Console.WriteLine("\nHandle List...\n");
            foreach (var message in Messages)
            {
                data.Append("ID ");
                data.Append(message.ID);
                data.Append(":\n");
                data.Append(message.Content);
                data.Append("\n\n");
                //Console.WriteLine(message.ID + ": " + message.Content);
            }

            return new Response(200, "OK", data.ToString());
        }

        private Response HandleAdd(RequestContext req)
        {
            Console.WriteLine("\nHandle Add...\n");
            Counter++;
            Message inputMessage = new Message(Counter, req.Payload);
            Messages.Add(inputMessage);
            Console.WriteLine("New message added.\n");

            return new Response(201, "Created", "ID: " + Counter.ToString());
        }

        private Response HandleRead(RequestContext req)
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
                    return new Response(200, "OK", message.Content);
                }
            }

            if (!msgFound)
            {
                Console.WriteLine("Message not found! Reading not possible!\n");
                return new Response(404, "Not Found");
            }

            return null;

        }

        private Response HandleUpdate(RequestContext req)
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
                    return new Response(200, "OK");
                }
            }

            if (!msgFound)
            {
                Console.WriteLine("Message not found! Updating not possible!\n");
                return new Response(404, "Not Found");
            }

            return null;

        }

        private Response HandleDelete(RequestContext req)
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
                    return new Response(200, "OK");
                }           
            }

            if (!msgFound)
            {
                Console.WriteLine("Message not found! Deleting not possible!\n");
                return new Response(404, "Not Found");
            }

            return null;

        }

        private int GetMsgIDFromPath(string path)
        {
            string msgName = System.IO.Path.GetFileName(path);
            int msgID = int.Parse(msgName); // is possible because regex has already validated path and message number
            return msgID;
        }


    }


   
}
