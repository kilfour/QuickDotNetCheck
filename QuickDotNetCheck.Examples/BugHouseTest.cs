using System;
using System.Text;
using QuickDotNetCheck.ShrinkingStrategies;
using QuickDotNetCheck.ShrinkingStrategies.Manipulations;
using QuickGenerate.Primitives;
using Xunit;

namespace QuickDotNetCheck.Examples
{
    public class BugHouse
    {
        private int count;
        public bool Run(int a)
        {
            if (count++ >= 3 && a == 6)
                throw new Exception();
            return true;
        }
    }

    public class BugHouseTest
    {
        [Fact]
        public void VerifyAll()
        {
            new Suite(50)
                .Using(() => new BugHouse())
                .Do(20, opt => opt.Register<BugHouseFixture>())
                .Run();
        }
    }

    public class BugHouseFixture : Fixture, IUse<BugHouse>
    {
        private int input { get; set; }
        private bool output;

        private BugHouse bugHouse;

        public void Set(BugHouse state)
        {
            bugHouse = state;
        }

        public override void Arrange()
        {
            input = new IntGenerator(0, 20).GetRandomValue();
        }

        protected override void Act()
        {
            output = bugHouse.Run(input);
        }

        private ShrinkingStrategy shrunk;

        public override void Shrink(Func<bool> runFunc)
        {
            shrunk =
                new ShrinkingStrategy()
                    .Add(Manipulate.This(this)
                             .Change(e => e.input, -1)
                             .Change(e => e.input, 0)
                             .Change(e => e.input, 1));
            shrunk.Shrink(runFunc);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append(GetType().Name);
            if(!shrunk.Shrunk())
                sb.AppendFormat(", input : {0}.", input);
            return sb.ToString();
        }

        [Spec]
        public void Always_Returns_True()
        {
            Ensure.True(output);
        }
    }
}