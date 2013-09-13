using System;

namespace QuickDotNetCheck.Exceptions
{
	public class UntestedSpecsException : Exception
	{
		public UntestedSpecsException(string message)
			: base(message) { }
	}
}