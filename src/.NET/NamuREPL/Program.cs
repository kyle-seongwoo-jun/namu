﻿using System;
using System.Collections.Generic;
using Namu.Compilers;

namespace NamuREPL
{
    class Program
    {
		static bool isCancel;
        const bool showCsharpCode = true;

        static void Main(string[] args)
        {
            Console.CancelKeyPress += Console_CancelKeyPress;

            var repl = REPL.Current;

            var list = new List<string>();

            while (true)
            {
                Console.Write("namu> ");

                var namu = Console.ReadLine();

                if (namu == "history") { list.ForEach(x => Console.WriteLine(x)); Console.WriteLine(); continue; }
                else if (namu == "clear") { list.Clear(); continue; }
                else if (namu == "exit" || namu == null || isCancel) break;
                else if (namu == string.Empty) continue;

                var result = Parser.Parse(namu);
                list.Add(result);
				if (showCsharpCode) Console.WriteLine($"C#> {result}");
                repl.RunAsync(result).Wait();
            }
        }

        static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            isCancel = e.Cancel;
        }
    }
}