using System;
using System.Collections.Generic;
using System.Text;

namespace BPServer.Core.Handlers
{
    public partial class InMemoryMessageBusSubscriptionsManager : IMessageBusSubscriptionManager
    {
        public class SubscriptionInfo
        {
            public byte Route { get; }
            public Type HandlerType { get; }

            private SubscriptionInfo(Type handlerType, byte route)
            {
                HandlerType = handlerType;
                Route = route;
            }

            public static SubscriptionInfo Typed(Type handlerType, byte route)
            {
                return new SubscriptionInfo(handlerType, route);
            }
        }
    }
}
