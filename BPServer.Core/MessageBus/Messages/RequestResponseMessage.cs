using System;
using BPServer.Core.Exceptions;
using BPServer.Core.MessageBus.Attributes;

namespace BPServer.Core.MessageBus.Messages
{
    [MessageType((byte)MessageType.RequestResponse)]
    public class RequestResponseMessage : Message
    {
        public RequestResponseMessage(byte[] message) : base(message)
        {
            if (message is null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            if (IsTypeOf(message, MessageType.RequestResponse) == false)
            {
                throw new MessageTypeException($"{nameof(message)} MessageType is not {MessageType.RequestResponse}");
            }
        }

        public RequestResponseMessage(byte Route, byte[] Value) : base((byte)MessageType.RequestResponse, Route, Value)
        {

        }
    }
}
