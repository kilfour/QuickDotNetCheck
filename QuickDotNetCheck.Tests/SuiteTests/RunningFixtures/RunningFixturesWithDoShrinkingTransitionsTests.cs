using QuickDotNetCheck;
using Xunit;

namespace QuickDotNetCheckTests.SuiteTests.RunningFixtures
{
    public class RunningFixturesWithDoShrinkingTransitionsTests
    {
        [Fact]
        public void ReverseEngineering()
        {
            var suite =
                new Suite(1, 0)
                    .Do<SomeFixtureToRun>()
                    .Do<SomeFixtureThatFails>();

            var report = suite.Run();
            Assert.True(report.Failed());
            Assert.False(report.Succeeded());
            Assert.Equal(1, report.SimplestFailCase.Fixtures.Count);
            Assert.Equal(typeof(SomeFixtureThatFails), report.SimplestFailCase.Fixtures[0].GetType());
        }

        public class SomeFixtureToRun : Fixture
        {
            protected override void Act() {}
        }

        public class SomeFixtureThatFails : Fixture
        {
            protected override void Act() {  }
            
            [Spec]
            public void Kaboom()
            {
                Ensure.Fail();
            }
        }
    }
}