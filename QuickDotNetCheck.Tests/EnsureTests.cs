using QuickDotNetCheck;
using QuickDotNetCheck.Exceptions;
using QuickDotNetCheck.Implementation;
using Xunit;

namespace QuickDotNetCheckTests
{
	public class EnsureTests
	{
        [Fact]
		public void IsTrueTrue()
		{
            Ensure.True(true);
		}

        [Fact]
		public void IsFalseTrue()
		{
			Assert.Throws<FalsifiableException>(() => Ensure.True(false));
		}

        [Fact]
		public void IsFalseFalse()
		{
			Ensure.False(false);
		}

        [Fact]
		public void IsTrueFalse()
		{
			Assert.Throws<FalsifiableException>(() => Ensure.False(true));
		}

        [Fact]
		public void IsNullNull()
		{
			Ensure.Null(null);
		}

        [Fact]
		public void IsSomethingEvenTheNumberNullNull()
		{
			Assert.Throws<FalsifiableException>(() => Ensure.Null(0));
		}

        [Fact]
		public void IsSomethingEvenTheNumberNullNotNull()
		{
			Ensure.NotNull(0);
		}

        [Fact]
		public void IsNullNotNull()
		{
			Assert.Throws<FalsifiableException>(() => Ensure.NotNull(null));
		}

        [Fact]
		public void AreTwoEqualThingsEqual()
		{
			Ensure.Equal(1, 1);
		}

        [Fact]
		public void AreTwoDifferentThingsEqual()
		{
			Assert.Throws<FalsifiableException>(() => Ensure.Equal(1, 0));
		}

        [Fact]
		public void AreTwoDifferentThingsNotEqual()
		{
			Ensure.NotEqual(0, 1);
		}

        [Fact]
		public void AreTwoEqualThingsNotEqual()
		{
			Assert.Throws<FalsifiableException>(() => Ensure.NotEqual(1, 1));
		}

        [Fact]
        public void Fail()
        {
            Assert.Throws<FalsifiableException>(() => Ensure.Fail());
        }

        [Fact]
        public void IntSmallerThan()
        {
            Ensure.SmallerThan(5, 0);
        }

        [Fact]
        public void IntSmallerThanFalsified()
        {
            Assert.Throws<FalsifiableException>(
                () => Ensure.SmallerThan(5, 6));

            Assert.Throws<FalsifiableException>(
                () => Ensure.SmallerThan(5, 5));
        }

        [Fact]
        public void IntSmallerThanOrEqual()
        {
            Ensure.SmallerThanOrEqual(5, 0);
            Ensure.SmallerThanOrEqual(5, 5);
        }

        [Fact]
        public void IntSmallerThanOrEqualFalsified()
        {
            Assert.Throws<FalsifiableException>(
                () => Ensure.SmallerThanOrEqual(5, 6));
        }

        [Fact]
        public void IntGreaterThan()
        {
            Ensure.GreaterThan(5, 6);
        }

        [Fact]
        public void IntGreaterThanFalsified()
        {
            Assert.Throws<FalsifiableException>(
                () => Ensure.GreaterThan(5, 4));

            Assert.Throws<FalsifiableException>(
                () => Ensure.GreaterThan(5, 5));
        }

        [Fact]
        public void IntGreaterThanOrEqual()
        {
            Ensure.GreaterThanOrEqual(0, 5);
            Ensure.GreaterThanOrEqual(5, 5);
        }

        [Fact]
        public void IntGreaterThanOrEqualFalsified()
        {
            Assert.Throws<FalsifiableException>(
                () => Ensure.GreaterThanOrEqual(5, 4));
        }

        [Fact]
        public void DateTimeSmallerThan()
        {
            Ensure.SmallerThan(1.January(2010), 31.December(2009));
        }

        [Fact]
        public void DateTimeSmallerThanFalsified()
        {
            Assert.Throws<FalsifiableException>(
                () => Ensure.SmallerThan(1.January(2000), 1.March(2000)));

            Assert.Throws<FalsifiableException>(
                () => Ensure.SmallerThan(1.January(2000), 1.January(2000)));
        }

        [Fact]
        public void DateTimeSmallerThanOrEqual()
        {
            Ensure.SmallerThanOrEqual(15.January(2000), 1.January(2000));
            Ensure.SmallerThanOrEqual(15.August(1970), 15.August(1970));
        }

        [Fact]
        public void DateTimeSmallerThanOrEqualFalsified()
        {
            Assert.Throws<FalsifiableException>(
                () => Ensure.SmallerThanOrEqual(15.August(1970), 16.August(1970)));
        }

        [Fact]
        public void DateTimeGreaterThan()
        {
            Ensure.GreaterThan(15.August(1970), 15.August(1971));
        }

        [Fact]
        public void DateTimeGreaterThanFalsified()
        {
            Assert.Throws<FalsifiableException>(
                () => Ensure.GreaterThan(15.August(1970), 15.January(1970)));

            Assert.Throws<FalsifiableException>(
                () => Ensure.GreaterThan(15.August(1970), 15.August(1970)));
        }

        [Fact]
        public void DateTimeGreaterThanOrEqual()
        {
            Ensure.GreaterThanOrEqual(15.August(1970), 15.August(2000));
            Ensure.GreaterThanOrEqual(15.August(2000), 15.August(2000));
        }

        [Fact]
        public void DateTimeGreaterThanOrEqualFalsified()
        {
            Assert.Throws<FalsifiableException>(
                () => Ensure.GreaterThanOrEqual(15.August(2000), 2.August(2000)));
        }
	}
}
