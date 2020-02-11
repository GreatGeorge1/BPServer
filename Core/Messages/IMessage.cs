namespace BPServer.Core.Messages
{
    using System;
    using System.Collections.Generic;
    public interface IMessage
    {
        Guid Id { get; }
        DateTime CreationDate { get; }
        
        byte[] Raw { get; }
        byte BodyXor { get; }
        ICollection<byte> Body { get; }
        byte Command { get; }
        MessageType Type { get; }
    }
}