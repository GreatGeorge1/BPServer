using System;
using BPServer.Core.Exceptions;

namespace BPServer.Core
{
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

        public RequestResponseMessage(byte Route, byte[] Value) : base(MessageType.RequestResponse, Route, Value)
        {
            
        }
    }
}
