using QuickDotNetCheck.ShrinkingStrategies;

namespace QuickDotNetCheck
{
    public static class ShrinkingStrategy
    {
        public static CompositeShrinkingStrategy<TEntity> For<TEntity>(TEntity entity)
        {
            return new CompositeShrinkingStrategy<TEntity>(entity);
        }
    }

}