using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection.Metadata;
using System.Text;
using Castle.Core.Internal;
using HttpRestServer.DB_Connection;
using Newtonsoft.Json;
using Mtcg;
using Newtonsoft.Json.Linq;

namespace HttpRestServer
{
    public class EndpointHandler : IEndpointHandler
    {
        private List<Message> _messages;
        private int _counter;
        private Database _db;


        public EndpointHandler(ref List<Message> messages)
        {
            _messages = messages;
            _counter = 0;
            _db = new Database();
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


            //Authorization (For all requests except Login and Registration)
            if (!TryAuthorization(req))
            {
                return new Response(403, "Forbidden", "User hat keine Berechtigung, um diese Aktion auszuführen.");
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
                case Action.BuyPackage:
                    response = HandleBuyPackage(req);
                    break;
                case Action.ShowCards:
                    response = HandleShowCards(req);
                    break;
                case Action.ShowDeck:
                    response = HandleShowDeck(req);
                    break;
                case Action.ConfigureDeck:
                    response = HandleConfigureDeck(req);
                    break;
                case Action.ShowDeckInPlainText:
                    response = HandleShowDeckInPlainText(req);
                    break;
                //case Action.ShowProfile:
                //    break;
                //case Action.EditProfile:
                //    break;
                //case Action.ShowStats:
                //    break;
                //case Action.ShowScoreboard:
                //    break;
                //case Action.JoinBattle:
                //    break;
                //case Action.ShowDeals:
                //    break;
                //case Action.CreateDeal:
                //    break;
                //case Action.DeleteDeal:
                //    break;
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
            User currUser = JsonConvert.DeserializeObject<User>(req.Payload);

            if (currUser.Username.IsNullOrEmpty() || currUser.Password.IsNullOrEmpty())
            {
                return new Response(400, "Bad Request", "Registrierung nicht möglich, Username oder Passwort fehlt.");
            }

            if (_db.RegisterUser(currUser.Username, currUser.Password))
            {
                return new Response(201, "Created", "Registrierung erfolgreich.");
            }

            return new Response(400, "Bad Request", "Registrierung nicht möglich, Username existiert bereits.");

        }

        private Response HandleLogin(RequestContext req)
        {
            User currUser = JsonConvert.DeserializeObject<User>(req.Payload);

            if (currUser.Username.IsNullOrEmpty() || currUser.Password.IsNullOrEmpty())
            {
                return new Response(400, "Bad Request", "Login nicht möglich, Username oder Passwort fehlt.");
            }

            if (_db.Login(currUser.Username, currUser.Password))
            {
                return new Response(200, "OK", "Login erfolgreich.");
            }

            return new Response(400, "Bad Request", "Registrierung nicht möglich. Überprüfgen Sie nochmals ihre Eingaben.");

        }

        private Response HandleAddPackage(RequestContext req)
        {
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

            // save package in database
            if (_db.AddPackage(cards))
            {
                return new Response(201, "Created", "Package wurde erfolgreich hinzugefügt.");
            }

            return new Response(400, "Bad Request", "Package konnte nicht hinzugefügt werden.");

        }

        private Response HandleBuyPackage(RequestContext req)
        {
            var jsonData = JObject.Parse(req.Payload);

            if (!jsonData.ContainsKey("PackageID") || jsonData["PackageID"] == null)
            {
                return new Response(400, "Bad Request", "ID wurde nicht korrekt übermittelt oder ist ungültig.");
            }

            string username = GetUsernameFromAuthValue(req.Headers["Authorization"]);

            // buy package for the user (with username and packageid)
            Console.WriteLine("User möchte Package {0} kaufen.", jsonData["PackageID"]);
            if (_db.BuyPackage((string)(jsonData["PackageID"]), username))
            {
                return new Response(200, "OK", "Package wurde gekauft.");
            }

            return new Response(400, "Bad Request", "Package konnte nicht gekauft werden.");

        }

        private Response HandleShowCards(RequestContext req)
        {
            string username = GetUsernameFromAuthValue(req.Headers["Authorization"]);

            // get cards to display
            List<ICard> myCards = _db.GetAllCards(username);
            Console.WriteLine("{0} Karten im Besitz", myCards.Count);

            if (myCards.IsNullOrEmpty())
            {
                Console.WriteLine("Sie haben noch keine Karten gekauft.");
                return new Response(200, "OK", "Keine Karten verfügbar.");
            }

            myCards.ForEach(item => Console.Write("ID: {0}, Name: {1} Damage: {2} Element: {3}\n", item.Id, item.Name, item.Damage, item.ElementType));
            var json = JsonConvert.SerializeObject(myCards, new Newtonsoft.Json.Converters.StringEnumConverter());
            return new Response(200, "OK", json);

        }

        private Response HandleShowDeck(RequestContext req)
        {
            string username = GetUsernameFromAuthValue(req.Headers["Authorization"]);

            // get cards to display
            List<ICard> myCards = _db.GetDeck(username);
            Console.WriteLine("{0} Karten im Deck", myCards.Count);

            if (myCards.IsNullOrEmpty())
            {
                Console.WriteLine("Sie haben noch keine Karten im Deck.");
                return new Response(200, "OK", "Keine Karten verfügbar.");
            }

            myCards.ForEach(item => Console.Write("ID: {0}, Name: {1} Damage: {2} Element: {3}\n", item.Id, item.Name, item.Damage, item.ElementType));
            var json = JsonConvert.SerializeObject(myCards, new Newtonsoft.Json.Converters.StringEnumConverter());
            return new Response(200, "OK", json);

        }

        private Response HandleConfigureDeck(RequestContext req)
        {
            string username = GetUsernameFromAuthValue(req.Headers["Authorization"]);

            var cardsForDeck = JArray.Parse(req.Payload);

            if (cardsForDeck.Count != 4)
            {
                return new Response(400, "Bad Request", "Es müssen 4 Karten für das Deck angegeben werden.");
            }

            Console.WriteLine(cardsForDeck[0].GetType());

            // get current deck (if available) as backup
            List<ICard> backupDeck = _db.GetDeck(username);
            bool backupDeckAvailable = !backupDeck.IsNullOrEmpty();

            Console.WriteLine(backupDeckAvailable);

            _db.DeleteDeck(username);


            bool successful = true;
            foreach (var cardId in cardsForDeck)
            {
                if (!_db.AddCardToDeck((string)cardId, username))
                {
                    successful = false;
                    Console.WriteLine("Fehler aufgetreten. Hinzufügen abbrechen.");
                    break;
                }
            }

            if (!successful)
            {
                _db.DeleteDeck(username);
                if (backupDeckAvailable)
                {
                    Console.WriteLine("Altes Deck wird wieder hergestellt...");
                    foreach (var card in backupDeck)
                    {
                        if (!_db.AddCardToDeck(card.Id, username))
                        {
                            successful = false;
                            Console.WriteLine("Fehler aufgetreten. Hinzufügen abbrechen.");
                            break;
                        }
                    }
                }
                return new Response(400, "Bad Request", "Neues Deck konnte nicht konfiguriert werden.");
            }

            return new Response(200, "OK");

        }

        private Response HandleShowDeckInPlainText(RequestContext req)
        {
            string username = GetUsernameFromAuthValue(req.Headers["Authorization"]);

            // get cards to display
            List<ICard> myCards = _db.GetDeck(username);
            Console.WriteLine("{0} Karten im Deck", myCards.Count);

            if (myCards.IsNullOrEmpty())
            {
                Console.WriteLine("Sie haben noch keine Karten im Deck.");
                return new Response(200, "OK", "Keine Karten verfügbar.");
            }

            StringBuilder myCardTable = new StringBuilder();
            myCards.ForEach(item => myCardTable.Append($"Name: {item.Name, -20} Damage: {item.Damage, -10} Element: {item.ElementType, -10} ID: {item.Id} \n"));
            return new Response(200, "OK", myCardTable.ToString());

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


        private bool IsAdmin(Dictionary<string, string> headers)
        {
            if (!headers.ContainsKey("Authorization"))
            {
                return false;
            }

            string currUsername = GetUsernameFromAuthValue(headers["Authorization"]);

            if (!_db.ValidateToken(headers["Authorization"], currUsername) || !_db.CheckIfUserIsAdmin(currUsername))
            {
                return false;
            }

            Console.WriteLine("Authorization erfolgreich. Du bist ein Admin. Hallo {0}!", currUsername);
            return true;
        }

        private bool IsLoggedIn(Dictionary<string, string> headers)
        {
            bool success = false;
            if (!headers.ContainsKey("Authorization"))
            {
                return false;
            }

            string currUsername = GetUsernameFromAuthValue(headers["Authorization"]);

            if (_db.ValidateToken(headers["Authorization"], currUsername))
            {
                success = true;
                Console.WriteLine("Authorization erfolgreich. Hallo {0}!", currUsername);
            }

            return success;
        }

        private string GetUsernameFromAuthValue(string authorizationValue)
        {
            // structure is "Basic <username>-<Token>" here: z.B. "Basic kienboec-mtcgToken"
            string firstpart = "Basic ";
            int tokenLength = 10; //  "-" (1 char) + -Tokenlenght "mtcgToken" (9 char)
            int lengthUsername = authorizationValue.Length - firstpart.Length - tokenLength;
            string username = authorizationValue.Substring(firstpart.Length, lengthUsername);

            return username;
        }

        private bool TryAuthorization(RequestContext req)
        {
            // this must be ensured for every user
            if (!(req.Action == Action.Registration || req.Action == Action.Login))
            {
                if (!IsLoggedIn(req.Headers))
                {
                    return false;
                }
            }

            if (req.Action == Action.AddPackage)
            {
                if (!IsAdmin(req.Headers))
                {
                    return false;
                }
            }

            return true;
        }


    }


   
}
