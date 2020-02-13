namespace BPServer.Core.Messages
{
    using BPServer.Core.Attributes;
    using BPServer.Core.Exceptions;

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
