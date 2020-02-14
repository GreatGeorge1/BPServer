using BPServer.Core.MessageBus.Messages;
using System;
using System.Text;
using System.Threading.Tasks;

namespace BPServer.Core.Transports
{
    public interface ITransport
    {
        string Name { get; }
        Task PushDataAsync(IMessage input);
        event EventHandler<IMessage> DataReceived;
        string GetInfo();
    }
}
