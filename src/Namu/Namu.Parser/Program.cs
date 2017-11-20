using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Namu.Parser
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                Console.Write(">>> ");

                var namu = Console.ReadLine();

                if (namu == "exit") break;

                Console.WriteLine(Parser.Parse(namu) + "\n");
            }
        }
    }

    static class Parser
    {
        static Dictionary<string, string> typeKeywordDictionary;
        static Dictionary<Regex, Func<Match, string>> codeParserDictionary;
        const string errorMessage = "error: wrong syntax.";

        static Parser()
        {
            typeKeywordDictionary = new Dictionary<string, string>
            {
                { "integer", "int" },
                { "text", "string" }
            };

            codeParserDictionary = new Dictionary<Regex, Func<Match, string>>
            {
                { 
                    // is statement
                    new Regex("(.+?) (am|are|is) (a|an) (.+?)\\."), (match) =>
                    {
                        var name = match.Groups[1].Value;
                        var type = match.Groups[4].Value;

                        if (typeKeywordDictionary.ContainsKey(type))
                        {
                            type = typeKeywordDictionary[type];
                        }

                        return $"var {name} = default({type});";
                    }
                }
            };
        }

        public static string Parse(string code)
        {
            foreach (var item in codeParserDictionary)
            {
                if (item.Key.IsMatch(code))
                {
                    var match = item.Key.Match(code);
                    return item.Value(match);
                }
            }

            return errorMessage;
        }
    }
}
