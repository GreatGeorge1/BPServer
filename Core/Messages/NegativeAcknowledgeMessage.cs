namespace BPServer.Core.Messages
{
    using BPServer.Core.Attributes;
    using BPServer.Core.Exceptions;

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

        public NegativeAcknowledgeMessage(byte Route, byte[] Value) : base(MessageType.NACK, Route, Value)
        {
        }

        public new const MessageType Type = MessageType.NACK;
    }
}
