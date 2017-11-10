using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Poker
{
    public enum CardSuit
    {
        Club = 0,
        Diamond = 1,
        Heart = 2,
        Spade = 3
    }

    public enum CardType
    {
        Two = 2,
        Three = 3,
        Four = 4,
        Five = 5,
        Six = 6,
        Seven = 7,
        Eight = 8,
        Nine = 9,
        Ten = 10,
        Jack = 11,
        Queen = 12,
        King = 13,
        Ace = 14,
    }

    public class Card
    {
        public CardSuit Suit { get; private set; }
        public CardType Type { get; private set; }

        public Card(CardSuit suit, CardType type)
        {
            this.Suit = suit;
            this.Type = type;
        }

        public Card(string card)
        {
            if (card == null || card.Length != 2)
                throw new ArgumentException();
            card = card.ToUpper();
            switch (card[1])
            {
                case 'C':
                    Suit = CardSuit.Club;
                    break;
                case 'S':
                    Suit = CardSuit.Spade;
                    break;
                case 'H':
                    Suit = CardSuit.Heart;
                    break;
                case 'D':
                    Suit = CardSuit.Diamond;
                    break;
                default:
                    throw new ArgumentException();
            }
            char uncheckedValue = card[0];
            if (!Enum.IsDefined(typeof(CardType), (CardType)uncheckedValue))
                throw new ArgumentException();
            Type = (CardType)uncheckedValue;
        }
        public string Encode()
        {
            string encodedCard = "" + (char)Type;
            switch (Suit)
            {
                case CardSuit.Club:
                    encodedCard += 'C';
                    break;
                case CardSuit.Spade:
                    encodedCard += 'S';
                    break;
                case CardSuit.Heart:
                    encodedCard += 'H';
                    break;
                case CardSuit.Diamond:
                    encodedCard += 'D';
                    break;
            }
            return encodedCard;
        }
        public override string ToString()
        {
            string tmp = "";
            switch (Type)
            {
                case CardType.Jack:
                    tmp += "Jack";
                    break;
                case CardType.Queen:
                    tmp += "Queen";
                    break;
                case CardType.King:
                    tmp += "King";
                    break;
                case CardType.Ace:
                    tmp += "Ace";
                    break;
                case CardType.Ten:
                    tmp += "T";
                    break;
                default:
                    tmp += (char)Type;
                    break;
            }
            tmp += " of " + System.Enum.GetName(typeof(CardSuit), Suit);
            return tmp;
        }

        public string Print()
        {
            return (Encode());
        }

        public string ToCard()
        {
            string tmp = "";
            switch (Type)
            {
                case CardType.Two:
                    tmp += "2";
                    break;
                case CardType.Three:
                    tmp += "3";
                    break;
                case CardType.Four:
                    tmp += "4";
                    break;
                case CardType.Five:
                    tmp += "5";
                    break;
                case CardType.Six:
                    tmp += "6";
                    break;
                case CardType.Seven:
                    tmp += "7";
                    break;
                case CardType.Eight:
                    tmp += "8";
                    break;
                case CardType.Nine:
                    tmp += "9";
                    break;
                case CardType.Ten:
                    tmp += "T";
                    break;
                case CardType.Jack:
                    tmp += "J";
                    break;
                case CardType.Queen:
                    tmp += "Q";
                    break;
                case CardType.King:
                    tmp += "K";
                    break;
                case CardType.Ace:
                    tmp += "A";
                    break;
                default:
                    throw new ArgumentException("cardType");
            }

            switch (this.Suit)
            {
                case CardSuit.Club:
                    tmp += "\u2663";
                    break;
                case CardSuit.Diamond:
                    tmp += "\u2666";
                    break;
                case CardSuit.Heart:
                    tmp += "\u2665";
                    break;
                case CardSuit.Spade:
                    tmp += "\u2660";
                    break;
                default:
                    throw new ArgumentException("cardSuit");
            }
            return tmp;
        }
    }
}