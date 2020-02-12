using BPServer.Core.Handlers;

namespace BPServer.Core.MessageBus
{
    using BPServer.Core.Messages;

    public interface IMessageBus
    {
        void Publish(IMessage @message, string transportName);
        void Subscribe<T>(string transportName)
            where T : IHandler;
        void Unsubscribe<T>(string transportName)
            where T : IHandler;
    }
}
