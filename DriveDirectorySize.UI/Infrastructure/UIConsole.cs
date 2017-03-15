using System;

namespace DriveDirectorySize.UI.Infrastructure
{
    public class UIConsole
    {
        private static readonly ConsoleColor _defaultForeground = ConsoleColor.White;
        private static readonly ConsoleColor _defaultBackground = ConsoleColor.Black;

        static UIConsole()
        {
            Console.WindowHeight = 40;
            Console.WindowWidth = 150;
        }

        public static void Write(string text)
        {
            SetDefaultColors();
            Console.Write(text);
        }

        public static void WriteLine(string text)
        {
            SetDefaultColors();
            Console.WriteLine(text);
        }

        public static void WriteColor(ConsoleColor color, string text)
        {
            Console.ForegroundColor = color;
            Console.Write(text);
            SetDefaultColors();
        }

        public static void WriteColorLine(ConsoleColor color, string text)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(text);
            SetDefaultColors();
        }

        public static void WriteConcurrent(string text)
        {
            EraseLine();
            Write(text);
        }

        public static void WriteHorizontalRule()
        {
            WriteLine(new string('-', Console.BufferWidth - 1));
        }

        public static void EraseLine()
        {
            string eraser = new string(' ', Console.BufferWidth - 1);
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write(eraser);
            Console.SetCursorPosition(0, Console.CursorTop);
        }

        public static string ReadLine()
        {
            return Console.ReadLine();
        }

        private static void SetDefaultColors()
        {
            Console.BackgroundColor = _defaultBackground;
            Console.ForegroundColor = _defaultForeground;
        }
    }
}
