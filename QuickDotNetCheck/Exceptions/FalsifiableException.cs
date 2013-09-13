using System;
using System.Text;

namespace QuickDotNetCheck.Exceptions
{
    public class FalsifiableException : Exception
    {
        public FalsifiableException(string expected, Exception exception)
            : base(expected, exception) { }

        public FalsifiableException(string expected, string actual) 
            : base( BuildMessage( expected, actual, "") ) { }

        public FalsifiableException(string expected, string actual, string message)
            : base(BuildMessage(expected, actual, message)) { }

        public Spec Spec { get; set; }

        private static string BuildMessage(string expected, string actual, string message)
        {
            var sbMessage = new StringBuilder();
            sbMessage.AppendLine();
            sbMessage.AppendFormat("Expected : {0}.", expected);
            sbMessage.AppendLine();
            sbMessage.AppendFormat("Actual : {0}.", actual);
            sbMessage.AppendLine();
            if(message != string.Empty)
            {
                sbMessage.AppendFormat(message);
                sbMessage.AppendLine();
            }
            return sbMessage.ToString();
        }
    }
}