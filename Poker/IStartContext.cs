using System;
using System.Collections.Generic;

namespace Poker
{
    public interface IStartGameContext
    {
        List<String> PlayerNames { get; }
        int StartMoney { get; }
    }

    public interface IStartHandContext
    {
        string FirstPlayerName { get; }
        Card FirstCard { get; }
        Card SecondCard { get; }
        int HandNumber { get; }
        int MoneyLeft { get; }
        int SmallBlind { get; }
    }

    public interface IStartRoundContext
    {
        List<Card> BoardCards { get; }
        int CurrentPot { get; }
        int MoneyLeft { get; }
        GameRoundType RoundType { get; }
    }
}
