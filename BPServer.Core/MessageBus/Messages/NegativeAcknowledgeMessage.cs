using BPServer.Core.Exceptions;
using BPServer.Core.MessageBus.Attributes;

namespace BPServer.Core.MessageBus.Messages
{
    [MessageType((byte)MessageType.NACK)]
    public class NegativeAcknowledgeMessage : Message
    {
        public NegativeAcknowledgeMessage(byte[] message) : base(message)
        {
            if (IsTypeOf(message, MessageType.NACK) == false)
            {
                throw new MessageTypeException($"{nameof(message)} MessageType is not {MessageType.NACK}");
            }
        }

        public NegativeAcknowledgeMessage(byte Route, byte[] Value) : base((byte)MessageType.NACK, Route, Value)
        {
        }

        public new const MessageType Type = MessageType.NACK;
    }
}
