using System;
using System.Collections.Generic;
using System.Text;

namespace HttpRestServer
{
    public enum Action
    {
        List,
        Add,
        Read,
        Update,
        Delete,
        Undefined,
        Registration,
        Login,
        AddPackage,
        BuyPackage,
        ShowCards,
        ShowDeck,
        ConfigureDeck,
        ShowDeckInPlainText,
        ShowProfile,
        EditProfile,
        ShowStats,
        ShowScoreboard,
        JoinBattle,
        ShowDeals,
        CreateDeal,
        DeleteDeal
    }
}
