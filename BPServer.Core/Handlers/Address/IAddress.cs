using System.Collections.Generic;
using System.Text;

namespace BPServer.Core.Handlers
{
    public interface IAddress
    {
        IRoute Route { get; }
        string TransportName { get; }
    }
}
