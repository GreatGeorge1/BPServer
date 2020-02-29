using System;

namespace BPServer.Core.Transports
{
    public class TransportAddedEventArgs
    {
        public TransportAddedEventArgs(string transportName, Type type)
        {
            TransportName = transportName ?? throw new ArgumentNullException(nameof(transportName));
            Type = type ?? throw new ArgumentNullException(nameof(type));
        }

        public string TransportName { get; }
        public Type Type { get; } 
    }
}