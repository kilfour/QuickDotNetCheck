using System;
using QuickDotNetCheck;
using QuickDotNetCheck.Exceptions;
using Xunit;

namespace QuickDotNetCheckTests
{
    public class UntestedSpecsReportingTests : Fixture 
    {
        public Spec SystemUnderTest()
        {
            return
                new Spec(() => { })
                .If(() => false);
        }

        [Fact]
        public void Throws()
        {
			Assert.Throws<UntestedSpecsException>(
                () =>
                new Suite()
                    .Do(() => new UntestedSpecsReportingTests())
                    .Run());
        }

        protected override void Act() { }
    }
}
