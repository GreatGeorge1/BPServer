namespace BPServer.Core.Messages
{
    using System;
    using BPServer.Core.Attributes;
    using BPServer.Core.Exceptions;

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
