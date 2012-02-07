using QuickDotNetCheck;
using Xunit;

namespace QuickDotNetCheckTests.SuiteTests.RunningFixtures
{
    public class RunningFixturesTests
    {
        public RunningFixturesTests()
        {
            SomeFixtureToRun.Counter = 0;
        }

        [Fact]
        public void OneTestOneTransitionOneFixture()
        {
            var suite =
                new Suite(1)
                    .Do(1, opt => opt.Register<SomeFixtureToRun>());

            suite.Run();

            Assert.Equal(1, SomeFixtureToRun.Counter);
        }

        [Fact]
        public void TenTestOneTransitionOneFixture()
        {
            var suite =
                new Suite(10)
                    .Do(1, opt => opt.Register<SomeFixtureToRun>());

            suite.Run();

            Assert.Equal(10, SomeFixtureToRun.Counter);
        }

        [Fact]
        public void OneTestTenTransitionOneFixture()
        {
            var suite =
                new Suite(1)
                    .Do(10, opt => opt.Register<SomeFixtureToRun>());

            suite.Run();

            Assert.Equal(10, SomeFixtureToRun.Counter);
        }

        public class SomeFixtureToRun : Fixture
        {
            public static int Counter { get; set; }
            protected override void Act() { Counter++; }
        }
    }
}
