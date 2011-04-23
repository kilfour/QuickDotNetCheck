using System;
using System.Text;
using QuickDotNetCheck.ShrinkingStrategies;
using QuickGenerate.Primitives;
using Xunit;

namespace QuickDotNetCheck.Examples
{
    public class BugHouse
    {
        private int count;
        public bool Run(int a)
        {
            if (count++ >= 3 && a == 6)
                throw new Exception();
            return true;
        }
    }

    public class BugHouseTest
    {
        [Fact]
        public void VerifyAll()
        {
            var suite = new Suite(50, 20);
            var report =
                suite
                    .Using(() => new BugHouseFixtureState())
                    .Register(() => new BugHouseFixture(suite))
                    .Run();

            Assert.True(report.Succeeded(), report.Report());
        }
    }

    public class BugHouseFixtureState
    {
        public BugHouse BugHouse { get; private set; }
        public BugHouseFixtureState()
        {
            BugHouse = new BugHouse();
        }
    }

    public class BugHouseFixture : Fixture
    {
        private readonly Suite suite;
        private int input { get; set; }
        private bool output;

        public BugHouseFixture(Suite suite)
        {
            this.suite = suite;
        }

        public override void Arrange()
        {
            input = new IntGenerator(0, 20).GetRandomValue();
        }

        protected override void Act()
        {
            output = suite.Get<BugHouseFixtureState>().BugHouse.Run(input);
        }

        private SimpleValuesShrinkingStrategy<BugHouseFixture, int> shrunk;

        public override void Shrink(Func<bool> runFunc)
        {
            shrunk =
                new SimpleValuesShrinkingStrategy<BugHouseFixture, int>(
                    this,
                    e => e.input);
            shrunk.AddValues(new object[] { -1, 0, 1 });
            shrunk.Shrink(runFunc);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine(GetType().Name);
            if(!shrunk.Shrunk())
                sb.AppendFormat("input : {0}.", input);
            return sb.ToString();
        }

        [Spec]
        public void Always_Returns_True()
        {
            Ensure.True(output);
        }
    }
}