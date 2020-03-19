using BPServer.Core.MessageBus;
using BPServer.Core.MessageBus.Messages;
using System;
using System.Text;
using System.Threading.Tasks;

namespace BPServer.Core.Transports
{
    public interface ITransport : IDisposable
    {
        void SetMessageBus(IMessageBus messageBus);
        string Name { get; }
        Task PushDataAsync(IMessage input);
        string GetInfo();
    }
}
