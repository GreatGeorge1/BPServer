using System;

namespace BPServer.Core.Transports
{
    public class TransportSubscription
    {
        public TransportSubscription(ITransport transport)
        {
            Transport = transport ?? throw new ArgumentNullException(nameof(transport));
        }

        public ITransport Transport { get; private set; }
    }
}
