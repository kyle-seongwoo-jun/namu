using System;
using System.Collections.Generic;
using System.Text;

namespace Namu
{
    public static class BasicLibrary
    {
        public static void print(object value)
        {
            Console.WriteLine(value);
        }

        public static void exit()
        {
            Environment.Exit(0);
        }
    }
}
