using System;

namespace QuickDotNetCheck.Examples.BroadCaster.SimpleModel
{
    public interface IClientProxy
    {
        event EventHandler Faulted;
        void SendNotificationAsynchronously(Notification notification);
    }
}