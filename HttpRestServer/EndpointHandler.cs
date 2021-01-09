using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using Castle.Core.Internal;
using HttpRestServer.DB_Connection;
using Newtonsoft.Json;
using Mtcg;
using Mtcg.Cards;
using Newtonsoft.Json.Linq;

namespace HttpRestServer
{
    public class EndpointHandler : IEndpointHandler
    {
        private ConcurrentBag<Battle> _allBattles;
        private Database _db;


        public EndpointHandler(ref ConcurrentBag<Battle> allBattles)
        {
            _allBattles = allBattles;
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
                return new Response(403, "Forbidden", "User hat keine Berechtigung, um diese Aktion auszufuehren.", false);
            }

            if (!IsPayloadValid(req))
            {
                return new Response(400, "Bad Request", "Inhalt fehlt.", false);
            }

            string username = "";
            if (req.Headers.ContainsKey("Authorization"))
            {
                username = GetUsernameFromAuthValue(req.Headers["Authorization"]);
            }

            try
            {
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
                        response = HandleShowDeck(username);
                        break;
                    case Action.ConfigureDeck:
                        response = HandleConfigureDeck(req, username);
                        break;
                    case Action.ShowDeckInPlainText:
                        response = HandleShowDeckInPlainText(req, username);
                        break;
                    case Action.ShowProfile:
                        response = HandleShowProfile(req, username);
                        break;
                    case Action.EditProfile:
                        response = HandleEditProfile(req, username);
                        break;
                    case Action.ShowStats:
                        response = HandleShowStats(username);
                        break;
                    case Action.ShowScoreboard:
                        response = HandleShowScoreboard();
                        break;
                    case Action.JoinBattle:
                        response = HandleJoinBattle(username);
                        break;
                    case Action.ShowDeals:
                        response = HandleShowDeals();
                        break;
                    case Action.CreateDeal:
                        response = HandleCreateDeal(req, username);
                        break;
                    case Action.DeleteDeal:
                        response = HandleDeleteDeal(req, username);
                        break;
                    case Action.DoTrading:
                        response = HandleDoTrading(req, username);
                        break;
                    default:
                        Console.WriteLine("Request in not valid");
                        response = new Response(400, "Bad Request");
                        return response;
                }
            }
            catch (NullReferenceException ex)
            {
                Console.WriteLine("An object couldn't be referenced: " + ex.Message);
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine("There was a problem: " + ex.Message);
                throw;
            }

            
          
            return response;

        }

        private Response HandleRegistration(RequestContext req)
        {
            User currUser = JsonConvert.DeserializeObject<User>(req.Payload);

            if (currUser.Username.IsNullOrEmpty() || currUser.Password.IsNullOrEmpty())
            {
                return new Response(400, "Bad Request", "Registrierung nicht moeglich, Username oder Passwort fehlt.", false);
            }

            if (_db.RegisterUser(currUser.Username, currUser.Password))
            {
                return new Response(201, "Created", "Registrierung erfolgreich.", false);
            }

            return new Response(400, "Bad Request", "Registrierung nicht moeglich, Username existiert bereits.", false);

        }

        private Response HandleLogin(RequestContext req)
        {
            User currUser = JsonConvert.DeserializeObject<User>(req.Payload);

            if (currUser.Username.IsNullOrEmpty() || currUser.Password.IsNullOrEmpty())
            {
                return new Response(400, "Bad Request", "Login nicht moeglich, Username oder Passwort fehlt.", false);
            }

            if (_db.Login(currUser.Username, currUser.Password))
            {
                return new Response(200, "OK", "Login erfolgreich.", false);
            }

            return new Response(400, "Bad Request", "Registrierung nicht moeglich. Ueberpruefen Sie nochmals ihre Eingaben.", false);

        }

        private Response HandleAddPackage(RequestContext req)
        {
            Console.WriteLine(req.Payload);
            var newPackage = JsonConvert.DeserializeObject<Package>(req.Payload);

            if (newPackage.Cards.Count != 5)
            {
                return new Response(400, "Bad Request", "Kartenanzahl fuer neues Package nicht korrekt. Es muessen genau 5 Karten angegeben werden.", false);
            }

            // check if IDs are valid
            foreach (var card in newPackage.Cards)
            {
                // validation of input
                if (card.Id.Length != 36)
                {
                    return new Response(400, "Bad Request", "Package konnte nicht hinzugefuegt werden. Bitte ueberpruefen Sie die Laenge der ID.", false);
                }

            }

            // save package in database
            if (_db.AddPackage(newPackage.PackageID, newPackage.Cards))
            {
                return new Response(201, "Created", "Package wurde erfolgreich hinzugefuegt.", false);
            }

            return new Response(400, "Bad Request", "Package konnte nicht hinzugefuegt werden.", false);

        }

        private Response HandleBuyPackage(RequestContext req)
        {
            var jsonData = JObject.Parse(req.Payload);

            if (!jsonData.ContainsKey("PackageID") || jsonData["PackageID"] == null)
            {
                return new Response(400, "Bad Request", "ID wurde nicht korrekt uebermittelt oder ist ungueltig.", false);
            }

            string username = GetUsernameFromAuthValue(req.Headers["Authorization"]);

            // buy package for the user (with username and packageid)
            Console.WriteLine("User moechte Package {0} kaufen.", jsonData["PackageID"]);
            if (_db.BuyPackage((string)(jsonData["PackageID"]), username))
            {
                return new Response(200, "OK", "Package wurde gekauft.", false);
            }

            return new Response(400, "Bad Request", "Package konnte nicht gekauft werden.", false);

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
                return new Response(200, "OK", "Keine Karten verfuegbar.", false);
            }

            myCards.ForEach(item => Console.Write("ID: {0}, Name: {1} Damage: {2} Element: {3}\n", item.Id, item.Name, item.Damage, item.ElementType));
            var json = JsonConvert.SerializeObject(myCards, new Newtonsoft.Json.Converters.StringEnumConverter());
            return new Response(200, "OK", json, true);

        }

        private Response HandleShowDeck(string username)
        {
            // get cards to display
            List<ICard> myCards = _db.GetDeck(username);
            Console.WriteLine("{0} Karten im Deck", myCards.Count);

            if (myCards.IsNullOrEmpty())
            {
                Console.WriteLine("Sie haben noch keine Karten im Deck.");
                return new Response(200, "OK", "Keine Karten verfuegbar.", false);
            }

            //myCards.ForEach(item => Console.Write("ID: {0}, Name: {1} Damage: {2} Element: {3}\n", item.Id, item.Name, item.Damage, item.ElementType));
            var json = JsonConvert.SerializeObject(myCards, new Newtonsoft.Json.Converters.StringEnumConverter());
            return new Response(200, "OK", json, true);

        }

        private Response HandleConfigureDeck(RequestContext req, string username)
        {
            var cardsForDeck = JArray.Parse(req.Payload);

            if (cardsForDeck.Count != 4)
            {
                return new Response(400, "Bad Request", "Es muessen 4 Karten fuer das Deck angegeben werden.", false);
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
                    Console.WriteLine("Fehler aufgetreten. Hinzufuegen abbrechen.");
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
                            Console.WriteLine("Fehler aufgetreten. Hinzufuegen abbrechen.");
                            break;
                        }
                    }
                }
                return new Response(400, "Bad Request", "Neues Deck konnte nicht konfiguriert werden.", false);
            }

            return new Response(200, "OK");

        }

        private Response HandleShowDeckInPlainText(RequestContext req, string username)
        {
            // get cards to display
            List<ICard> myCards = _db.GetDeck(username);
            Console.WriteLine("{0} Karten im Deck", myCards.Count);

            if (myCards.IsNullOrEmpty())
            {
                Console.WriteLine("Sie haben noch keine Karten im Deck.");
                return new Response(200, "OK", "Keine Karten verfuegbar.", false);
            }

            StringBuilder myCardTable = new StringBuilder();
            myCards.ForEach(item => myCardTable.Append($"Name: {item.Name, -20} Damage: {item.Damage, -10} Element: {item.ElementType, -10} ID: {item.Id} \n"));
            return new Response(200, "OK", myCardTable.ToString(), false);

        }

        private Response HandleShowProfile(RequestContext req, string username)
        {
            int i = req.ResourcePath.LastIndexOf('/');
            string targetUsername = req.ResourcePath.Substring(i + 1);

            if (username != targetUsername)
            {
                Console.WriteLine("Fremdes Profil darf nicht angesehen werden.");
                return new Response(403, "Forbidden", "Fremdes Profil darf nicht angesehen werden.", false);
            }

            Profile userProfile = _db.GetUserProfile(targetUsername);

            if (userProfile != null)
            {
                var json = JsonConvert.SerializeObject(userProfile);
                return new Response(200, "OK", json, true);
            }

            return new Response(400, "Bad Request", "Fehler.", false);

        }

        private Response HandleEditProfile(RequestContext req, string username)
        {
            int i = req.ResourcePath.LastIndexOf('/');
            string targetUsername = req.ResourcePath.Substring(i + 1);

            if (username != targetUsername)
            {
                Console.WriteLine("Fremdes Profil darf nicht bearbeitet werden.");
                return new Response(403, "Forbidden", "Fremdes Profil darf nicht bearbeitet werden.", false);
            }

            Console.WriteLine(req.Payload);

            Profile newProfile = JsonConvert.DeserializeObject<Profile>(req.Payload);

            if (newProfile.Name.IsNullOrEmpty() || newProfile.Bio.IsNullOrEmpty() || newProfile.Image.IsNullOrEmpty())
            {
                return new Response(400, "Bad Request", "Nicht alle Werte uebermittelt.", false);
            }

            if (_db.EditUserProfile(targetUsername, newProfile.Name, newProfile.Bio, newProfile.Image))
            {
                Profile userProfile = _db.GetUserProfile(targetUsername);

                if (userProfile != null)
                {
                    Console.WriteLine("Profil wurde aktualisiert.");
                    var json = JsonConvert.SerializeObject(userProfile);
                    return new Response(200, "OK", json, true);
                }
            }

            return new Response(400, "Bad Request", "Bearbeiten nicht moeglich.", false);

        }
        
        private Response HandleJoinBattle(string username)
        {
            // show all existing battles
            Console.WriteLine("Current Battles: " + _allBattles.Count);
            foreach (var test in _allBattles)
            {
                Console.Write(String.Format("Battle: {0}{1}", test.Player1.Username,
                    test.Player2 == null ? "" : String.Format(" gegen {0}\n", test.Player2.Username)));
            }

            // Get user data for battle
            User myUser = new User();
            myUser.Username = username;
            myUser.Deck = _db.GetDeck(username);
            myUser.Stats = _db.GetStats(username);

            // make sure that all necessary objects are here
            if (myUser.Deck.IsNullOrEmpty() || myUser.Deck.Count < 4)
            {
                return new Response(200, "OK", "Es wurde noch kein Deck konfiguriert.", false);
            }


            myUser.Deck.ForEach(card => Console.Write("ID: {0}, Name: {1} Damage: {2} Element: {3}\n", card.Id, card.ElementType, card.Damage, card.GetType()));

            //check if user can join existing game
            bool addingSuccessful = false;
            foreach (var currBattle in _allBattles)
            {
                if (currBattle.AddUserToBattle(myUser))
                {
                    addingSuccessful = true;
                    Console.WriteLine("Adding successful: " + addingSuccessful);

                    while (!currBattle.HasStarted())
                    {
                        Console.WriteLine("Warten auf Spielstart...");
                    }

                    Console.WriteLine("Spiel wurde gestartet...");

                    while (currBattle.IsRunning())
                    {
                        //Console.WriteLine("Game is running...");
                    }

                    Console.WriteLine("Spiel beendet.");
                    var json = JsonConvert.SerializeObject(currBattle.Gamelog);
                    return new Response(200, "OK", json, true);
                }
            }

            if (!addingSuccessful)
            {
                Battle battleThatPlayerHosts = new Battle { Player1 = myUser };
                _allBattles.Add(battleThatPlayerHosts);

                while (battleThatPlayerHosts.UserCount != 2)
                {
                    //Console.WriteLine("Noch kein Zweiter Spieler hier!");
                }

                Console.WriteLine("Zweiter Spieler ist hier!");
                battleThatPlayerHosts.StartGame();

                while (battleThatPlayerHosts.IsRunning())
                {
                    Console.WriteLine("Spiel laeuft...");
                }

                Console.WriteLine("Spiel endet...");


                // show Log
                //battleThatPlayerHosts.Gamelog.Show();

                // look at results
                if (battleThatPlayerHosts.Winner == null)
                {
                    // Draw - nothing happens
                    
                } else if (battleThatPlayerHosts.Winner.Username == battleThatPlayerHosts.Player1.Username)
                {
                    _db.UpdateStatsAfterWin(battleThatPlayerHosts.Player1.Username);
                    _db.UpdateStatsAfterLoss(battleThatPlayerHosts.Player2.Username);
                }
                else if (battleThatPlayerHosts.Winner.Username == battleThatPlayerHosts.Player2.Username)
                {
                    _db.UpdateStatsAfterWin(battleThatPlayerHosts.Player2.Username);
                    _db.UpdateStatsAfterLoss(battleThatPlayerHosts.Player1.Username);
                }

                var json = JsonConvert.SerializeObject(battleThatPlayerHosts.Gamelog);
                return new Response(200, "OK", json, true);

            }

            return new Response(400, "Bad Request", "Fehler.", false);

        }

        private Response HandleDoTrading(RequestContext req, string username)
        {
            // get trade id via substring, must be possible because length is already validated
            string tradeId = req.ResourcePath.Substring(10, 36);
            var offeredCardId = (string)JToken.Parse(req.Payload);
            
            // get details of own card that ist offered
            ICard offeredCard = _db.GetCard(offeredCardId, username);
            Trade requestedTrade = _db.GetTrade(tradeId);

            if ((requestedTrade != null) && (offeredCard != null))
            {
                // print data of two cards
                string offeredCardType = "";
                if (offeredCard is Monster)
                {
                    offeredCardType = "Monster";
                } else if (offeredCard is Spell)
                {
                    offeredCardType = "Spell";
                }

                Console.WriteLine("Karte aus Tradedeal: Typ: {0}, MinDamage: {1}", requestedTrade.Type, requestedTrade.MinimumDamage);
                Console.WriteLine("Angebotene Karte: Typ: {0}, Damage: {1}", offeredCardType, offeredCard.Damage);
                // make sure that he doesn't try to trade with himself
                if (requestedTrade.Username == username)
                {
                    return new Response(400, "Bad Request", "Mit sich selbst kann nicht getraded werden.", false);
                }


                // compare if offeredCard meets requirements from tradecard
                if (requestedTrade.MinimumDamage < offeredCard.Damage)
                {
                    if (requestedTrade.Type.ToLower() == offeredCardType.ToLower())
                    {
                        Console.WriteLine("Trade ist moeglich.");
                        if (_db.ExecuteDeal(tradeId, requestedTrade.Username, requestedTrade.CardToTrade, username, offeredCardId))
                        {
                            Console.WriteLine("Trade wurde durchgefuehrt.");
                            return new Response(200, "OK", "Trade wurde durchgefuehrt. Karten wurden ausgetauscht.", false);
                        }
                    }
                    else
                    {
                        Console.WriteLine("Trade ist nicht moeglich. Ueberpruefe, ob erforderter Typ und Damage-Wert erfuellt ist.");
                    }
                }

            }

            return new Response(400, "Bad Request", "Trade konnte nicht durchgefuehrt werden.", false);

        }

        private Response HandleCreateDeal(RequestContext req, string username)
        {
            Trade newTrade = JsonConvert.DeserializeObject<Trade>(req.Payload);

            if (newTrade.Id.IsNullOrEmpty() || newTrade.CardToTrade.IsNullOrEmpty() || newTrade.Type.IsNullOrEmpty() || (newTrade.MinimumDamage < 0))
            {
                return new Response(400, "Bad Request", "Nicht alle Werte uebermittelt.", false);
            }

            if (_db.AddTrade(username, newTrade.Id, newTrade.CardToTrade, newTrade.Type, newTrade.MinimumDamage))
            {
    
                return new Response(200, "OK", "Trade wurde erstellt.", false);
                
            }

            return new Response(400, "Bad Request", "Fehler.", false);

        }

        private Response HandleDeleteDeal(RequestContext req, string username)
        {
            // get trade id via substring, must be possible because length is already validated
            string tradeId = req.ResourcePath.Substring(10, 36);

            if (_db.DeleteDeal(username, tradeId))
            {
                return new Response(200, "OK", "Deal wurde entfernt.", false);
            }

            return new Response(400, "Bad Request", "Fehler.", false);

        }

        private Response HandleShowDeals()
        {
            // get deals to display
            List<Trade> allTrades = _db.GetAllTrades();
            Console.WriteLine("{0} Trades verfuegbar.", allTrades.Count);

            if (!allTrades.IsNullOrEmpty())
            {
                var json = JsonConvert.SerializeObject(allTrades, new Newtonsoft.Json.Converters.StringEnumConverter());
                return new Response(200, "OK", json, true);
            }

            return new Response(400, "Bad Request", "Keine Trades verfuegbar.", false);

        }


        private Response HandleShowStats(string username)
        {
            // get stats to display
            Stats userStats = _db.GetStats(username);

            if (userStats != null)
            {
                var json = JsonConvert.SerializeObject(userStats);
                return new Response(200, "OK", json, true);
            }

            return new Response(400, "Bad Request", "Fehler.", false);
            
        }

        private Response HandleShowScoreboard()
        {
            // get scoreboard to display
            List<Stats> scoreboard = _db.GetScoreboard();
            Console.WriteLine("{0} User spielen MTCG.", scoreboard.Count);

            if (!scoreboard.IsNullOrEmpty())
            {
                var json = JsonConvert.SerializeObject(scoreboard, new Newtonsoft.Json.Converters.StringEnumConverter());
                return new Response(200, "OK", json, true);
            }

            return new Response(400, "Bad Request", "Keine Spieler verfuegbar.", false);
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
            if (!(req.Action == Action.Registration || req.Action == Action.Login || req.Action == Action.Undefined))
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
        private bool IsPayloadValid(RequestContext req)
        {
            // ensure that payload exists where necessary
            if (!(req.Action == Action.ShowCards || 
                req.Action == Action.ShowDeck ||
                req.Action == Action.ShowDeckInPlainText ||
                req.Action == Action.ShowProfile || 
                req.Action == Action.ShowStats ||
                req.Action == Action.ShowScoreboard ||
                req.Action == Action.ShowDeals ||
                req.Action == Action.JoinBattle) )
            {
                if (req.Payload.IsNullOrEmpty())
                {
                    return false;
                }
            }
            return true;
        }
        
        
    }


   
}
