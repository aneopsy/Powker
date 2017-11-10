using System;
using System.Collections.Generic;

namespace Poker
{
    public class PlayerUI : APlayerUI
    {
        private const ConsoleColor PlayerBoxColor = ConsoleColor.DarkGreen;

        private readonly int row;

        private readonly int width;

        private readonly int commonRow;

        public PlayerUI(IPlayer player, int row, int width, int commonRow)
            : base(player)
        {
            this.row = row;
            this.width = width;
            this.commonRow = commonRow;
            ConsoleConfig.WriteOnConsole(6, 1, "Name: " + player.Name);
        }

        private IReadOnlyCollection<Card> BoardCards { get; set; }


        public override void StartHand(IStartHandContext context)
        {
            this.UpdateCommonRow(0);
            var dealerSymbol = context.FirstPlayerName == this.Player.Name ? "Dealer" : "      ";

            ConsoleConfig.WriteOnConsole(this.row + 1, 1, dealerSymbol, ConsoleColor.Green);

            ConsoleConfig.WriteOnConsole(this.row + 11, 2, "Money: " + context.MoneyLeft.ToString());
            this.DrawSingleCard(this.row + 9, width / 2 - 6, context.FirstCard);
            this.DrawSingleCard(this.row + 9, width / 2 - 1, context.SecondCard);

            DrawSingleHideCard(this.row + 4, width / 2 - 4);
            DrawSingleHideCard(this.row + 4, width / 2 - 9);
            DrawSingleHideCard(this.row + 4, width / 2 + 1);
            DrawSingleHideCard(this.row + 4, width / 2 - 14);
            DrawSingleHideCard(this.row + 4, width / 2 + 6);

            DrawSingleHideCard(this.row + 4, width / 2 + 27);
            DrawSingleHideCard(this.row + 4, width / 2 + 22);

            base.StartHand(context);
        }

        public override void EndHand(IEndHandContext context)
        {
            this.UpdateCommonRow(0);
            if (context.ShowdownCards != null)
            {
                context.ShowdownCards.Remove(this.Player.Name);
                foreach (KeyValuePair<string, List<Card>> pair in context.ShowdownCards)
                {

                    this.DrawSingleCard(this.row + 4, width / 2 + 27, pair.Value[1]);
                    this.DrawSingleCard(this.row + 4, width / 2 + 22, pair.Value[0]);

                }
            }

            base.EndHand(context);
        }


        private void UpdateCommonRow(int pot)
        {
            this.DrawCommunityCards();

            var potAsString = "Pot: " + pot;
            ConsoleConfig.WriteOnConsole(this.commonRow, this.width - 13, new string(' ', 12));
            ConsoleConfig.WriteOnConsole(this.commonRow, this.width - potAsString.Length - 2, potAsString);
        }

        private void DrawSingleCard(int row, int col, Card card)
        {
            var cardColor = this.GetCardColor(card);
            ConsoleConfig.WriteOnConsole(row, col, card.ToCard() + "  ", cardColor, ConsoleColor.White);
            ConsoleConfig.WriteOnConsole(row + 1, col, "    ", cardColor, ConsoleColor.White);
            ConsoleConfig.WriteOnConsole(row + 2, col, "  " + card.ToCard(), cardColor, ConsoleColor.White);
        }

        private void DrawSingleHideCard(int row, int col)
        {
            ConsoleConfig.WriteOnConsole(row, col, "?   ", ConsoleColor.Gray, ConsoleColor.DarkGray);
            ConsoleConfig.WriteOnConsole(row + 1, col, "    ", ConsoleColor.Gray, ConsoleColor.DarkGray);
            ConsoleConfig.WriteOnConsole(row + 2, col, "   ?", ConsoleColor.Gray, ConsoleColor.DarkGray);
        }

        private ConsoleColor GetCardColor(Card card)
        {
            switch (card.Suit)
            {
                case CardSuit.Club: return ConsoleColor.DarkGreen;
                case CardSuit.Diamond: return ConsoleColor.Blue;
                case CardSuit.Heart: return ConsoleColor.Red;
                case CardSuit.Spade: return ConsoleColor.Black;
                default: throw new ArgumentException("card.Suit");
            }
        }

        public override void StartRound(IStartRoundContext context)
        {
            this.BoardCards = context.BoardCards;
            this.UpdateCommonRow(context.CurrentPot);

            ConsoleConfig.WriteOnConsole(this.row + 1, this.width / 2 - 5, context.RoundType + "   ");
            base.StartRound(context);
        }


        public override PlayerAction GetTurn(ITurnContext context)
        {
            this.UpdateCommonRow(context.CurrentPot);
            ConsoleConfig.WriteOnConsole(this.row + 11, 2, new string(' ', 20));
            ConsoleConfig.WriteOnConsole(this.row + 11, 2, "Money: " + context.MoneyLeft.ToString());

            var action = base.GetTurn(context);

            var lastAction = (PlayerActionType)action.Type + (action.Type == (int)PlayerActionType.Fold
                ? string.Empty
                : "(" + (action.Money + ((context.MoneyToCall < 0) ? 0 : context.MoneyToCall) + ")"));

            ConsoleConfig.WriteOnConsole(this.row + 11, this.width - 26, new string(' ', 25));
            ConsoleConfig.WriteOnConsole(this.row + 11, this.width - 26, "Last act: " + lastAction);

            var moneyAfterAction = action.Type == (int)PlayerActionType.Fold
                ? context.MoneyLeft
                : context.MoneyLeft - action.Money - context.MoneyToCall;

            ConsoleConfig.WriteOnConsole(this.row + 11, 2, new string(' ', 20));
            ConsoleConfig.WriteOnConsole(this.row + 11, 2, "Money: " + context.MoneyLeft.ToString());

            return action;
        }

        private void DrawCommunityCards()
        {
            if (this.BoardCards != null)
            {
                var cardsAsString = this.BoardCards.CardsToString();
                var cardsLength = 4;
                var cardsStartCol = (this.width / 2) - (cardsLength / 2);
                var cardIndex = 0;
                var spacing = 0;

                foreach (var communityCard in this.BoardCards)
                {
                    this.DrawSingleCard(this.row + 4, (cardsStartCol + (cardIndex * 4) + spacing) - 12, communityCard);
                    cardIndex++;

                    spacing += 1;
                }
            }
        }

    }
}
