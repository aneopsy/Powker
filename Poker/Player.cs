using System;
using System.Collections.Generic;
using System.Linq;

namespace Poker
{

    public class Player : APlayer
    {
        private int row;

        public Player(int row, string PlayerName)
        {
            this.row = row;
            this.Name = PlayerName;
        }

        public override string Name { get; set; }

        public override PlayerAction GetTurn(ITurnContext context)
        {
            this.DrawPlayerOptions(context.MoneyToCall);
            ConsoleConfig.SetInput();
            while (true)
            {
                var key = Console.ReadLine().ToUpper();
                PlayerAction action = null;
                switch (key)
                {
                    case "C":
                        action = PlayerAction.CheckOrCall();
                        break;
                    case "R":
                        action = PlayerAction.Raise(10);
                        break;
                    case "F":
                        action = PlayerAction.Fold();
                        break;
                    case "A":
                        action = context.MoneyLeft > 0
                                     ? PlayerAction.Raise(context.MoneyLeft)
                                     : PlayerAction.CheckOrCall();
                        break;
                }

                if (action != null)
                {
                    return action;
                }
            }
        }

        private void DrawPlayerOptions(int moneyToCall)
        {
            var col = 2;
            ConsoleConfig.WriteOnConsole(22, col, "Select action: [");
            col += 16;
            ConsoleConfig.WriteOnConsole(22, col, "C", ConsoleColor.Yellow);
            col++;
            ConsoleConfig.WriteOnConsole(22, col, "]heck/[");
            col += 7;
            ConsoleConfig.WriteOnConsole(22, col, "C", ConsoleColor.Yellow);
            col++;

            var callString = moneyToCall <= 0 ? "]all, [" : "]all(" + moneyToCall + "), [";

            ConsoleConfig.WriteOnConsole(22, col, callString);
            col += callString.Length;
            ConsoleConfig.WriteOnConsole(22, col, "R", ConsoleColor.Yellow);
            col++;
            ConsoleConfig.WriteOnConsole(22, col, "]aise, [");
            col += 8;
            ConsoleConfig.WriteOnConsole(22, col, "F", ConsoleColor.Yellow);
            col++;
            ConsoleConfig.WriteOnConsole(22, col, "]old, [");
            col += 7;
            ConsoleConfig.WriteOnConsole(22, col, "A", ConsoleColor.Yellow);
            col++;
            ConsoleConfig.WriteOnConsole(22, col, "]ll-in");
        }
    }
}
