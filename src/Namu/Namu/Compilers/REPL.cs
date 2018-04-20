using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Text;
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
                string description = GetDescription(returnValue);
                Console.WriteLine(description);
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

        string GetDescription(object obj)
        {
            if (obj is char)
            {
                return $"'{obj}'";
            }
            else if (obj is string)
            {
                return $"\"{obj}\"";
            }
            else if (obj is IEnumerable enumerable)
            {
                var collection = enumerable.OfType<object>();

                string type = CSharpName(obj.GetType());

                int count = collection.Count();
                var desc = collection.Select(x => GetDescription(x));
                string description = string.Join(", ", desc);

                return $"{type}({count}) {{ {description} }}";
            }
            else
            {
                var type = obj.GetType();
                if (type.IsClass) 
                {
                    var methods = type.GetMembers();
                    bool isOverridden = methods.Where(x => x.Name == "ToString").Any(x => x.DeclaringType != typeof(object));

                    if (isOverridden)
                    {
                        return $"[{obj}]";
                    }
                    else 
                    {
                        var props = type.GetProperties().Select(x => $"{x.Name}={GetDescription(x.GetValue(obj, null))}");
                        return  $"{obj} {{ {string.Join(", ", props)} }}";
                    }
                }
            }

            return obj.ToString();
        }

        string CSharpName(Type type)
        {
            var sb = new StringBuilder();
            var name = type.Name;
            if (!type.IsGenericType) return name;
            sb.Append(name.Substring(0, name.IndexOf('`')));
            sb.Append("<");
            sb.Append(string.Join(", ", type.GetGenericArguments().Select(t => CSharpName(t))));
            sb.Append(">");
            return sb.ToString();
        }
    }
}
