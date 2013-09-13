using QuickDotNetCheck;
using QuickDotNetCheck.Exceptions;
using Xunit;

namespace QuickDotNetCheckTests.SuiteTests
{
	public class FutureSpecsTests : Fixture
	{
		[Fact]
		public void ThrowsUntestedSpec()
		{
			Assert.Throws<UntestedSpecsException>(
				() => new Suite()
				.Do(() => new SomeFixtureToRun())
				.Run());
		}

		[Fact]
		public void RunFails()
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