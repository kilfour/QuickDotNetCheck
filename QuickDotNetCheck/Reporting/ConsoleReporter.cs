using System;

namespace QuickDotNetCheck.Reporting
{
    public class ConsoleReporter : IReporter
    {
        public void Write(string message)
        {
            Console.Write(message);
        }

        public void WriteLine(string message)
        {
            Console.WriteLine(message);
        }
    }
}