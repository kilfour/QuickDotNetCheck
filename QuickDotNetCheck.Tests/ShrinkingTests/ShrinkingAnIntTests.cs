using System;
using QuickDotNetCheck.ShrinkingStrategies;
using QuickDotNetCheck.ShrinkingStrategies.Manipulations;
using Xunit;

namespace QuickDotNetCheckTests.ShrinkingTests
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

            var shrinkStrat =
                new ShrinkingStrategy()
                    .Add(Manipulate.This(this)
                             .Change(e => e.theInt, -1)
                             .Change(e => e.theInt, 0)
                             .Change(e => e.theInt, 1));

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
                new ShrinkingStrategy()
                    .Add(Manipulate.This(this)
                             .Change(e => e.theInt, -1)
                             .Change(e => e.theInt, 0)
                             .Change(e => e.theInt, 1));

            shrinkStrat.Shrink(runFunc);

            Assert.False(shrinkStrat.Shrunk());
        }
    }
}
