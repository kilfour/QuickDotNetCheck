using System;
using QuickDotNetCheck;
using Xunit;

namespace QuickDotNetCheckTests
{
    public class UntestedSpecsReportingTests : Fixture 
    {
        public Spec SystemUnderTest()
        {
            return
                new Spec("Testingk", () => { })
                .If(() => false);
        }

        [Fact]
        public void Throws()
        {
            Xunit.Assert.Throws<ApplicationException>(
                () =>
                new Suite(1, 1)
                    .Register(() => new UntestedSpecsReportingTests())
                    .Run());
        }

        protected override void Act() { }
    }
}
