using System;

namespace BPServer.Core.MessageBus.Handlers.Address
{
    public class Address : IAddress
    {
        public Address(byte command, byte messageType, string transportName)
        {
            if (string.IsNullOrWhiteSpace(transportName))
            {
                throw new ArgumentException("message", nameof(transportName));
            }

            Command = command;
            MessageType = messageType;
            TransportName = transportName;
        }

        public byte Command { get; protected set; }
        public byte MessageType { get; protected set; }

        public string TransportName { get; protected set; }
    }
}
