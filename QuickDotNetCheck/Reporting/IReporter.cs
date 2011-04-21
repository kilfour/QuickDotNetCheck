namespace QuickDotNetCheck.Reporting
{
    public interface IReporter
    {
        void WriteLine(string message);
        void Write(string message);
    }
}