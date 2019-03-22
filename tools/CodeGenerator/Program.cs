using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace CodeGenerator
{
    public static class Program
    {
        private static StringBuilder Builder;
        private static ClipboardWriter Writer;
        public static void Main()
        {
            Before();

            After();
        }

        private static void After()
        {
            Writer.Clip();
            Console.WriteLine(Builder);
            Console.ReadKey();
        }

        private static void Before()
        {
            Builder = new StringBuilder();
            Writer = ClipboardWriter.CreateForStringBuilder(Builder);
        }

        private static void Write(string value)
        {
            Writer.Write(value);
        }

        private static void WriteLine(string value)
        {
            Writer.WriteLine(value);
        }

        private static void WriteLine()
        {
            Writer.WriteLine();
        }

        private static IDisposable OpenBlock()
        {
            return Writer.OpenBlock();
        }
    }
}
