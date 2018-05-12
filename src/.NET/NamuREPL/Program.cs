using System;
using System.Threading.Tasks;
using Namu.Compilers;

namespace NamuREPL
{
    class Program
    {
		static bool isCancel;
        const bool showCsharpCode = true;

        static async Task Main(string[] args)
        {
            Console.CancelKeyPress += Console_CancelKeyPress;

            var repl = REPL.Current;
            await repl.InitializeAsync();

            while (true)
            {
                Console.Write(">>> ");

                string namuCode = Console.ReadLine();

                if (namuCode == "exit" || namuCode == null || isCancel) break;
                else if (namuCode == string.Empty) continue;

                string csharpCode = Parser.Parse(namuCode);
                Console.WriteLine($"[C#] {csharpCode}");

                var result = await repl.RunAsync(csharpCode);
                switch (result.Result)
                {
                    case ResultType.SuccessWithReturnValue:
                        Console.WriteLine(result.ReturnString);
                        break;
                    
                    case ResultType.Fail:
					case ResultType.CompilationError:
                        ConsoleManager.PrintError(result.ReturnString);
                        break;
                }
            }
        }

        static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            isCancel = e.Cancel;
        }
    }
}
