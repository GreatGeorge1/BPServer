namespace BPServer.Core.Messages
{
    using System.Collections.Generic;
    public interface IMessage
    {
        byte[] Raw { get; }
        byte BodyXor { get; }
        ICollection<byte> Body { get; }
        byte Route { get; }
        MessageType Type { get; }
    }
}