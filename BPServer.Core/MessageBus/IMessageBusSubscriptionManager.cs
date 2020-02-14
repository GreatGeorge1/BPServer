using BPServer.Core.MessageBus.Handlers;
using BPServer.Core.MessageBus.Handlers.Address;
using System;
using System.Collections.Generic;
using System.Text;

namespace BPServer.Core.MessageBus
{
    public interface IMessageBusSubscriptionManager
    {
        bool IsEmpty { get; }
        event EventHandler<IAddress> AddressRemoved;

        void AddSubscription<T>(string transportName)
            where T : IHandler;
        void RemoveSubscription<T>(string transportName)
            where T : IHandler;

        void Clear();
        IEnumerable<SubscriptionInfo> GetHandlersForAddress(IAddress address);
        Type GetMessageTypeByByte(byte input);
        Type GetCommandTypeByByte(byte input);

        bool HasSubscriptionsForAddress(IAddress address);
        bool HasSubscriptionsForCommand<T>(string transportName) where T : ICommand;

    }
}
