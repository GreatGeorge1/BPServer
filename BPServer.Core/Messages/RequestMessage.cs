namespace BPServer.Core.Messages
{
    using System;
    using BPServer.Core.Attributes;
    using BPServer.Core.Exceptions;

    [MessageType((byte)MessageType.Request)]
    public class RequestMessage : Message
    {
        public RequestMessage(byte[] message) : base(message)
        {
            if (message is null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            if (IsTypeOf(message, MessageType.Request) == false)
            {
                throw new MessageTypeException($"{nameof(message)} MessageType is not {MessageType.Request}");
            }
        }

        public RequestMessage(byte Route, byte[] Value) : base((byte)MessageType.Request, Route, Value)
        {
        }

        public new const MessageType Type = MessageType.Request;
    }
}
