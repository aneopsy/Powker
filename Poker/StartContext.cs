using System;
using System.Collections.Generic;
using System.Net;
using ProtoBuf;

namespace Poker
{
    [ProtoContract]
    public class StartGameContext : IStartGameContext
    {
        public StartGameContext(List<string> playerNames, int startMoney)
        {
            this.PlayerNames = playerNames;
            this.StartMoney = startMoney;
        }
        public StartGameContext() { }
        [ProtoMember(1)]
        public List<String> PlayerNames { get; set; }
        [ProtoMember(2)]
        public int StartMoney { get; set; }
    }

    public class StartHandContext : IStartHandContext
    {
        public StartHandContext(Card firstCard, Card secondCard, int handNumber, int moneyLeft, int smallBlind, string firstPlayerName)
        {
            this.FirstCard = firstCard;
            this.SecondCard = secondCard;
            this.HandNumber = handNumber;
            this.MoneyLeft = moneyLeft;
            this.SmallBlind = smallBlind;
            this.FirstPlayerName = firstPlayerName;
        }

        public Card FirstCard { get; }

        public Card SecondCard { get; }

        public int HandNumber { get; }

        public int MoneyLeft { get; }

        public int SmallBlind { get; }

        public string FirstPlayerName { get; }
    }

    public class StartRoundContext : IStartRoundContext
    {
        public StartRoundContext(GameRoundType roundType, List<Card> boardCards, int moneyLeft, int currentPot)
        {
            this.RoundType = roundType;
            this.BoardCards = boardCards;
            this.MoneyLeft = moneyLeft;
            this.CurrentPot = currentPot;
        }

        public GameRoundType RoundType { get; }

        public List<Card> BoardCards { get; }

        public int MoneyLeft { get; }

        public int CurrentPot { get; }
    }

    [ProtoContract]
    public enum GameRoundType
    {
        [ProtoEnum]
        PreFlop = 0,
        [ProtoEnum]
        Flop = 1,
        [ProtoEnum]
        Turn = 2,
        [ProtoEnum]
        River = 3
    }

    public enum HandRankType
    {
        HighCard = 0,
        Pair = 1000,
        TwoPairs = 2000,
        ThreeOfAKind = 3000,
        Straight = 4000,
        Flush = 5000,
        FullHouse = 6000,
        FourOfAKind = 7000,
        StraightFlush = 8000
    }

}
