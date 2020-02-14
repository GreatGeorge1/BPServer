using System;
using System.Collections.Generic;

namespace BPServer.Core.MessageBus.Messages
{
    public interface IMessage
    {
        Guid Id { get; }
        DateTime CreationDate { get; }

        byte[] Raw { get; }
        byte BodyXor { get; }
        ICollection<byte> Body { get; }
        byte Command { get; }
        byte Type { get; }
    }
}