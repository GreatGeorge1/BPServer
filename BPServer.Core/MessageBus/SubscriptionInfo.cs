using BPServer.Core.MessageBus.Handlers.Address;
using System;
using System.Collections.Generic;
using System.Text;

namespace BPServer.Core.MessageBus
{

    public class SubscriptionInfo
    {
        public IAddress Address { get; }
        public Type CommandType { get; }
        public Type MessageType { get; }
        public Type HandlerType { get; }

        private SubscriptionInfo(IAddress address, Type commandType, Type messageType, Type handlerType)
        {
            Address = address ?? throw new ArgumentNullException(nameof(address));
            CommandType = commandType ?? throw new ArgumentNullException(nameof(commandType));
            MessageType = messageType ?? throw new ArgumentNullException(nameof(messageType));
            HandlerType = handlerType ?? throw new ArgumentNullException(nameof(handlerType));
        }

        public static SubscriptionInfo Typed(IAddress address, Type commandType, Type messageType, Type handlerType)
        {
            return new SubscriptionInfo(address, commandType, messageType, handlerType);
        }
    }
}
