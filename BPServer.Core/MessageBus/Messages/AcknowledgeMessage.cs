using BPServer.Core.Exceptions;
using BPServer.Core.MessageBus.Attributes;

namespace BPServer.Core.MessageBus.Messages
{
    [MessageType((byte)MessageType.ACK)]
    public class AcknowledgeMessage : Message
    {
        public AcknowledgeMessage(byte[] message) : base(message)
        {
            if (IsTypeOf(message, MessageType.ACK) == false)
            {
                throw new MessageTypeException($"{nameof(message)} MessageType is not {MessageType.ACK}");
            }
        }

        public AcknowledgeMessage(byte Route, byte[] Value) : base((byte)MessageType.ACK, Route, Value)
        {
        }
    }
}
