using System;
using QuickGenerate;
using Rhino.Mocks;
using QuickDotNetCheck.Examples.BroadCaster.SimpleModel;

namespace QuickDotNetCheck.Examples.BroadCaster
{
    public class RegisteredClientFaults : Fixture
    {
        IClientProxy client;

        public override bool CanAct()
        {
            return BroadcasterSuite.GetBroadcastersClients().Count > 0;
        }

        public override void Arrange()
        {
            client = BroadcasterSuite.GetBroadcastersClients().PickOne();
        }

        protected override void Act()
        {
            client.Raise(c => c.Faulted += null, client, EventArgs.Empty);
        }

        public Spec ClientIsRemovedFromCollection()
        {
            return new Spec(() => Ensure.False(BroadcasterSuite.GetBroadcastersClients().Contains(client)));
        }
    }
}
