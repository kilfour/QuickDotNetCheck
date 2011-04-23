using System;
using System.Collections.Generic;
using System.Linq;
using QuickDotNetCheck.ShrinkingStrategies;
using Xunit;

namespace QuickDotNetCheckTests.ShrinkingTests
{
    public class ShrinkingAListTests
    {
        private IList<int> theList { get; set; }

        [Fact]
        public void SimpleShrinking()
        {
            Func<bool> runFunc =
                () =>
                    {
                        if (theList.Contains(4)) return true;
                        return false;
                    };
            theList = new List<int> {1, 2, 3, 4, 5};

            var shrinkStrat =
                new ListShrinkingStrategy<ShrinkingAListTests, int>(
                    this,
                    e => e.theList, 
                    new[]{- 1, 0, 1});
            
            shrinkStrat.Shrink(runFunc);

            Assert.Equal(1, shrinkStrat.Result.Count());
            Assert.Equal(4, shrinkStrat.Result.ElementAt(0));
        }

        [Fact]
        public void Trickier()
        {
            Func<bool> runFunc =
                () =>
                {
                    if (theList.Count(i => i==1) >= 2) return true; // a list with two or more one's fails
                    return false;
                };
            theList = new List<int> { 1, 1, 1};

            var shrinkStrat =
                new ListShrinkingStrategy<ShrinkingAListTests, int>(
                    this,
                    e => e.theList,
                    new[] { -1, 0, 1 });

            shrinkStrat.Shrink(runFunc);

            Assert.Equal(2, shrinkStrat.Result.Count());
            Assert.Equal(1, shrinkStrat.Result.ElementAt(0));
            Assert.Equal(1, shrinkStrat.Result.ElementAt(1));
        }
    }
}
