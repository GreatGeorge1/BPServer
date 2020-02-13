using BPServer.Core.Handlers;

namespace BPServer.Core.MessageBus
{
    using BPServer.Core.Messages;
    using System.Threading.Tasks;

    public interface IMessageBus
    {
        Task Publish(IMessage @message, string transportName);
        void Subscribe<T>(string transportName)
            where T : IHandler;
        void Unsubscribe<T>(string transportName)
            where T : IHandler;
    }
}
