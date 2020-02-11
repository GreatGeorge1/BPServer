using BPServer.Core.Messages;
using System;

namespace BPServer.Core.Transports
{
    public class MessageReceivedEventArgs
    {
        public MessageReceivedEventArgs(IMessage message, ITransport transport)
        {
            Message = message ?? throw new ArgumentNullException(nameof(message));
            Transport = transport ?? throw new ArgumentNullException(nameof(transport));
        }

        public IMessage Message { get; }
        public ITransport Transport { get; }

    }
}
