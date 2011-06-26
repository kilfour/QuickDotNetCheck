using QuickDotNetCheck.ShrinkingStrategies;
using QuickGenerate;
using Xunit;

namespace QuickDotNetCheckTests.ShrinkingTests
{
    public class ShrinkingAnObjectTestsRemovingBookkeepingAllValuesTests
    {
        [Fact]
        public void Shrunk()
        {
            var something = Generate.One<SomethingToShrink>();
            something.IntProperty = 42;
            var composite =
                new ShrinkingStrategy()
                    .Add(Simple.AllValues())
                    .Register(something, e => e.IntProperty)
                    .Register(something, e => e.StringProperty);

            composite.Shrink(() => something.IntProperty == 42);

            Assert.False(composite.Shrunk());
            Assert.False(composite.Shrunk(something, e => e.IntProperty));
            Assert.True(composite.Shrunk(something, e => e.StringProperty));
        }

        public class SomethingToShrink
        {
            public int IntProperty { get; set; }
            public string StringProperty { get; set; }
        }
    }
}