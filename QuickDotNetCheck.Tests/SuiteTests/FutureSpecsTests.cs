using QuickDotNetCheck;
using QuickDotNetCheck.Exceptions;
using Xunit;

namespace QuickDotNetCheckTests.SuiteTests
{
	public class FutureSpecsTests : Fixture
	{
		[Fact(Skip="work in progress")]
		public void DoesNotThrow()
		{
			Assert.Throws<UntestedSpecsException>(
				() => new Suite()
				.Do(() => new SomeFixtureToRun())
				.Run());
		}

		[Fact(Skip = "work in progress")]
		public void Throws()
		{
			Assert.Throws<RunReport>(
				() =>
				new Suite()
					.Do(() => new SomeFixtureToRun())
					.Do(() => new SomeOtherFixtureToRun())
					.Run());
		}

		protected override void Act() { }

		public class SomeFixtureToRun : Fixture
		{
			public Spec SystemUnderTest()
			{
				return
					new Spec(Ensure.Fail)
						.For<SomeOtherFixtureToRun>();
			}
		}

		public class SomeOtherFixtureToRun : Fixture { }
	}
}