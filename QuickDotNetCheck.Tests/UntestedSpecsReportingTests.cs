using System;
using QuickDotNetCheck;
using Xunit;

namespace QuickDotNetCheckTests
{
    public class UntestedSpecsReportingTests : Fixture 
    {
        public bool Flag { get; set; }

        [Fact(Skip = "Working on it")]
        public void This_Is_The_Test()
        {
            Xunit.Assert.Throws<ApplicationException>(
                () =>
                new Suite(1, 1)
                    .Register(() => new UntestedSpecsReportingTests())
                    .Run());
        }

        protected override void Act() { }

        [Spec]
        //[If(typeof(AlwaysFalse))]
        public void TheUntestedSpec() { }
    }

    public class AlwaysFalse : Condition<Fixture>
    {
        public override bool Evaluate(Fixture fixture)
        {
            return false;
        }
    }
}
