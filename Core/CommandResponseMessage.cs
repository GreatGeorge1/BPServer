using BPServer.Core.Exceptions;

namespace BPServer.Core
{
    public class CommandResponseMessage : Message
    {
        public CommandResponseMessage(byte[] message) : base(message)
        {
            if (IsTypeOf(message, MessageType.CommandResponse) == false)
            {
                throw new MessageTypeException($"{nameof(message)} MessageType is not {MessageType.CommandResponse}");
            }
        }

        public CommandResponseMessage(byte Route, byte[] Value) : base(MessageType.CommandResponse, Route, Value)
        {
        }

        public new const MessageType Type = MessageType.CommandResponse;
    }
}
