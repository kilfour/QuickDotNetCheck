using System;
using System.Text;

namespace QuickDotNetCheck.Exceptions
{
    public class FalsifiableException : Exception
    {
        public FalsifiableException(string expected, Exception exception)
            : base(expected, exception) { }

        public FalsifiableException(string expected, string actual) 
            : base( BuildMessage( expected, actual) ) { }

        public Spec Spec { get; set; }

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

    public class UnexpectedException : FalsifiableException
    {
        public UnexpectedException(Exception exception)
            : base(BuildMessage(exception), exception) { }

        private static string BuildMessage(Exception exception)
        {
            var sbMessage = new StringBuilder();
 
            sbMessage.AppendLine("Act threw : ");
            sbMessage.AppendLine(exception.ToString());
            return sbMessage.ToString();
        }
    }
}