using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Text;
using Castle.Core.Internal;
using Newtonsoft.Json;
using Mtcg;

namespace HttpRestServer
{
    public class EndpointHandler : IEndpointHandler
    {
        private List<Message> _messages;
        private int _counter;


        public EndpointHandler(ref List<Message> messages)
        {
            _messages = messages;
            _counter = 0;
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

                case Action.Registration:
                    response = HandleRegistration(req);
                    break;
                case Action.Login:
                    response = HandleLogin(req);
                    break;
                case Action.AddPackage:
                    response = HandleAddPackage(req);
                    break;
                default:
                    Console.WriteLine("Request in not valid");
                    response = new Response(400, "Bad Request");
                    return response;


                    /*
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
                    */
            }
          
            return response;

        }

        private Response HandleRegistration(RequestContext req)
        {
            Console.WriteLine("I handle Registration");

            Console.WriteLine("das sind die daten: ");

            User myUser = JsonConvert.DeserializeObject<User>(req.Payload);

            if (myUser.Username.IsNullOrEmpty() || myUser.Password.IsNullOrEmpty())
            {
                return new Response(400, "Bad Request", "Registrierung nicht möglich, Username oder Passwort fehlt.");
            }

            Console.WriteLine(myUser.Username);
            Console.WriteLine(myUser.Password);

            // ask in database, if user already exists
            // <DATABASE ACTION: Schauen ob username in Datenbank bereits existiert.>
            // select * from user where username = myUser.Username;
            bool userAlreadyExists = false;

            if (userAlreadyExists)
            {
                return new Response(400, "Bad Request", "Registrierung nicht möglich, Username existiert bereits.");
            }
            else
            {
                // <DATABASE ACTION: User in Datenbank anlegen. inkl. Token miterstellen: Basic <username>-mtcgToken z.B. Basic kienboeck-mtcgToken
                return new Response(201, "Created", "Registrierung erfolgreich.");
            }

        }

        private Response HandleLogin(RequestContext req)
        {
            Console.WriteLine("I handle Login");

            Console.WriteLine("das sind die daten: ");

            User myUser = JsonConvert.DeserializeObject<User>(req.Payload);

            if (myUser.Username.IsNullOrEmpty() || myUser.Password.IsNullOrEmpty())
            {
                return new Response(400, "Bad Request", "Login nicht möglich, Username oder Passwort fehlt.");
            }

            Console.WriteLine(myUser.Username);
            Console.WriteLine(myUser.Password);

            // ask in database, if username and password combination is correct
            // <DATABASE ACTION: Schauen ob username und password Kombination übereinstimmt>
            bool validLoginData = false;

            if (validLoginData)
            {
                return new Response(200, "OK", "Login erfolgreich.");
            }
            else
            {
                return new Response(400, "Bad Request", "Registrierung nicht möglich, Username und Passwort stimmen nicht überein.");
            }

        }

        private Response HandleAddPackage(RequestContext req)
        {
            Console.WriteLine("I handle Add Package");

            // security check: admin token here?
            if (!IsAdmin(req.Headers))
            {
                return new Response(403, "Forbidden", "User hat keine Berechtigung, um diese Aktion auszuführen.");
            }

            //List<ICard> myPackage = JsonConvert.DeserializeObject<List<ICard>>(req.Payload);

            // List<string> myPackage = JsonConvert.DeserializeObject<List<string>>(req.Payload);
            //List<string> cards = new List<string>();

            var inputSyntax = new[] { new { Id = "", Name = "", Damage = "" } };
            var cards = JsonConvert.DeserializeAnonymousType(req.Payload, inputSyntax);

            if (cards.Length != 5)
            {
                return new Response(400, "Bad Request", "Kartenanzahl für neues Package nicht korrekt. Es müssen genau 5 Karten angegeben werden.");
            }

            // check if IDs are valid
            foreach (var card in cards)
            {
                // validation of input
                if (card.Id.Length != 36)
                {
                    return new Response(400, "Bad Request", "Package konnte nicht hinzugefügt werden. Bitte überprüfen Sie die Länge der ID.");
                }

            }

            // extract elementType and cardType and save package in database
            foreach (var card in cards)
            {
                string cardType = ExtractCardType(card.Name); // spell or monster
                var elementType = ExtractElementType(card.Name); // fire, water or normal
                Console.WriteLine(card.Name);
                Console.WriteLine(card.Damage);
                Console.WriteLine("Element of Card is: {0}", elementType);
                Console.WriteLine("Type of Card is: {0}", cardType);
                Console.WriteLine();

                // <DATABASE ACTION: Karten in Tabelle Cards einfügen> (values: id, name, damage, elementtype, cardtype)
                // <DATABASE ACTION: Karten als Package in Tabelle Package einfügen>
            }

            return new Response(201, "Created", "Package wurde erfolgreich hinzugefügt.");

        }

        private Response HandleList(RequestContext req)
        {
            if (_messages.Count == 0)
            {
                return new Response(200, "OK", "No messages have been sent yet.");
            }

            StringBuilder data = new StringBuilder();

            Console.WriteLine("\nHandle List...\n");
            foreach (var message in _messages)
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
            _counter++;
            Message inputMessage = new Message(_counter, req.Payload);
            _messages.Add(inputMessage);
            Console.WriteLine("New message added.\n");

            return new Response(201, "Created", "ID: " + _counter.ToString());
        }

        private Response HandleRead(RequestContext req)
        {
            Console.WriteLine("\nHandle Read...\n");
            bool msgFound = false;
            int msgID = GetMsgIDFromPath(req.ResourcePath); 

            // search in messages if message id exists
            foreach (var message in _messages)
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
            foreach (var message in _messages)
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
            for (int i = 0; i < _messages.Count; i++)
            {
                if (_messages[i].ID == msgID)
                {
                    //Console.WriteLine("TO DELETE: " + Messages[i].Content);
                    msgFound = true;
                    _messages.Remove(_messages[i]);
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

        private string ExtractElementType(string cardName)
        {
            var arr = Enum.GetValues(typeof(Element));

            for (var i = 0; i < arr.Length; i++)
            {
                var elementType = arr.GetValue(i)?.ToString();
                if (elementType != null && cardName.ToLower().Contains(elementType.ToLower()))
                {
                    return elementType;
                }
            }

            // if there is no specific element type given, the card will be defined as first element of enum-array (here: normal)
            return arr.GetValue(0)?.ToString();
        }

        private string ExtractCardType(string cardName)
        {
            var arr = Enum.GetValues(typeof(CardType));

            for (var i = 0; i < arr.Length; i++)
            {
                var cardType = arr.GetValue(i)?.ToString();
                if (cardType != null && cardName.ToLower().Contains(cardType.ToLower()))
                {
                    return cardType;
                }
            }

            // if there is no specific monster type given, the card will be defined as first element of enum-array (here: spell)
            return arr.GetValue(0)?.ToString();

        }

        private bool IsAdmin(Dictionary<string, string> headers)
        {
            return headers.ContainsKey("Authorization") && headers.ContainsValue("Basic admin-mtcgToken");
        }



    }


   
}
