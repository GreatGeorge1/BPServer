namespace BPServer.Core.Handlers
{
    using BPServer.Core.Attributes;
    using BPServer.Core.Messages;
    using System;
    using System.Collections.Generic;
    using System.Linq;


    /// <summary>
    /// TODO один адрес (byte,byte) => один подписчик. пока так
    /// </summary>
    public partial class InMemoryMessageBusSubscriptionsManager : IMessageBusSubscriptionManager
    {
        /// <summary>
        /// (route,messageType)
        /// </summary>
        private readonly Dictionary<(byte, byte), List<SubscriptionInfo>> _handlers;
        /// <summary>
        /// byte is MessageType
        /// </summary>
        private readonly Dictionary<(byte, byte), Type> _messageTypes;

        /// <summary>
        /// TODO ивент
        /// </summary>
        public event EventHandler<(byte, byte)> OnRouteMessageRemoved;

        public InMemoryMessageBusSubscriptionsManager()
        {
            _handlers = new Dictionary<(byte, byte), List<SubscriptionInfo>>();
            _messageTypes = new Dictionary<(byte, byte), Type>();
        }

        //private void InitMessageTypes()
        //{
        //    _messageTypes.Add((byte)MessageType.Notification, typeof(NotificationMessage));
        //    _messageTypes.Add((byte)MessageType.ACK, typeof(AcknowledgeMessage));
        //    _messageTypes.Add((byte)MessageType.NACK, typeof(NegativeAcknowledgeMessage));
        //    _messageTypes.Add((byte)MessageType.Request, typeof(RequestMessage));
        //    _messageTypes.Add((byte)MessageType.RequestResponse, typeof(RequestResponseMessage));
        //    _messageTypes.Add((byte)MessageType.Command, typeof(CommandMessage));
        //    _messageTypes.Add((byte)MessageType.CommandResponse, typeof(CommandResponseMessage));
        //}

        public bool IsEmpty => !_handlers.Keys.Any();
        public void Clear() => _handlers.Clear();

        public void AddSubscription<T, TH>()
            where T : IMessage
            where TH : IHandler<IMessage>
        {
            var routeAttribute = typeof(TH).GetCustomAttributes(typeof(RouteAttribute), true).FirstOrDefault() as RouteAttribute;
            var messageTypeAttribute = typeof(T).GetCustomAttributes(typeof(MessageTypeAttribute), true).FirstOrDefault() as MessageTypeAttribute;
            if (messageTypeAttribute is null)
            {
                throw new ArgumentException(
                  $"Message Type {typeof(T).Name} has no MessageTypeAttribute", nameof(messageTypeAttribute));
            }
            if (!(routeAttribute is null))
            {
                DoAddSubscription(typeof(TH), routeAttribute.Route, messageTypeAttribute.MessageType);
            }
            else
            {
                throw new ArgumentException(
                  $"Handler Type {typeof(T).Name} has no RouteAttribute", nameof(routeAttribute));
            }

            if (!_messageTypes.ContainsKey((routeAttribute.Route, messageTypeAttribute.MessageType)))
            {
                _messageTypes.Add((routeAttribute.Route, messageTypeAttribute.MessageType), typeof(T));
            }

        }

        private void DoAddSubscription(Type handlerType, byte route, byte messageType)
        {
            if (!HasSubscriptionsForMessage(route, messageType))
            {
                _handlers.Add((route, messageType), new List<SubscriptionInfo>());
            }
            if (_handlers[(route, messageType)].Any(s => s.HandlerType == handlerType))
            {
                throw new ArgumentException(
                   $"Handler Type {handlerType.Name} already registered for '{BitConverter.ToString(new byte[] { route, messageType })}'", nameof(handlerType));
            }

            _handlers[(route, messageType)].Add(SubscriptionInfo.Typed(handlerType, route));
        }

        public IEnumerable<SubscriptionInfo> GetHandlersForRoute(byte route, byte messageType)
        {
            _handlers.TryGetValue((route, messageType), out List<SubscriptionInfo> value);
            return value;
        }

        public string GetMessageKey<T>()
        {
            return typeof(T).Name;
        }

        public Type GetMessageTypeByByte(byte route, byte messageType)
        {
            _messageTypes.TryGetValue((route, messageType), out Type value);
            return value;
        }

        public bool HasSubscriptionsForMessage<T>(byte route) where T : IMessage
        {
            var messageTypeAttribute = typeof(T).GetCustomAttributes(typeof(MessageTypeAttribute), true).FirstOrDefault() as MessageTypeAttribute;
            if (!(messageTypeAttribute is null))
            {
                _messageTypes.TryGetValue((route, messageTypeAttribute.MessageType), out Type type);
                if (type == typeof(T))
                {
                    return true;
                }
            }
            else
            {
                throw new ArgumentException(
                   $"Message Type {typeof(T).Name} has no MessageTypeAttribute", nameof(messageTypeAttribute));
            }
            return false;
        }

        public bool HasSubscriptionsForMessage(byte route, byte messageType)
        {
            if (_messageTypes.ContainsKey((route, messageType)))
            {
                return true;
            }
            return false;
        }

        public void RemoveSubscription<T, TH>()
            where T : IMessage
            where TH : IHandler<IMessage>
        {
            var routeAttribute = typeof(TH).GetCustomAttributes(typeof(RouteAttribute), true).FirstOrDefault() as RouteAttribute;
            var messageTypeAttribute = typeof(T).GetCustomAttributes(typeof(MessageTypeAttribute), true).FirstOrDefault() as MessageTypeAttribute;
            if (messageTypeAttribute is null)
            {
                throw new ArgumentException(
                  $"Message Type {typeof(T).Name} has no MessageTypeAttribute", nameof(messageTypeAttribute));
            }
            if (routeAttribute is null)
            {
                throw new ArgumentException(
                  $"Handler Type {typeof(T).Name} has no RouteAttribute", nameof(routeAttribute));
            }
            _handlers.TryGetValue((routeAttribute.Route, messageTypeAttribute.MessageType), out List<SubscriptionInfo> value);
            if (!(value is null))
            {
                var handlerToRemove = value.Where(s => s.HandlerType == typeof(TH)).FirstOrDefault();
                if (!(handlerToRemove is null))
                {
                    _handlers[(routeAttribute.Route, messageTypeAttribute.MessageType)].Remove(handlerToRemove);
                    _messageTypes.Remove((routeAttribute.Route, messageTypeAttribute.MessageType));
                }
            }
        }
    }
}
