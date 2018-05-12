using System;

namespace NamuREPL
{
    public static class ConsoleManager
    {
        static readonly ConsoleColor defaultColor;

        static ConsoleManager()
        {
            defaultColor = Console.ForegroundColor;
        }

        public static void PrintError(string text)
        {
            Console.ForegroundColor = ConsoleColor.Red;

            Console.WriteLine(text);

            Console.ForegroundColor = defaultColor;
        }
    }
}
