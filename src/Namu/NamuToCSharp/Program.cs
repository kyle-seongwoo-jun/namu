using System;
using System.Collections.Generic;
using Namu.Compilers;

namespace NamuToCSharp
{
    class Program
    {
        static void Main(string[] args)
        {
            var list = new List<string>();

            while (true)
            {
                Console.Write(">>> ");

                var namu = Console.ReadLine();

                if (namu == "history") { list.ForEach(x => Console.WriteLine(x)); Console.WriteLine(); continue; }
                else if (namu == "clear") { list.Clear(); continue; }
                else if (namu == "exit") break;
                else if (namu == "") continue;

                var result = Parser.Parse(namu);
                list.Add(result);
                Console.WriteLine(result + "\n");
            }
        }
    }

}
