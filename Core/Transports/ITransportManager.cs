using BPServer.Core.Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace BPServer.Core.Transports
{
    public interface ITransportManager
    {
        event EventHandler<MessageReceivedEventArgs> MessageReceived;
        void AddTransport(ITransport transport);
        void RemoveTransport(ITransport transport);
        ITransport GetTransportByName(string name);
    }
}
