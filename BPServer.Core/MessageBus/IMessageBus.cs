namespace BPServer.Core.MessageBus
{
    using BPServer.Core.MessageBus.Handlers;
    using BPServer.Core.MessageBus.Messages;
    using BPServer.Core.Transports;
    using System;
    using System.Threading.Tasks;

    public interface IMessageBus
    {
        Task Publish(IMessage message, string exchange);
        void Subscribe<T>(string transportName)
            where T : IHandler;
        void Unsubscribe<T>(string transportName)
            where T : IHandler;
        void AddTransport(ITransport transport);
        void RemoveTransport(ITransport transport);
        event EventHandler<TransportAddedEventArgs> TransportAdded;
    }
}
