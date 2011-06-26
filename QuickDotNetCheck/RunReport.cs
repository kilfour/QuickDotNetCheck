using System;
using System.Text;
using QuickDotNetCheck.Exceptions;

namespace QuickDotNetCheck
{
    public class RunReport : Exception
    {
        private readonly SimplestFailCase simplestFailCase;

        public SimplestFailCase SimplestFailCase{get { return simplestFailCase; }}

        public RunReport(int testNumber, int fixtureNumber, FalsifiableException failure, SimplestFailCase simplestFailCase)
            : base(GetMessage(testNumber, fixtureNumber, failure, simplestFailCase), failure)
        {
            this.simplestFailCase = simplestFailCase;
        }

        public static string GetMessage(int testNumber, int fixtureNumber, FalsifiableException failure, SimplestFailCase simplestFailCase)
        {
            var sb = new StringBuilder();
            sb.AppendLine();
            sb.AppendFormat("Ran {0} test, {1} fixtures.", testNumber, fixtureNumber);
            sb.AppendLine(); 
            if (failure.Spec != null)
                sb.AppendLine("Spec '" + failure.Spec.Name + "' does not hold.");
            if (simplestFailCase != null)
                sb.Append(simplestFailCase.Report());
            return sb.ToString();
        }
    }
}