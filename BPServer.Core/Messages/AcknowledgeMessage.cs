namespace BPServer.Core.Messages
{
    using BPServer.Core.Attributes;
    using BPServer.Core.Exceptions;

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
