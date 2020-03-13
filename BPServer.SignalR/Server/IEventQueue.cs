using System;
using System.Collections.Generic;
using System.Text;

namespace BPServer.SignalR.Server
{
    public interface IEventQueue
    {
        void Publish(IMessage @event);
        void Send(IMessage @event, string address);
    }
}
