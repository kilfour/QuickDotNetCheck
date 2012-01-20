using Xunit;

namespace QuickDotNetCheck.Examples.Multiple
{
    public class MySuite
    {
        [Fact]
        public void VerifyAll()
        {
            var suite = new Suite(50, 20);
            suite
                .Using(() => new Project())
                .Register(() => new AddBudget(suite))
                .Register(() => new AddPayement(suite))
                .Run();
        }
    }

    public class AddBudget : Fixture
    {
        private readonly Suite suite;

        public AddBudget(Suite suite)
        {
            this.suite = suite;
        }

        public override bool CanAct()
        {
            return suite.Get<Project>().Budget == null;
        }

        protected override void Act()
        {
            suite.Get<Project>().Budget = new Budget();
        }
    }

    public class AddPayement : Fixture
    {
        private readonly Suite suite;

        public AddPayement(Suite suite)
        {
            this.suite = suite;
        }

        protected override void Act()
        {
            suite.Get<Project>().Add(new Payement());
        }
    }
}