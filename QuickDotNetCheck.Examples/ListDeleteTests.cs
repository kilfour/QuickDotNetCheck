using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QuickDotNetCheck.Implementation;
using QuickDotNetCheck.ShrinkingStrategies;
using QuickGenerate;
using QuickGenerate.Primitives;
using Xunit;

namespace QuickDotNetCheck.Examples
{
    public class ListDeleter
    {
        public IList<int> DoingMyThing(IList<int> theList, int iNeedToBeRemoved)
        {
            var result = theList.ToList();
            result.Remove(iNeedToBeRemoved);
            return result;
        }            
    }

    public class UnitTesting
    {
        [Fact]
        public void Result_Does_Not_Contain_Removed_Value()
        {
            var inputList = new List<int> {1, 2, 3, 4, 5};
            var resultList = new ListDeleter().DoingMyThing(inputList, 4);
            Assert.DoesNotContain(4, resultList);
        }
    }

    public class PropertyBasedTesting : Fixture
    {
        private readonly ListDeleter listDeleter = new ListDeleter();
        private readonly IntGenerator intGen = new IntGenerator(1, 50);

        private IList<int> inputList { get; set; }
        private IList<int> resultList;
        private int toRemove;

        public override void Arrange()
        {
            toRemove = intGen.GetRandomValue();
            inputList = new DomainGenerator().With(() => intGen).Many<int>(1, 10).ToList();
        }

        protected override void Act()
        {
            resultList = listDeleter.DoingMyThing(inputList, toRemove);
        }

        [Spec]
        public void Result_Does_Not_Contain_Removed_Value()
        {
            Ensure.False(resultList.Contains(toRemove));
        }

        [Fact]
        public void VerifyAll()
        {
            new Suite(100, 20)
                .Register(() => new PropertyBasedTesting())
                .Run();
        }

        public override void Shrink(Func<bool> runFunc)
        {
            new ListShrinkingStrategy<PropertyBasedTesting, int>(
                this, e => e.inputList, new[] { -1, 0, 1 })
                    .Shrink(runFunc);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine(GetType().Name);

            sb.Append("inputList : ");
            inputList.ForEach(i => sb.AppendFormat("{0}, ", i));
            sb.Remove(sb.Length - 2, 2);
            sb.Append(".");
            sb.AppendLine();

            sb.AppendFormat("toRemove : {0}.", toRemove);

            return sb.ToString();
        }
    }
}
