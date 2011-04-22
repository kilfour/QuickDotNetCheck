using System.Text;
using QuickDotNetCheck.Exceptions;

namespace QuickDotNetCheck
{
    public class RunReport
    {
        private static readonly RunReport success = new RunReport(true, null, null);
        public static RunReport Success
        {
            get { return success; }
        }

        private readonly bool succeeded;
        private readonly FalsifiableException failure;
        private readonly SimplestFailCase simplestFailCase;
        public SimplestFailCase SimplestFailCase{get { return simplestFailCase; }}
        
        public RunReport(bool succeeded, FalsifiableException failure, SimplestFailCase simplestFailCase)
        {
            this.succeeded = succeeded;
            this.failure = failure;
            this.simplestFailCase = simplestFailCase;
        }

        public bool Failed()
        {
            return !succeeded;
        }

        public bool Succeeded()
        {
            return succeeded;
        }

        public string Report()
        {
            if (succeeded)
                return "Passed";

            var sbReport = new StringBuilder();
            sbReport.AppendLine();
            sbReport.AppendLine("----------------------------------------------------------");
            sbReport.AppendLine(failure.Message);
            if (failure.InnerException != null)
                sbReport.AppendLine(failure.InnerException.Message);
            //sbReport.AppendFormat(
            //    "Falsifiable after {0} test(s), {1} Actions(s).",
            //    0,
            //    0);
            if (simplestFailCase != null)
                sbReport.AppendLine(simplestFailCase.Report());
            return sbReport.ToString();
        }
    }
}