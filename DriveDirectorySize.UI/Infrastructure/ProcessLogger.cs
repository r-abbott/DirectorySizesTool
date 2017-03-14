using DriveDirectorySize.Domain.Contracts;
using System;

namespace DriveDirectorySize.UI.Infrastructure
{
    public class ProcessLogger : ILog
    {
        public void Write(string text)
        {
            Console.Write(text);
        }

        public void WriteConcurrent(string text)
        {
            EraseLine();
            Write(text);
        }

        private void EraseLine()
        {
            string eraser = new string(' ', Console.BufferWidth - 1);
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write(eraser);
            Console.SetCursorPosition(0, Console.CursorTop);
        }

        public void WriteLine(string text)
        {
            Console.WriteLine(text);
        }
    }
}
