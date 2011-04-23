using System;
using QuickDotNetCheck.ShrinkingStrategies;
using Xunit;

namespace QuickDotNetCheckTests
{
    public class ShrinkingAnIntTests
    {
        // we only shrink properties, not fields.
        public int theInt { get; set; }

        [Fact]
        public void SimpleShrinking()
        {
            Func<bool> runFunc = () => true; // means fail always

            theInt = 42;

            var shrinkStrat = new SimpleValuesShrinkingStrategy<ShrinkingAnIntTests>(
                this,
                e => e.theInt);
            shrinkStrat.AddValues(new object[] { -1, 0, 1 });

            shrinkStrat.Shrink(runFunc);

            Assert.True(shrinkStrat.Shrunk());
        }

        [Fact]
        public void NotShrinking()
        {
            Func<bool> runFunc =
                () =>
                {
                    if (theInt == 42) return true; // means failure
                    return false;
                };

            theInt = 42;

            var shrinkStrat = 
                new SimpleValuesShrinkingStrategy<ShrinkingAnIntTests>(
                    this,
                    e => e.theInt);
            shrinkStrat.AddValues(new object[] { -1, 0, 1 });
            shrinkStrat.Shrink(runFunc);

            Assert.False(shrinkStrat.Shrunk());
        }

        [Fact]
        public void SimpleShrinking_Alternative_Constructor()
        {
            Func<bool> runFunc = () => true; // means fail always

            theInt = 42;

            var shrinkStrat = new SimpleValuesShrinkingStrategy<ShrinkingAnIntTests>(
                this,
                e => e.theInt,
                (e, i) => e.theInt = (int)i);
            shrinkStrat.AddValues(new object[] { -1, 0, 1 });
            shrinkStrat.Shrink(runFunc);

            Assert.True(shrinkStrat.Shrunk());
        }

        [Fact]
        public void NotShrinking_Alternative_Constructor()
        {
            Func<bool> runFunc =
                () =>
                {
                    if (theInt == 42) return true; // means failure
                    return false;
                };

            theInt = 42;

            var shrinkStrat =
                new SimpleValuesShrinkingStrategy<ShrinkingAnIntTests>(
                    this,
                    e => e.theInt,
                    (e, i) => e.theInt = (int)i);
            shrinkStrat.AddValues(new object[] { -1, 0, 1 });
            shrinkStrat.Shrink(runFunc);

            Assert.False(shrinkStrat.Shrunk());
        }
    }

    
}
