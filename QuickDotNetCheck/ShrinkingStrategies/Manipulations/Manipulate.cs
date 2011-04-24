namespace QuickDotNetCheck.ShrinkingStrategies.Manipulations
{
    public static class Manipulate
    {
        public static Manipulator<TEntity> This<TEntity>(TEntity target)
        {
            return new Manipulator<TEntity>(target);
        }
    }
}