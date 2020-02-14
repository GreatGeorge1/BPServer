using System;
using BPServer.Core.Exceptions;
using BPServer.Core.MessageBus.Attributes;

namespace BPServer.Core.MessageBus.Messages
{
    [MessageType((byte)MessageType.Command)]
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

        public CommandMessage(byte Route, byte[] Value) : base((byte)MessageType.Command, Route, Value)
        {
        }
    }
}
