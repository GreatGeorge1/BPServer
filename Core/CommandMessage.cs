using System;
using BPServer.Core.Exceptions;

namespace BPServer.Core
{
    public class CommandMessage : Message
    {
        public CommandMessage(byte[] message) : base(message)
        {
            if (message is null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            if (IsTypeOf(message, MessageType.Command) == false)
            {
                throw new MessageTypeException($"{nameof(message)} MessageType is not {MessageType.Command}");
            }
        }

        public CommandMessage(byte Route, byte[] Value) : base(MessageType.Command, Route, Value)
        {
        }

        public new const MessageType Type = MessageType.Command;
    }
}
