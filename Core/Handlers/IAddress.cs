using System;
using System.Collections.Generic;
using System.Text;

namespace BPServer.Core.Handlers
{
    public interface IAddress
    {
        IRoute Route { get; }
        string SerialPort { get; }
    }

    public interface IRoute
    {
        byte Command { get; }
        byte MessageType { get; }
    }
}
