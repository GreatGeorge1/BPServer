using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPServer.Core.Transports
{
    public interface ITransportManager 
    {
        event EventHandler<TransportAddedEventArgs> TransportAdded;
        void AddTransport(ITransport transport);
        void RemoveTransport(ITransport transport);
        ITransport GetTransportByName(string name);
        IEnumerable<ITransport> GetTransports();
        void Clear();
    }
}
