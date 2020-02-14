using BPServer.Core.Exceptions;
using BPServer.Core.MessageBus.Attributes;

namespace BPServer.Core.MessageBus.Messages
{
    [MessageType((byte)MessageType.CommandResponse)]
    public class CommandResponseMessage : Message
    {
        public CommandResponseMessage(byte[] message) : base(message)
        {
            if (IsTypeOf(message, MessageType.CommandResponse) == false)
            {
                throw new MessageTypeException($"{nameof(message)} MessageType is not {MessageType.CommandResponse}");
            }
        }

        public CommandResponseMessage(byte Route, byte[] Value) : base((byte)MessageType.CommandResponse, Route, Value)
        {
        }
    }
}
