using System;
using System.Collections.Generic;
using System.Linq;
using Poker;

namespace PokerServ
{
    public interface IHandEvaluator
    {
        BestHand GetBestHand(IEnumerable<Card> cards);
    }

    public class BestHand : IComparable<BestHand>
    {
        internal BestHand(HandRankType rankType, ICollection<CardType> cards)
        {
            if (cards.Count != 5)
            {
                throw new ArgumentException("Cards collection should contains exactly 5 elements", nameof(cards));
            }

            this.Cards = cards;
            this.RankType = rankType;
        }

        public ICollection<CardType> Cards { get; }

        public HandRankType RankType { get; }

        public int CompareTo(BestHand other)
        {
            if (this.RankType > other.RankType)
            {
                return 1;
            }

            if (this.RankType < other.RankType)
            {
                return -1;
            }

            switch (this.RankType)
            {
                case HandRankType.HighCard:
                    return CompareTwoHandsWithHighCard(this.Cards, other.Cards);
                case HandRankType.Pair:
                    return CompareTwoHandsWithPair(this.Cards, other.Cards);
                case HandRankType.TwoPairs:
                    return CompareTwoHandsWithTwoPairs(this.Cards, other.Cards);
                case HandRankType.ThreeOfAKind:
                    return CompareTwoHandsWithThreeOfAKind(this.Cards, other.Cards);
                case HandRankType.Straight:
                    return CompareTwoHandsWithStraight(this.Cards, other.Cards);
                case HandRankType.Flush:
                    return CompareTwoHandsWithHighCard(this.Cards, other.Cards);
                case HandRankType.FullHouse:
                    return CompareTwoHandsWithFullHouse(this.Cards, other.Cards);
                case HandRankType.FourOfAKind:
                    return CompareTwoHandsWithFourOfAKind(this.Cards, other.Cards);
                case HandRankType.StraightFlush:
                    return CompareTwoHandsWithStraight(this.Cards, other.Cards);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static int CompareTwoHandsWithHighCard(
            ICollection<CardType> firstHand,
            ICollection<CardType> secondHand)
        {
            var firstSorted = firstHand.OrderByDescending(x => x).ToList();
            var secondSorted = secondHand.OrderByDescending(x => x).ToList();
            var cardsToCompare = Math.Min(firstHand.Count, secondHand.Count);
            for (var i = 0; i < cardsToCompare; i++)
            {
                if (firstSorted[i] > secondSorted[i])
                {
                    return 1;
                }

                if (firstSorted[i] < secondSorted[i])
                {
                    return -1;
                }
            }

            return 0;
        }

        private static int CompareTwoHandsWithPair(
            ICollection<CardType> firstHand,
            ICollection<CardType> secondHand)
        {
            var firstPairType = firstHand.GroupBy(x => x).First(x => x.Count() >= 2);
            var secondPairType = secondHand.GroupBy(x => x).First(x => x.Count() >= 2);

            if (firstPairType.Key > secondPairType.Key)
            {
                return 1;
            }

            if (firstPairType.Key < secondPairType.Key)
            {
                return -1;
            }

            return CompareTwoHandsWithHighCard(firstHand, secondHand);
        }

        private static int CompareTwoHandsWithTwoPairs(
            ICollection<CardType> firstHand,
            ICollection<CardType> secondHand)
        {
            var firstPairType = firstHand.GroupBy(x => x).Where(x => x.Count() == 2).OrderByDescending(x => x.Key).ToList();
            var secondPairType = secondHand.GroupBy(x => x).Where(x => x.Count() == 2).OrderByDescending(x => x.Key).ToList();

            for (int i = 0; i < firstPairType.Count; i++)
            {
                if (firstPairType[i].Key > secondPairType[i].Key)
                {
                    return 1;
                }

                if (secondPairType[i].Key > firstPairType[i].Key)
                {
                    return -1;
                }
            }

            return CompareTwoHandsWithHighCard(firstHand, secondHand);
        }

        private static int CompareTwoHandsWithThreeOfAKind(
            ICollection<CardType> firstHand,
            ICollection<CardType> secondHand)
        {
            var firstThreeOfAKindType = firstHand.GroupBy(x => x).Where(x => x.Count() == 3).OrderByDescending(x => x.Key).FirstOrDefault();
            var secondThreeOfAKindType = secondHand.GroupBy(x => x).Where(x => x.Count() == 3).OrderByDescending(x => x.Key).FirstOrDefault();
            if (firstThreeOfAKindType.Key > secondThreeOfAKindType.Key)
            {
                return 1;
            }

            if (secondThreeOfAKindType.Key > firstThreeOfAKindType.Key)
            {
                return -1;
            }

            return CompareTwoHandsWithHighCard(firstHand, secondHand);
        }

        private static int CompareTwoHandsWithStraight(
            ICollection<CardType> firstHand,
            ICollection<CardType> secondHand)
        {
            var firstBiggestCardType = firstHand.Max();
            if (firstBiggestCardType == CardType.Ace && firstHand.Contains(CardType.Five))
            {
                firstBiggestCardType = CardType.Five;
            }

            var secondBiggestCardType = secondHand.Max();
            if (secondBiggestCardType == CardType.Ace && secondHand.Contains(CardType.Five))
            {
                secondBiggestCardType = CardType.Five;
            }

            return firstBiggestCardType.CompareTo(secondBiggestCardType);
        }

        private static int CompareTwoHandsWithFullHouse(
            ICollection<CardType> firstHand,
            ICollection<CardType> secondHand)
        {
            var firstThreeOfAKindType = firstHand.GroupBy(x => x).Where(x => x.Count() == 3).OrderByDescending(x => x.Key).FirstOrDefault();
            var secondThreeOfAKindType = secondHand.GroupBy(x => x).Where(x => x.Count() == 3).OrderByDescending(x => x.Key).FirstOrDefault();

            if (firstThreeOfAKindType.Key > secondThreeOfAKindType.Key)
            {
                return 1;
            }

            if (secondThreeOfAKindType.Key > firstThreeOfAKindType.Key)
            {
                return -1;
            }

            var firstPairType = firstHand.GroupBy(x => x).First(x => x.Count() == 2);
            var secondPairType = secondHand.GroupBy(x => x).First(x => x.Count() == 2);
            return firstPairType.Key.CompareTo(secondPairType.Key);
        }

        private static int CompareTwoHandsWithFourOfAKind(
            ICollection<CardType> firstHand,
            ICollection<CardType> secondHand)
        {
            var firstFourOfAKingType = firstHand.GroupBy(x => x).First(x => x.Count() == 4);
            var secondFourOfAKindType = secondHand.GroupBy(x => x).First(x => x.Count() == 4);

            if (firstFourOfAKingType.Key > secondFourOfAKindType.Key)
            {
                return 1;
            }

            if (firstFourOfAKingType.Key < secondFourOfAKindType.Key)
            {
                return -1;
            }

            return CompareTwoHandsWithHighCard(firstHand, secondHand);
        }
    }

    public class HandEvaluator : IHandEvaluator
    {
        private const int ComparableCards = 5;

        public BestHand GetBestHand(IEnumerable<Card> cards)
        {
            var cardSuitCounts = new int[(int)CardSuit.Spade + 1];
            var cardTypeCounts = new int[(int)CardType.Ace + 1];
            foreach (var card in cards)
            {
                cardSuitCounts[(int)card.Suit]++;
                cardTypeCounts[(int)card.Type]++;
            }

            if (cardSuitCounts.Any(x => x >= ComparableCards))
            {
                var straightFlushCards = this.GetStraightFlushCards(cardSuitCounts, cards);
                if (straightFlushCards.Count > 0)
                {
                    return new BestHand(HandRankType.StraightFlush, straightFlushCards);
                }

                for (var i = 0; i < cardSuitCounts.Length; i++)
                {
                    if (cardSuitCounts[i] >= ComparableCards)
                    {
                        var flushCards =
                            cards.Where(x => x.Suit == (CardSuit)i)
                                .Select(x => x.Type)
                                .OrderByDescending(x => x)
                                .Take(ComparableCards)
                                .ToList();
                        return new BestHand(HandRankType.Flush, flushCards);
                    }
                }
            }

            if (cardTypeCounts.Any(x => x == 4))
            {
                var bestFourOfAKind = this.GetTypesWithNCards(cardTypeCounts, 4)[0];
                var bestCards = new List<CardType>
                                    {
                                        bestFourOfAKind,
                                        bestFourOfAKind,
                                        bestFourOfAKind,
                                        bestFourOfAKind,
                                        cards.Where(x => x.Type != bestFourOfAKind).Max(x => x.Type)
                                    };

                return new BestHand(HandRankType.FourOfAKind, bestCards);
            }

            var pairTypes = this.GetTypesWithNCards(cardTypeCounts, 2);
            var threeOfAKindTypes = this.GetTypesWithNCards(cardTypeCounts, 3);
            if ((pairTypes.Count > 0 && threeOfAKindTypes.Count > 0) || threeOfAKindTypes.Count > 1)
            {
                var bestCards = new List<CardType>();
                for (var i = 0; i < 3; i++)
                {
                    bestCards.Add(threeOfAKindTypes[0]);
                }

                if (threeOfAKindTypes.Count > 1)
                {
                    for (var i = 0; i < 2; i++)
                    {
                        bestCards.Add(threeOfAKindTypes[1]);
                    }
                }

                if (pairTypes.Count > 0)
                {
                    for (var i = 0; i < 2; i++)
                    {
                        bestCards.Add(pairTypes[0]);
                    }
                }

                return new BestHand(HandRankType.FullHouse, bestCards);
            }

            var straightCards = this.GetStraightCards(cardTypeCounts);
            if (straightCards != null)
            {
                return new BestHand(HandRankType.Straight, straightCards);
            }

            if (threeOfAKindTypes.Count > 0)
            {
                var bestThreeOfAKindType = threeOfAKindTypes[0];
                var bestCards =
                    cards.Where(x => x.Type != bestThreeOfAKindType)
                        .Select(x => x.Type)
                        .OrderByDescending(x => x)
                        .Take(ComparableCards - 3).ToList();
                bestCards.AddRange(Enumerable.Repeat(bestThreeOfAKindType, 3));

                return new BestHand(HandRankType.ThreeOfAKind, bestCards);
            }

            if (pairTypes.Count >= 2)
            {
                var bestCards = new List<CardType>
                                    {
                                        pairTypes[0],
                                        pairTypes[0],
                                        pairTypes[1],
                                        pairTypes[1],
                                        cards.Where(x => x.Type != pairTypes[0] && x.Type != pairTypes[1])
                                            .Max(x => x.Type)
                                    };
                return new BestHand(HandRankType.TwoPairs, bestCards);
            }

            if (pairTypes.Count == 1)
            {
                var bestCards =
                    cards.Where(x => x.Type != pairTypes[0])
                        .Select(x => x.Type)
                        .OrderByDescending(x => x)
                        .Take(3).ToList();
                bestCards.Add(pairTypes[0]);
                bestCards.Add(pairTypes[0]);
                return new BestHand(HandRankType.Pair, bestCards);
            }
            else
            {

                var bestCards = new List<CardType>();
                for (var i = cardTypeCounts.Length - 1; i >= 0; i--)
                {
                    if (cardTypeCounts[i] > 0)
                    {
                        bestCards.Add((CardType)i);
                    }

                    if (bestCards.Count == ComparableCards)
                    {
                        break;
                    }
                }

                return new BestHand(HandRankType.HighCard, bestCards);
            }
        }

        private IList<CardType> GetTypesWithNCards(int[] cardTypeCounts, int n)
        {
            var pairs = new List<CardType>();
            for (var i = cardTypeCounts.Length - 1; i >= 0; i--)
            {
                if (cardTypeCounts[i] == n)
                {
                    pairs.Add((CardType)i);
                }
            }

            return pairs;
        }

        private ICollection<CardType> GetStraightFlushCards(int[] cardSuitCounts, IEnumerable<Card> cards)
        {
            var straightFlushCardTypes = new List<CardType>();
            for (var i = 0; i < cardSuitCounts.Length; i++)
            {
                if (cardSuitCounts[i] < ComparableCards)
                {
                    continue;
                }

                var cardTypeCounts = new int[(int)CardType.Ace + 1];
                foreach (var card in cards)
                {
                    if (card.Suit == (CardSuit)i)
                    {
                        cardTypeCounts[(int)card.Type]++;
                    }
                }

                var bestStraight = this.GetStraightCards(cardTypeCounts);
                if (bestStraight != null)
                {
                    straightFlushCardTypes.AddRange(bestStraight);
                }
            }

            return straightFlushCardTypes;
        }

        private ICollection<CardType> GetStraightCards(int[] cardTypeCounts)
        {
            var lastCardType = cardTypeCounts.Length;
            var straightLength = 0;
            for (var i = cardTypeCounts.Length - 1; i >= 1; i--)
            {
                var hasCardsOfType = cardTypeCounts[i] > 0 || (i == 1 && cardTypeCounts[(int)CardType.Ace] > 0);
                if (hasCardsOfType && i == lastCardType - 1)
                {
                    straightLength++;
                    if (straightLength == ComparableCards)
                    {
                        var bestStraight = new List<CardType>(ComparableCards);
                        for (var j = i; j <= i + ComparableCards - 1; j++)
                        {
                            if (j == 1)
                            {
                                bestStraight.Add(CardType.Ace);
                            }
                            else
                            {
                                bestStraight.Add((CardType)j);
                            }
                        }

                        return bestStraight;
                    }
                }
                else
                {
                    straightLength = 0;
                }

                lastCardType = i;
            }

            return null;
        }
    }

    public static class Logic
    {
        private static readonly IHandEvaluator HandEvaluator = new HandEvaluator();

        public static HandRankType GetHandRank(ICollection<Card> cards)
        {
            return HandEvaluator.GetBestHand(cards).RankType;
        }

        public static int CompareCards(List<Card> firstPlayerCards, List<Card> secondPlayerCards)
        {
            if (firstPlayerCards.Count() > 7)
            {
                throw new ArgumentNullException(nameof(firstPlayerCards));
            }
            if (secondPlayerCards.Count() > 7)
            {
                throw new ArgumentNullException(nameof(secondPlayerCards));
            }
            var firstPlayerBestHand = HandEvaluator.GetBestHand(firstPlayerCards);
            var secondPlayerBestHand = HandEvaluator.GetBestHand(secondPlayerCards);
            return firstPlayerBestHand.CompareTo(secondPlayerBestHand);
        }
    }
}
