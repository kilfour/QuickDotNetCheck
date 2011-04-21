using QuickDotNetCheck.NotInTheRoot;

namespace QuickDotNetCheck
{
    public interface ICondition
    {
        bool Evaluate(object fixture);
    }

    public abstract class Condition<TFixture> : ICondition 
        where TFixture : IFixture
    {
        public abstract bool Evaluate(TFixture fixture);
        public bool Evaluate(object fixture)
        {
            return Evaluate((TFixture) fixture);
        }
    }
}