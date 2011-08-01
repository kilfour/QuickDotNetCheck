using System.Threading;
using QuickDotNetCheck.Examples.BroadCaster.SimpleModel;
using QuickGenerate.Primitives;
using Rhino.Mocks;

namespace QuickDotNetCheck.Examples.BroadCaster
{
    public class RegisterClient : Fixture
    {
        IClientProxy client;
        int sleepTime;

        public override void Arrange()
        {
            client = MockRepository.GenerateMock<IClientProxy>();
            sleepTime = new IntGenerator(0, 10).GetRandomValue();
        }

        protected override void Act()
        {
            BroadcasterSuite.ClientFactory
                .Stub(f => f.CreateClientProxyForCurrentContext(null))
                .IgnoreArguments()
                .Return(client)
                .Repeat.Once();
            
            client
                .Stub(c => c.SendNotificationAsynchronously(null))
                .IgnoreArguments()
                .WhenCalled(obj => Thread.Sleep(sleepTime));

            BroadcasterSuite.Broadcaster.Register();
        }

        public Spec ClientExistsInCollection()
        {
            return new Spec(
                () => Ensure.True(BroadcasterSuite.GetBroadcastersClients().Contains(client)));
        }
    }
}