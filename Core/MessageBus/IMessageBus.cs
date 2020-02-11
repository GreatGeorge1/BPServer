using BPServer.Core.Handlers;

namespace BPServer.Core.MessageBus
{
    using BPServer.Core.Messages;

    public interface IMessageBus
    {
        void Publish(IMessage @message, string serialPort);
        void Subscribe<T>(string serialPort)
            where T : IHandler<IMessage, ICommand>;
        void Unsubscribe<T>(string serialPort)
            where T : IHandler<IMessage, ICommand>;
    }
}
