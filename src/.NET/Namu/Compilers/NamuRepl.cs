using System;
using System.Collections;
using System.Threading.Tasks;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace Namu.Compilers
{
    public class NamuRepl
    {
        static Lazy<NamuRepl> current = new Lazy<NamuRepl>(() => new NamuRepl());

        public static NamuRepl Current => current.Value;

        ScriptState<object> scriptState;

        protected NamuRepl()
        {
        }

        public async Task InitializeAsync()
        {
            const string defaultUsings = @"using System;
            using System.IO;
            using System.Collections.Generic;
            using System.Diagnostics;
            using System.Text;
            using System.Threading.Tasks;
            using static System.Console;
            using static Namu.BasicLibrary;";

            scriptState = await CSharpScript.RunAsync(defaultUsings, 
                ScriptOptions.Default.WithReferences(typeof(Namu.BasicLibrary).Assembly));
        }

        public async Task<ReplResult> RunAsync(string namuCode)
        {
            string csharpCode = ToCsharpCode(namuCode);
#if DEBUG
            Console.WriteLine($"[C#] {csharpCode}");
#endif
            try
            {
                object returnValue = await ExecuteAsync(csharpCode);
                return new ReplResult(returnValue);
            }
            catch (CompilationErrorException e)
            {
                return new ReplResult(e);
            }
            catch (Exception e)
            {
                return new ReplResult(e);
            }
        }

        async Task<object> ExecuteAsync(string csharpCode)
        {
            if (scriptState == null)
                await InitializeAsync();

			scriptState = await scriptState.ContinueWithAsync(csharpCode);

            if (scriptState.ReturnValue != null && !string.IsNullOrEmpty(scriptState.ReturnValue.ToString()))
                return scriptState.ReturnValue;

            return null;
        }

        string ToCsharpCode(string namuCode)
        {
            return Parser.Parse(namuCode);
        }
    }

    public enum ResultType
    {
        Success,
        SuccessWithReturnValue,
        CompilationError,
        Fail
    }

    public class ReplResult
    {
        public ResultType Result { get; private set; }
        public string ReturnString { get; private set; }
        public Exception Exception { get; private set; }

        public ReplResult(object obj = null)
        {
            if (obj != null) 
            {
                Result = ResultType.SuccessWithReturnValue;
                ReturnString = GetDescription(obj);
            }
            else
            {
				Result = ResultType.Success;
                ReturnString = string.Empty;
            }
        }

        public ReplResult(CompilationErrorException exception)
        {
            Result = ResultType.CompilationError;
            ReturnString = exception.Message;
            Exception = exception;
        }

        public ReplResult(Exception exception)
        {
            Result = ResultType.Fail;
            ReturnString = exception.Message;
            Exception = exception;
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
                        return $"{obj} {{ {string.Join(", ", props)} }}";
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
