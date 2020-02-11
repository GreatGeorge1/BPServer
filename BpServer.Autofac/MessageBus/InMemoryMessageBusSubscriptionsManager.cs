using BPServer.Core.Handlers;

namespace BPServer.Core.MessageBus
{
    using BPServer.Core.Attributes;
    using BPServer.Core.Messages;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class InMemoryMessageBusSubscriptionsManager : IMessageBusSubscriptionManager
    {
        private readonly Dictionary<IAddress, List<IHandler>> _handlers;

        public bool IsEmpty => !_handlers.Any();

        public event EventHandler<IAddress> AddressRemoved;

        protected virtual void OnAddressRemoved(IAddress e)
        {
            EventHandler<IAddress> handler = AddressRemoved;
            handler?.Invoke(this, e);
        }

        public void AddSubscription<T>(string serialPort) where T : IHandler<IMessage, ICommand>
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public Type GetCommandTypeByByte(byte input)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<SubscriptionInfo> GetHandlersForAddress(IAddress address)
        {
            throw new NotImplementedException();
        }

        public Type GetMessageTypeByByte(byte input)
        {
            throw new NotImplementedException();
        }

        public bool HasSubscriptionsForAddress(IAddress address)
        {
            throw new NotImplementedException();
        }

        public bool HasSubscriptionsForCommand<T>(string serialPort) where T : ICommand
        {
            throw new NotImplementedException();
        }

        public void RemoveSubscription<T>(string serialPort) where T : IHandler<IMessage, ICommand>
        {
            //OnAddressRemoved(address);
            throw new NotImplementedException();
        }
    }
}
