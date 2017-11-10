using System;

namespace Poker
{
    public static class ConsoleConfig
    {
        public static void WriteOnConsole(int row, int col, string text, ConsoleColor foregroundColor = ConsoleColor.Gray, ConsoleColor backgroundColor = ConsoleColor.Black)
        {
            Console.ForegroundColor = foregroundColor;
            Console.BackgroundColor = backgroundColor;
            Console.SetCursorPosition(col, row);
            Console.Write(text);
        }

        public static void ConfigConsole(int GameHeight, int GameWidth)
        {
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.BufferHeight = Console.WindowHeight = GameHeight;
            Console.BufferWidth = Console.WindowWidth = GameWidth;
            Console.OutputEncoding = System.Text.Encoding.UTF8;
        }

        public static void DrawBox(int x, int y, int w, int h, ConsoleColor color, string border)
        {
            ConsoleConfig.WriteOnConsole(y, x, new string(border[0], w), color);
            ConsoleConfig.WriteOnConsole(y + h, x, new string(border[2], w), color);
            ConsoleConfig.WriteOnConsole(y, x, new string(border[4], 1), color);
            ConsoleConfig.WriteOnConsole(y, x + w - 1, new string(border[5], 1), color);
            ConsoleConfig.WriteOnConsole(y + h, x, new string(border[7], 1), color);
            ConsoleConfig.WriteOnConsole(y + h, x + w - 1, new string(border[6], 1), color);
            for (var i = 1; i < h; i++)
            {
                ConsoleConfig.WriteOnConsole(y + i, x, new string(border[3], 1), color);
                ConsoleConfig.WriteOnConsole(y + i, x + w - 1, new string(border[1], 1), color);
            }
        }

        public static void SetInput()
        {
            ConsoleConfig.WriteOnConsole(25, 1, new string(' ', 66 - 2), ConsoleColor.White);
            Console.SetCursorPosition(1, 25);
        }
    }

    public class ConsoleInterface
    {
        private readonly string ProgramName;
        private readonly int row;
        private readonly int width;

        public ConsoleInterface(string ProgramName, int row, int width)
        {
            this.ProgramName = ProgramName;
            this.row = row;
            this.width = width;
            ConsoleConfig.ConfigConsole(row, width);
            DrawTitleBox();
            DrawInfoBox();
            DrawGameBox();
            DrawInputBox();
        }

        private void DrawTitleBox()
        {
            ConsoleConfig.DrawBox(0, 0, this.width, 4, ConsoleColor.Green, "═║═║╔╗╝╚");
            ConsoleConfig.WriteOnConsole(2, width / 2 - ProgramName.Length / 2 - 1, ProgramName, ConsoleColor.Green);
        }

        private void DrawInfoBox()
        {
            ConsoleConfig.DrawBox(0, 5, this.width, 2, ConsoleColor.Green, "─│─│╭╮╯╰");
        }

        private void DrawGameBox()
        {
            ConsoleConfig.DrawBox(0, 8, this.width, 12, ConsoleColor.Green, "─│─│╭╮╯╰");
        }

        public void DrawGameBoard()
        {
            ConsoleConfig.WriteOnConsole(10, 0, "├──────╯", ConsoleColor.Green);
            ConsoleConfig.WriteOnConsole(9, 7, "│", ConsoleColor.Green);
            ConsoleConfig.WriteOnConsole(8, 7, "┬", ConsoleColor.Green);
            ConsoleConfig.DrawBox(25, 16, 13, 4, ConsoleColor.Green, "─│─│╭╮┴┴");
            ConsoleConfig.DrawBox(17, 11, 28, 4, ConsoleColor.Green, "─│─│╭╮╯╰");
        }

        private void DrawInputBox()
        {
            ConsoleConfig.DrawBox(0, 24, this.width, 2, ConsoleColor.Green, "─│─│╭╮╯╰");
        }

        public void SetCursor(int x, int y)
        {
            Console.SetCursorPosition(y, x);
        }

        public void SetInput()
        {
            ConsoleConfig.WriteOnConsole(25, 1, new string(' ', width - 2), ConsoleColor.White);
            Console.SetCursorPosition(1, 25);
        }

        public void SetMsg(string msg)
        {
            ConsoleConfig.WriteOnConsole(21, 1, msg, ConsoleColor.White);
        }

        public void ClearMsg()
        {
            ConsoleConfig.WriteOnConsole(21, 0, new string(' ', width), ConsoleColor.White);
            ConsoleConfig.WriteOnConsole(22, 0, new string(' ', width), ConsoleColor.White);
            ConsoleConfig.WriteOnConsole(23, 0, new string(' ', width), ConsoleColor.White);

            ConsoleConfig.WriteOnConsole(25, 1, new string(' ', width - 2), ConsoleColor.White);
        }
    }

    class Program
    {
        private const string ProgramName = "\u2660 Powker \u2660";
        private const int GameHeight = 28;
        private const int GameWidth = 66;

        public static void Main()
        {
            Random rnd = new Random();
            var consoleInterface = new ConsoleInterface(ProgramName, GameHeight, GameWidth);
            var consolePlayer = new PlayerUI(new Player(0, Environment.MachineName + '-' + rnd.Next(999).ToString()), 8, GameWidth, 9);
            var consoleManager = new Manager(consoleInterface, consolePlayer);

            consoleManager.Start();
        }
    }
}
