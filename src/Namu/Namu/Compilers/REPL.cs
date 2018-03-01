using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Namu.Compilers
{
    public class REPL
    {
        static Lazy<REPL> current = new Lazy<REPL>(() => new REPL());

        public static REPL Current => current.Value;

        bool isWindows;
        Process shell;

        protected REPL()
        {
            isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

            shell = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = isWindows ? "csi.exe" : "csharp",
                    UseShellExecute = false,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                }
            };

            shell.OutputDataReceived += Shell_DataReceived;
            shell.ErrorDataReceived += Shell_DataReceived;

            shell.Start();

            shell.BeginOutputReadLine();
            shell.BeginErrorReadLine();
        }

        public void Run(string code)
        {


            shell.StandardInput.WriteLine(code);
        }

        void Shell_DataReceived(object sender, DataReceivedEventArgs e)
        {
            Console.WriteLine(e.Data);
        }
    }
}
