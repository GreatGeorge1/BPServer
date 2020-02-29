using BPServer.Core.MessageBus.Attributes;
using BPServer.Core.MessageBus.Handlers;
using System;
using System.Collections.Generic;
using System.Text;

namespace BPServer.Core.MessageBus.Command
{
    [CommandByte(0xD4)]
    public class PingCommand : ICommand
    {
        public byte Command => 0xD4;
    }
}
