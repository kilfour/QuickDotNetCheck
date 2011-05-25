using System;
using QuickDotNetCheck;
using Xunit;

namespace QuickDotNetCheckTests
{
    public class PostconditionTest : Fixture
    {
        private Func<bool> postconditionHolds;
        private bool specWasVerified;

        public Spec SystemUnderTest()
        {
            return
                new Spec("Testingk", () => { specWasVerified = true; })
                    .IfAfter(() => postconditionHolds());
        }

        protected override void Act()
        {
            postconditionHolds();
        }

        [Fact]
        public void WhenPostconditionDoesNotHoldThenSpecIsNotVerified()
        {
            postconditionHolds = () => false;
            Execute();
            Assert();
            Xunit.Assert.False(specWasVerified);
        }

        [Fact]
        public void WhenPostconditionHoldsThenSpecIsVerified()
        {
            postconditionHolds = () => true;
            Execute();
            Assert();
            Xunit.Assert.True(specWasVerified);
        }
    }
}