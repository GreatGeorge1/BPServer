using System;
using System.Collections.Generic;
using System.Text;

namespace BPServer.SignalR
{
    public interface IIntergrationEvent
    {
        Guid Id { get; }
        DateTime CreationTime { get; }
        string SenderId { get; }
        string Type { get; }
    }
}
