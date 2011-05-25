using System;
using QuickDotNetCheck;
using Xunit;

namespace QuickDotNetCheckTests
{
    // Confusingly I've buried what I'm asserting about Fixture in a Fixture ;-)
    public class PreconditionTest : Fixture 
    {
        public bool Flag { get; set; }

        [Fact(Skip = "Working on it")]
        public void This_Is_The_Test()
        {
            Flag = false; // This makes the precondition return false, ...
            Arrange();
            Execute(); // ..., so this one does not throw.
            Assert();

            Flag = true; // This makes the precondition return true, ...
            Xunit.Assert.Throws<Exception>(
                () =>
                    {
                        Arrange();
                        Execute(); // ..., so this one does throw.
                        Assert();
                    });
        }

        protected override void Act()
        {
            // setting this to false explicitely in order to show 
            // we're using the value before act is called
            Flag = false; 
        }

        [Spec]
        //[If(typeof(MyCondition))] // our sut basically
        public void foo()
        {
            throw new Exception();
        }
    }

    public class MyCondition : Condition<PreconditionTest>
    {
        public override bool Evaluate(PreconditionTest fixture)
        {
            return fixture.Flag;
        }
    }
}
