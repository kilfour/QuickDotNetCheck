using System;
using System.Collections.Generic;

namespace QuickDotNetCheck.Examples.BroadCaster.SimpleModel
{
    public class Broadcaster : IBroadcaster
    {
        private readonly object monitor = new object();

        private List<IClientProxy> clients;

        private readonly IClientProxyFactory clientProxyFactory;

        public Broadcaster(IClientProxyFactory clientProxyFactory)
        {
            this.clientProxyFactory = clientProxyFactory;
            clients = new List<IClientProxy>();
        }

        public void Register()
        {
            var client = clientProxyFactory.CreateClientProxyForCurrentContext("anUrl");
            client.Faulted += Client_Faulted;
            AddClientToRegisteredClients(client);
        }

        private void AddClientToRegisteredClients(IClientProxy client)
        {
            lock (monitor)
            {
                // The 'dead' code on the next line is the correct implementation
                //clients = new List<IClientProxy>(clients) { client };

                clients.Add(client);
            }
        }

        public void Broadcast(Notification notification)
        {
            foreach (var client in clients)
            {
                client.SendNotificationAsynchronously(notification);
            }
        }

        private void Client_Faulted(object sender, EventArgs e)
        {
            var client = (IClientProxy)sender;
            client.Faulted -= Client_Faulted;
            RemoveClientFromRegisteredClients(client);
        }

        private void RemoveClientFromRegisteredClients(IClientProxy client)
        {
            lock (monitor)
            {
                var clientList = new List<IClientProxy>(clients);
                clientList.Remove(client);
                clients = clientList;

                // using the code below :
                //clients.Remove(client);
                // instead of the implementation above will result in a bug
            }
        }
    }
}