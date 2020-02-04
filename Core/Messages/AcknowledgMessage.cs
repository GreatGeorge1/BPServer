using BPServer.Core.Exceptions;

namespace BPServer.Core
{
    public class AcknowledgMessage : Message
    {
        public AcknowledgMessage(byte[] message) : base(message)
        {
            if (IsTypeOf(message, MessageType.ACK) == false)
            {
                throw new MessageTypeException($"{nameof(message)} MessageType is not {MessageType.ACK}");
            }
        }

        public AcknowledgMessage(byte Route, byte[] Value) : base(MessageType.ACK, Route, Value)
        {
        }

        public new const MessageType Type = MessageType.ACK;
    }
}
