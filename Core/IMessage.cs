using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace BPServer.Core
{
    public interface IMessage
    {
        byte[] Raw { get; }
        byte BodyXor { get; }
        ICollection<byte> Body { get; }
        byte Route { get; }
        MessageType Type { get; }
    }
}