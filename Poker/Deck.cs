using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Poker
{
    public static class RandGen
    {
        private static readonly ThreadLocal<Random> Random =
            new ThreadLocal<Random>(() => new Random(Interlocked.Increment(ref seed)));

        private static int seed = Environment.TickCount;

        public static int Next(int minValue, int maxValue)
        {
            return Random.Value.Next(minValue, maxValue);
        }
    }
    public static class EnumerableExtensions
    {
        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            var array = source.ToArray();
            var x = array.Length;
            for (var i = 0; i < x; i++)
            {
                var z = i + RandGen.Next(0, x - i);
                var temp = array[i];
                array[i] = array[z];
                array[z] = temp;
            }

            return array;
        }

        public static string CardsToString(this IEnumerable<Card> cards)
        {
            if (cards == null)
            {
                return string.Empty;
            }

            var stringBuilder = new StringBuilder();
            foreach (var card in cards)
            {
                stringBuilder.Append(card).Append(" ");
            }

            return stringBuilder.ToString().Trim();
        }
    }

    static class ListExtension
    {
        public static T PopAt<T>(this List<T> list, int index)
        {
            T r = list[index];
            list.RemoveAt(index);
            return r;
        }
    }

    public class Deck : IDeck
    {
        public static readonly IReadOnlyList<Card> all_cards;
        private static readonly IEnumerable<CardType> all_card_types = new List<CardType>
                                                                         {
                                                                             CardType.Two,
                                                                             CardType.Three,
                                                                             CardType.Four,
                                                                             CardType.Five,
                                                                             CardType.Six,
                                                                             CardType.Seven,
                                                                             CardType.Eight,
                                                                             CardType.Nine,
                                                                             CardType.Ten,
                                                                             CardType.Jack,
                                                                             CardType.Queen,
                                                                             CardType.King,
                                                                             CardType.Ace
                                                                         };
        private static readonly IEnumerable<CardSuit> all_card_suits = new List<CardSuit>
                                                                         {
                                                                             CardSuit.Club,
                                                                             CardSuit.Diamond,
                                                                             CardSuit.Heart,
                                                                             CardSuit.Spade
                                                                         };
        private readonly IList<Card> cards;
        private int card_index;

        static Deck()
        {
            var cards = new List<Card>();
            foreach (var cardSuit in all_card_suits)
            {
                foreach (var cardType in all_card_types)
                {
                    cards.Add(new Card(cardSuit, cardType));
                }
            }

            all_cards = cards.AsReadOnly();
        }

        public Deck()
        {
            this.cards = all_cards.Shuffle().ToList();
            this.card_index = all_cards.Count;
        }

        public override string ToString()
        {
            string tmp = "";
            foreach (Card card in cards)
            {
                tmp += card.ToString();
                tmp += "\n";
            }
            tmp = tmp.Remove(tmp.Length - 1);
            return tmp;
        }

        public string ToCard()
        {
            string tmp = "";
            foreach (Card card in cards)
            {
                tmp += card.ToCard();
                tmp += "\n";
            }
            tmp = tmp.Remove(tmp.Length - 1);
            return tmp;
        }

        public int Length()
        {
            return (this.card_index);
        }

        public Card GetNextCard()
        {
            if (this.card_index == 0)
                throw new ArgumentException();

            --this.card_index;
            var card = this.cards[this.card_index];
            return card;
        }
    }
}
