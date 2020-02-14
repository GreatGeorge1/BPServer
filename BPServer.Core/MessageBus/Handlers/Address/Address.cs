using System;

namespace BPServer.Core.MessageBus.Handlers.Address
{
    public class Address : IAddress
    {
        public Address(IRoute route, string transportName)
        {
            if (string.IsNullOrWhiteSpace(transportName))
            {
                throw new ArgumentException("message", nameof(transportName));
            }

            Route = route ?? throw new ArgumentNullException(nameof(route));
            TransportName = transportName;
        }

        public IRoute Route { get; protected set; }

        public string TransportName { get; protected set; }
    }
}
