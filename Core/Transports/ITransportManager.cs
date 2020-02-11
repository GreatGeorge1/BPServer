using System;
using System.Linq;
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
