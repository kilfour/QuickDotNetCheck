using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using QuickDotNetCheck.Examples.BroadCaster.SimpleModel;
using Rhino.Mocks;
using Xunit;

namespace QuickDotNetCheck.Examples.BroadCaster
{
    public class BroadcasterSuite
    {
        public static Broadcaster Broadcaster;
        public static IClientProxyFactory ClientFactory;
        
        public static bool ThreadSwitch;
        public static Thread Thread;

        public BroadcasterSuite()
        {
            ThreadSwitch = false;
            ClientFactory = MockRepository.GenerateMock<IClientProxyFactory>();
            Broadcaster = new Broadcaster(ClientFactory);
        }

        [Fact]
        public void Verify()
        {
            new Suite(1)
                .Do(40,
                    opt => opt.Register<RegisterClient>(),
                    opt => opt.Register<RegisteredClientFaults>(),
                    opt => opt.Register<Broadcast>(),
                    opt => opt.Register<StopBroadcasting>())
                .Run();
        }

        public static List<IClientProxy> GetBroadcastersClients()
        {
            var clientsFieldInfo =
                typeof(Broadcaster).GetField("clients", BindingFlags.NonPublic | BindingFlags.Instance);
            return (List<IClientProxy>)clientsFieldInfo.GetValue(Broadcaster);
        }
    }
}