namespace QuickDotNetCheck
{
    public class RunReport
    {
        private static readonly RunReport success = new RunReport(true, null);
        public static RunReport Success
        {
            get { return success; }
        }

        private readonly bool succeeded;
        private readonly SimplestFailCase simplestFailCase;
        public SimplestFailCase SimplestFailCase{get { return simplestFailCase; }}
        
        public RunReport(bool succeeded, SimplestFailCase simplestFailCase)
        {
            this.succeeded = succeeded;
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
    }
}