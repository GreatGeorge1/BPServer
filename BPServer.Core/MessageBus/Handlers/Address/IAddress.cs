using System.Collections.Generic;
using System.Text;

namespace BPServer.Core.MessageBus.Handlers.Address
{
    public interface IAddress
    {
        IRoute Route { get; }
        string TransportName { get; }
    }
}
