using QuickDoc;
using QuickDotNetCheck;
using QuickDotNetCheck.Exceptions;
using QuickDotNetCheck.Implementation;
using Xunit;

namespace QuickDotNetCheckTests
{
    [Doc]
	public class EnsureTests
	{
        [Fact]
        [Doc]
		public void IsTrueTrue()
		{
            Ensure.True(true);
		}

        [Fact]
        [Doc]
		public void IsFalseTrue()
		{
			Assert.Throws<FalsifiableException>(() => Ensure.True(false));
		}

        [Fact]
        [Doc]
		public void IsFalseFalse()
		{
			Ensure.False(false);
		}

        [Fact]
        [Doc]
		public void IsTrueFalse()
		{
			Assert.Throws<FalsifiableException>(() => Ensure.False(true));
		}

        [Fact]
        [Doc]
		public void IsNullNull()
		{
			Ensure.Null(null);
		}

        [Fact]
        [Doc]
		public void IsSomethingEvenTheNumberNullNull()
		{
			Assert.Throws<FalsifiableException>(() => Ensure.Null(0));
		}

        [Fact]
        [Doc]
		public void IsSomethingEvenTheNumberNullNotNull()
		{
			Ensure.NotNull(0);
		}

        [Fact]
        [Doc]
		public void IsNullNotNull()
		{
			Assert.Throws<FalsifiableException>(() => Ensure.NotNull(null));
		}

        [Fact]
        [Doc]
		public void AreTwoEqualThingsEqual()
		{
			Ensure.Equal(1, 1);
		}

        [Fact]
        [Doc]
		public void AreTwoDifferentThingsEqual()
		{
			Assert.Throws<FalsifiableException>(() => Ensure.Equal(1, 0));
		}

        [Fact]
        [Doc]
		public void AreTwoDifferentThingsNotEqual()
		{
			Ensure.NotEqual(0, 1);
		}

        [Fact]
        [Doc]
		public void AreTwoEqualThingsNotEqual()
		{
			Assert.Throws<FalsifiableException>(() => Ensure.NotEqual(1, 1));
		}

        [Fact]
        [Doc]
        public void Fail()
        {
            Assert.Throws<FalsifiableException>(() => Ensure.Fail());
        }

        [Fact]
        [Doc]
        public void IntSmallerThan()
        {
            Ensure.SmallerThan(5, 0);
        }

        [Fact]
        [Doc]
        public void IntSmallerThanFalsified()
        {
            Assert.Throws<FalsifiableException>(
                () => Ensure.SmallerThan(5, 6));

            Assert.Throws<FalsifiableException>(
                () => Ensure.SmallerThan(5, 5));
        }

        [Fact]
        [Doc]
        public void IntSmallerThanOrEqual()
        {
            Ensure.SmallerThanOrEqual(5, 0);
            Ensure.SmallerThanOrEqual(5, 5);
        }

        [Fact]
        [Doc]
        public void IntSmallerThanOrEqualFalsified()
        {
            Assert.Throws<FalsifiableException>(
                () => Ensure.SmallerThanOrEqual(5, 6));
        }

        [Fact]
        [Doc]
        public void IntGreaterThan()
        {
            Ensure.GreaterThan(5, 6);
        }

        [Fact]
        [Doc]
        public void IntGreaterThanFalsified()
        {
            Assert.Throws<FalsifiableException>(
                () => Ensure.GreaterThan(5, 4));

            Assert.Throws<FalsifiableException>(
                () => Ensure.GreaterThan(5, 5));
        }

        [Fact]
        [Doc]
        public void IntGreaterThanOrEqual()
        {
            Ensure.GreaterThanOrEqual(0, 5);
            Ensure.GreaterThanOrEqual(5, 5);
        }

        [Fact]
        [Doc]
        public void IntGreaterThanOrEqualFalsified()
        {
            Assert.Throws<FalsifiableException>(
                () => Ensure.GreaterThanOrEqual(5, 4));
        }

        [Fact]
        [Doc]
        public void DateTimeSmallerThan()
        {
            Ensure.SmallerThan(1.January(2010), 31.December(2009));
        }

        [Fact]
        [Doc]
        public void DateTimeSmallerThanFalsified()
        {
            Assert.Throws<FalsifiableException>(
                () => Ensure.SmallerThan(1.January(2000), 1.March(2000)));

            Assert.Throws<FalsifiableException>(
                () => Ensure.SmallerThan(1.January(2000), 1.January(2000)));
        }

        [Fact]
        [Doc]
        public void DateTimeSmallerThanOrEqual()
        {
            Ensure.SmallerThanOrEqual(15.January(2000), 1.January(2000));
            Ensure.SmallerThanOrEqual(15.August(1970), 15.August(1970));
        }

        [Fact]
        [Doc]
        public void DateTimeSmallerThanOrEqualFalsified()
        {
            Assert.Throws<FalsifiableException>(
                () => Ensure.SmallerThanOrEqual(15.August(1970), 16.August(1970)));
        }

        [Fact]
        [Doc]
        public void DateTimeGreaterThan()
        {
            Ensure.GreaterThan(15.August(1970), 15.August(1971));
        }

        [Fact]
        [Doc]
        public void DateTimeGreaterThanFalsified()
        {
            Assert.Throws<FalsifiableException>(
                () => Ensure.GreaterThan(15.August(1970), 15.January(1970)));

            Assert.Throws<FalsifiableException>(
                () => Ensure.GreaterThan(15.August(1970), 15.August(1970)));
        }

        [Fact]
        [Doc]
        public void DateTimeGreaterThanOrEqual()
        {
            Ensure.GreaterThanOrEqual(15.August(1970), 15.August(2000));
            Ensure.GreaterThanOrEqual(15.August(2000), 15.August(2000));
        }

        [Fact]
        [Doc]
        public void DateTimeGreaterThanOrEqualFalsified()
        {
            Assert.Throws<FalsifiableException>(
                () => Ensure.GreaterThanOrEqual(15.August(2000), 2.August(2000)));
        }
	}
}
