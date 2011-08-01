namespace QuickDotNetCheck.Examples.BroadCaster.SimpleModel
{
    public interface IClientProxyFactory
    {
        IClientProxy CreateClientProxyForCurrentContext(string s);
    }
}