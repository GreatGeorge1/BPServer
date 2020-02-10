namespace BPServer.Core.Handlers
{
    using BPServer.Core.Messages;
    using System;
    using System.Collections.Generic;
    using System.Text;


    public interface IHandlerProcessManager
    {

    }

    public interface IMessageBusSubscriptionManager
    {
        bool IsEmpty { get; }

        Dictionary<string,List<IHandler>> OnRun { get; }
        Queue<IMessage> messages { get; }

        event EventHandler<IAddress> AddressRemoved;

        void OnReady(IHandler<IMessage> handler);
        void OnWaiting(IHandler<IMessage> handler);
        void OnCompleted(IHandler<IMessage> handler);

        void AddSubscription<T, TH>(string serialPort,SubscriptionType subscriptionType)
            where T : IMessage
            where TH : IHandler<IMessage>;

        void AddDynamicSubscription<T, TH>(string serialPort, SubscriptionType subscriptionType)
            where T : IMessage
            where TH : IHandler<IMessage>;

        void Clear();
        IEnumerable<SubscriptionInfo> GetHandlersForAddress(IAddress address);
        string GetMessageKey<T>();
        Type GetMessageTypeByAddress(IAddress address);
        bool HasSubscriptionsForAddress(IAddress address);
        bool HasSubscriptionsForMessage<T>(string serialPort) where T : IMessage;
        void RemoveSubscription<T, TH>(string serialPort)
            where T : IMessage
            where TH : IHandler<IMessage>;
    }
}
