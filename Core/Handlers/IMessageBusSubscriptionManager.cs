namespace BPServer.Core.Handlers
{
    using BPServer.Core.Messages;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using static BPServer.Core.Handlers.InMemoryMessageBusSubscriptionsManager;

    public interface IMessageBusSubscriptionManager
    {
        bool IsEmpty { get; }

        event EventHandler<(byte, byte)> OnRouteMessageRemoved;

        void AddSubscription<T, TH>()
            where T : IMessage
            where TH : IHandler<IMessage>;
        void Clear();
        IEnumerable<SubscriptionInfo> GetHandlersForRoute(byte route, byte messageType);
        string GetMessageKey<T>();
        Type GetMessageTypeByByte(byte route, byte messageType);
        bool HasSubscriptionsForMessage(byte route, byte messageType);
        bool HasSubscriptionsForMessage<T>(byte route) where T : IMessage;
        void RemoveSubscription<T, TH>()
            where T : IMessage
            where TH : IHandler<IMessage>;
    }


    public static class GenericTypeExtensions
    {
        public static string GetGenericTypeName(this Type type)
        {
            var typeName = string.Empty;

            if (type.IsGenericType)
            {
                var genericTypes = string.Join(",", type.GetGenericArguments().Select(t => t.Name).ToArray());
                typeName = $"{type.Name.Remove(type.Name.IndexOf('`'))}<{genericTypes}>";
            }
            else
            {
                typeName = type.Name;
            }

            return typeName;
        }

        public static string GetGenericTypeName(this object @object)
        {
            return @object.GetType().GetGenericTypeName();
        }
    }
}
