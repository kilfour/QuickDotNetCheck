using System;
using System.Text;

namespace QuickDotNetCheck.Exceptions
{
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