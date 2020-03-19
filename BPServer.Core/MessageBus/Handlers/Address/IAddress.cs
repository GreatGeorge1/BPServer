using System;
using System.Collections.Generic;
using System.Text;

namespace BPServer.Core.MessageBus.Handlers.Address
{
    public interface IAddress
    {
        byte Command { get; }
        byte MessageType { get; }
        string TransportName { get; }
    }
}
