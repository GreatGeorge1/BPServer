using System;
using System.Collections.Generic;
using System.Text;

namespace BPServer.Core.Handlers
{

        public class SubscriptionInfo
        {
            public IAddress Address { get; }
            public Type HandlerType { get; }

            private SubscriptionInfo(Type handlerType, IAddress address)
            {
                HandlerType = handlerType;
                Address = address;
            }

            public static SubscriptionInfo ReadOnly(Type handlerType, IAddress address)
            {
                return new SubscriptionInfo(handlerType, address);
            }

            public static SubscriptionInfo ReadWrite(Type handlerType, IAddress address)
            {
                return new SubscriptionInfo(handlerType, address);
            }
        }    
}
