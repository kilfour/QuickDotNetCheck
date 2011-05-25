using QuickDotNetCheck;
using Xunit;

namespace QuickDotNetCheckTests
{
    public class PreconditionTest : Fixture
    {
        private bool preconditionHolds;
        private bool specWasVerified;

        // This method is the SUT
        public Spec foo()
        {
            return
                new Spec("Testingk", () => { specWasVerified = true; })
                .If(() => preconditionHolds);
        }

        // Setting preconditionHolds to false explicitely in order to show 
        // we're using the precondition before act is called
        protected override void Act()
        {
            preconditionHolds = false;
        }

        [Fact]
        public void WhenPreconditionDoesNotHoldThenSpecIsNotVerified()
        {
            preconditionHolds = false;
            Execute();
            Assert();
            Xunit.Assert.False(specWasVerified);
        }

        [Fact]
        public void WhenPreconditionHoldsThenSpecIsVerified()
        {
            preconditionHolds = true;
            Execute();
            Assert();
            Xunit.Assert.True(specWasVerified);
        }

        
    }
}
