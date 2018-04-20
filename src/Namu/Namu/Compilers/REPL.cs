using System;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace Namu.Compilers
{
    public class REPL
    {
        static Lazy<REPL> current = new Lazy<REPL>(() => new REPL());

        public static REPL Current => current.Value;

        ScriptState<object> scriptState;
        ConsoleColor defaultColor;

        protected REPL()
        {
            InitializeAsync().Wait();
        }

        public async Task InitializeAsync()
        {
            const string defaultUsings = @"using System;
            using System.IO;
            using System.Collections.Generic;
            using System.Diagnostics;
            using System.Text;
            using System.Threading.Tasks;
            using static System.Console;";

            scriptState = await CSharpScript.RunAsync(defaultUsings);

            defaultColor = Console.ForegroundColor;
        }

        public async Task RunAsync(string code)
        {
            var returnValue = await ExecuteAsync(code);
            if (returnValue != null)
            {
                Console.WriteLine(returnValue);
            }
        }

        public async Task<object> ExecuteAsync(string code)
        {
            try
            {
                scriptState = await scriptState.ContinueWithAsync(code);
            }
            catch (CompilationErrorException e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(e.Message);
                Console.ForegroundColor = defaultColor;

                return null;
            }

            if (scriptState.ReturnValue != null && !string.IsNullOrEmpty(scriptState.ReturnValue.ToString()))
                return scriptState.ReturnValue;

            return null;
        }
    }
}
