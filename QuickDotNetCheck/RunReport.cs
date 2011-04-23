using System;
using QuickDotNetCheck.Exceptions;

namespace QuickDotNetCheck
{
    public class RunReport : Exception
    {
        private readonly SimplestFailCase simplestFailCase;
        public SimplestFailCase SimplestFailCase{get { return simplestFailCase; }}
        
        public RunReport(FalsifiableException failure, SimplestFailCase simplestFailCase)
            : base(simplestFailCase.Report(), failure)
        {
            this.simplestFailCase = simplestFailCase;
        }

        public RunReport(FalsifiableException failure) : base("",failure) { }
    }
}