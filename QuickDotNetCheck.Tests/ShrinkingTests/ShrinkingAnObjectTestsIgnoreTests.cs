using System;
using QuickDotNetCheck.ShrinkingStrategies;
using QuickGenerate;
using Xunit;

namespace QuickDotNetCheckTests.ShrinkingTests
{
    public class ShrinkingAnObjectTestsIgnoreTests
    {
        [Fact]
        public void Shrunk()
        {
            var something = Generate.One<SomethingToShrink>();
            something.IntProperty = 42;
            var composite =
                new ShrinkingStrategy()
                    .Add(-1, 0, 1)
                    .Ignore<SomethingToShrink, int>(e => e.IntProperty)
                    .RegisterAll(something);

            Assert.Throws<NullReferenceException>(() => composite.Shrunk(something, e => e.IntProperty));
        }

        public class SomethingToShrink
        {
            public int IntProperty { get; set; }
        }
    }
}