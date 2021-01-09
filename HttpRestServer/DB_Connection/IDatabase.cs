using System;
using System.Collections.Generic;
using Mtcg;
using Mtcg.Cards;
using Npgsql;

namespace HttpRestServer
{
    public interface IDatabase
    {
        public bool RegisterUser(string username, string password);

        public bool Login(string username, string password);

        public bool AddPackage(string packageId, List<Card> cards);

        public bool ValidateToken(string token, string username);

        public bool BuyPackage(string packageID, string username);

        public bool CheckIfUserIsAdmin(string username);

        public List<ICard> GetAllCards(string username);

        public ICard GetCard(string cardId, string username);

        public List<ICard> GetDeck(string username);

        public bool AddCardToDeck(string card1, string username);

        public bool DeleteDeck(string username);

        public bool DeleteDeal(string username, string tradeId);

        public Stats GetStats(string username);

        public bool UpdateStatsAfterWin(string username);

        public bool UpdateStatsAfterLoss(string username);

        public List<Stats> GetScoreboard();

        public Profile GetUserProfile(string username);

        public Trade GetTrade(string tradeId);

        public bool EditUserProfile(string username, string newName, string newBio, string newImage);

        public bool AddTrade(string username, string tradeId, string cardToTradeId, string cardType, float minDamage);

        public bool ExecuteDeal(string tradeId, string usernameDealOwner, string cardIdDealOwner,
            string usernameTradeExecuter, string cardIdTradeExecuter);

        public List<Trade> GetAllTrades();

        public bool CreateUserProfile(string username);

    }
}