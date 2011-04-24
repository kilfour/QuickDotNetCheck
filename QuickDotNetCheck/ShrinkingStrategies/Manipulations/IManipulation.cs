namespace QuickDotNetCheck.ShrinkingStrategies.Manipulations
{
    public interface IManipulation
    {
        void Manipulate();
        void Reset();
        string Report();
        string[] Keys();
    }
}