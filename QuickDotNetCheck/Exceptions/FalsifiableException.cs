using System;
using System.Reflection;
using System.Text;

namespace QuickDotNetCheck.Exceptions
{
    public class FalsifiableException : Exception
    {
        public FalsifiableException(string expected, string actual) 
            : base( BuildMessage( expected, actual) ) { }

        public MethodInfo Spec { get; set; }

        private static string BuildMessage(string expected, string actual)
        {
            var sbMessage = new StringBuilder();
            sbMessage.AppendLine();
            sbMessage.AppendFormat("Expected : {0}.", expected);
            sbMessage.AppendLine();
            sbMessage.AppendFormat("Actual : {0}.", actual);
            sbMessage.AppendLine();
            return sbMessage.ToString();
        }
    }
}