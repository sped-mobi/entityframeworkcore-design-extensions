using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Design.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.EntityFrameworkCore.Design.Internal
{


    public class EfDesignerOperationReportHandler : IOperationReportHandler
    {
        public void OnError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            TimeStamp();
            Console.WriteLine(message);
            Console.ResetColor();
        }

        public void OnWarning(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red | ConsoleColor.Yellow;
            TimeStamp();
            Console.WriteLine(message);
            Console.ResetColor();
        }

        public void OnInformation(string message)
        {
            Console.ForegroundColor = ConsoleColor.White;
            TimeStamp();
            Console.WriteLine(message);
            Console.ResetColor();
        }

        public void OnVerbose(string message)
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            TimeStamp();
            Console.WriteLine(message);
            Console.ResetColor();
        }

        public int Version { get; }

        private static void TimeStamp()
        {
            Console.Write("[");
            Console.Write(DateTime.Now.ToLongTimeString());
            Console.Write("] ");
        }
    }
}
