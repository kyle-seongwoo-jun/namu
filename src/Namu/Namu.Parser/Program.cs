﻿using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Namu.Parser
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
                { "number", "double" },
                { "text", "string" }
            };

            codeParserDictionary = new Dictionary<Regex, Func<Match, string>>
            {
                { 
                    // declare variable with am/are/is and a/an operator. 
                    new Regex(@"([A-Za-z]+) (am|are|is) (a|an) ([A-Za-z]+)\."), (match) =>
                    {
                        var name = match.Groups[1].Value;
                        var type = match.Groups[4].Value;
                        bool isPrimaryType = false;

                        if (typeKeywordDictionary.ContainsKey(type))
                        {
                            type = typeKeywordDictionary[type];

                            isPrimaryType = (type != "string");
                        }

                        if (isPrimaryType)
                        {
                            return $"var {name} = default({type});";
                        }
                        else if (type == "string")
                        {
                            return $"var {name} = {type}.Empty;";
                        }
                        else
                        {
                            return $"var {name} = new {type}();";
                        }
                    }
                },
                { 
                    // declare variable with var keyword.
                    new Regex(@"([A-Za-z]+) (am|are|is) ([A-Za-z]+)\."), (match) =>
                    {
                        var name = match.Groups[1].Value;
                        var value = match.Groups[3].Value;
                                         
                        return $"var {name} = {value};";
                    }
                },
                { 
                    // call method
                    new Regex(@"([A-Za-z]+) the ([A-Za-z0-9]{1,5}( *, *[A-Za-z0-9]{1,5})*)\."), (match) =>
                    {
                        var functionName = match.Groups[1].Value;
                        var parameters = match.Groups[2].Value;

                        return $"{functionName}({parameters});";
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
