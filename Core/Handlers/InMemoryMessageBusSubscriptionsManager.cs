namespace BPServer.Core.Handlers
{
    using BPServer.Core.Attributes;
    using BPServer.Core.Messages;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public partial class InMemoryMessageBusSubscriptionsManager : IMessageBusSubscriptionManager
    {
        public bool IsEmpty => throw new NotImplementedException();

        public Dictionary<string, List<IHandler>> OnRun => throw new NotImplementedException();

        public Queue<IMessage> messages => throw new NotImplementedException();

        public event EventHandler<IAddress> AddressRemoved;

        public void AddDynamicSubscription<T, TH>(string serialPort, SubscriptionType subscriptionType)
            where T : IMessage
            where TH : IHandler<IMessage>
        {
            throw new NotImplementedException();
        }

        public void AddSubscription<T, TH>(string serialPort, SubscriptionType subscriptionType)
            where T : IMessage
            where TH : IHandler<IMessage>
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<SubscriptionInfo> GetHandlersForAddress(IAddress address)
        {
            throw new NotImplementedException();
        }

        public string GetMessageKey<T>()
        {
            throw new NotImplementedException();
        }

        public Type GetMessageTypeByAddress(IAddress address)
        {
            throw new NotImplementedException();
        }

        public bool HasSubscriptionsForAddress(IAddress address)
        {
            throw new NotImplementedException();
        }

        public bool HasSubscriptionsForMessage<T>(string serialPort) where T : IMessage
        {
            throw new NotImplementedException();
        }


        public void RemoveSubscription<T, TH>(string serialPort)
            where T : IMessage
            where TH : IHandler<IMessage>
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// TODO один адрес (byte,byte) => один подписчик. пока так
    /// </summary>
    //public partial class InMemoryMessageBusSubscriptionsManager : IMessageBusSubscriptionManager
    //{
    //    /// <summary>
    //    /// (route,messageType)
    //    /// </summary>
    //    private readonly Dictionary<IAddress, List<SubscriptionInfo>> _handlers;
    //    private readonly Dictionary<IAddress, IHandlerContext> _contexts;
    //    /// <summary>
    //    /// byte is MessageType
    //    /// </summary>
    //    private readonly Dictionary<(byte, byte), Type> _messageTypes;

    //    /// <summary>
    //    /// TODO ивент
    //    /// </summary>
    //    public event EventHandler<(byte, byte)> OnRouteMessageRemoved;

    //    public InMemoryMessageBusSubscriptionsManager()
    //    {
    //        _handlers = new Dictionary<(byte, byte), List<SubscriptionInfo>>();
    //        _messageTypes = new Dictionary<(byte, byte), Type>();
    //        _contexts = new Dictionary<byte, IHandlerContext>();
    //    }

    //    event EventHandler<IAddress> IMessageBusSubscriptionManager.OnRouteMessageRemoved
    //    {
    //        add
    //        {
    //            throw new NotImplementedException();
    //        }

    //        remove
    //        {
    //            throw new NotImplementedException();
    //        }
    //    }

    //    //private void InitMessageTypes()
    //    //{
    //    //    _messageTypes.Add((byte)MessageType.Notification, typeof(NotificationMessage));
    //    //    _messageTypes.Add((byte)MessageType.ACK, typeof(AcknowledgeMessage));
    //    //    _messageTypes.Add((byte)MessageType.NACK, typeof(NegativeAcknowledgeMessage));
    //    //    _messageTypes.Add((byte)MessageType.Request, typeof(RequestMessage));
    //    //    _messageTypes.Add((byte)MessageType.RequestResponse, typeof(RequestResponseMessage));
    //    //    _messageTypes.Add((byte)MessageType.Command, typeof(CommandMessage));
    //    //    _messageTypes.Add((byte)MessageType.CommandResponse, typeof(CommandResponseMessage));
    //    //}

    //    public bool IsEmpty => !_handlers.Keys.Any();
    //    public void Clear() => _handlers.Clear();

    //    public void AddSubscription<T, TH>()
    //        where T : IMessage
    //        where TH : IHandler<IMessage>
    //    {
    //        var routeAttribute = typeof(TH).GetCustomAttributes(typeof(CommandSubscriptionAttribute), true).FirstOrDefault() as CommandSubscriptionAttribute;
    //        var messageTypeAttribute = typeof(T).GetCustomAttributes(typeof(MessageTypeAttribute), true).FirstOrDefault() as MessageTypeAttribute;
    //        if (messageTypeAttribute is null)
    //        {
    //            throw new ArgumentException(
    //              $"Message Type {typeof(T).Name} has no MessageTypeAttribute", nameof(messageTypeAttribute));
    //        }
    //        if (!(routeAttribute is null))
    //        {
    //            DoAddSubscription(typeof(TH), routeAttribute.Route, messageTypeAttribute.MessageType);
    //        }
    //        else
    //        {
    //            throw new ArgumentException(
    //              $"Handler Type {typeof(T).Name} has no RouteAttribute", nameof(routeAttribute));
    //        }

    //        if (!_messageTypes.ContainsKey((routeAttribute.Route, messageTypeAttribute.MessageType)))
    //        {
    //            _messageTypes.Add((routeAttribute.Route, messageTypeAttribute.MessageType), typeof(T));
    //        }

    //    }

    //    private void DoAddSubscription(Type handlerType, byte route, byte messageType)
    //    {
    //        if (!HasSubscriptionsForAddress(route, messageType))
    //        {
    //            _handlers.Add((route, messageType), new List<SubscriptionInfo>());
    //        }
    //        if (_handlers[(route, messageType)].Any(s => s.HandlerType == handlerType))
    //        {
    //            throw new ArgumentException(
    //               $"Handler Type {handlerType.Name} already registered for '{BitConverter.ToString(new byte[] { route, messageType })}'", nameof(handlerType));
    //        }

    //        _handlers[(route, messageType)].Add(SubscriptionInfo.Typed(handlerType, route));
    //    }

    //    public IEnumerable<SubscriptionInfo> GetHandlersForAddress(byte route, byte messageType)
    //    {
    //        _handlers.TryGetValue((route, messageType), out List<SubscriptionInfo> value);
    //        return value;
    //    }

    //    public string GetMessageKey<T>()
    //    {
    //        return typeof(T).Name;
    //    }

    //    public Type GetMessageTypeByAddress(byte route, byte messageType)
    //    {
    //        _messageTypes.TryGetValue((route, messageType), out Type value);
    //        return value;
    //    }

    //    public bool HasSubscriptionsForMessage<T>(byte route) where T : IMessage
    //    {
    //        var messageTypeAttribute = typeof(T).GetCustomAttributes(typeof(MessageTypeAttribute), true).FirstOrDefault() as MessageTypeAttribute;
    //        if (!(messageTypeAttribute is null))
    //        {
    //            _messageTypes.TryGetValue((route, messageTypeAttribute.MessageType), out Type type);
    //            if (type == typeof(T))
    //            {
    //                return true;
    //            }
    //        }
    //        else
    //        {
    //            throw new ArgumentException(
    //               $"Message Type {typeof(T).Name} has no MessageTypeAttribute", nameof(messageTypeAttribute));
    //        }
    //        return false;
    //    }

    //    public bool HasSubscriptionsForAddress(byte route, byte messageType)
    //    {
    //        if (_messageTypes.ContainsKey((route, messageType)))
    //        {
    //            return true;
    //        }
    //        return false;
    //    }

    //    public void RemoveSubscription<T, TH>()
    //        where T : IMessage
    //        where TH : IHandler<IMessage>
    //    {
    //        var routeAttribute = typeof(TH).GetCustomAttributes(typeof(CommandSubscriptionAttribute), true).FirstOrDefault() as CommandSubscriptionAttribute;
    //        var messageTypeAttribute = typeof(T).GetCustomAttributes(typeof(MessageTypeAttribute), true).FirstOrDefault() as MessageTypeAttribute;
    //        if (messageTypeAttribute is null)
    //        {
    //            throw new ArgumentException(
    //              $"Message Type {typeof(T).Name} has no MessageTypeAttribute", nameof(messageTypeAttribute));
    //        }
    //        if (routeAttribute is null)
    //        {
    //            throw new ArgumentException(
    //              $"Handler Type {typeof(T).Name} has no RouteAttribute", nameof(routeAttribute));
    //        }
    //        _handlers.TryGetValue((routeAttribute.Route, messageTypeAttribute.MessageType), out List<SubscriptionInfo> value);
    //        if (!(value is null))
    //        {
    //            var handlerToRemove = value.Where(s => s.HandlerType == typeof(TH)).FirstOrDefault();
    //            if (!(handlerToRemove is null))
    //            {
    //                _handlers[(routeAttribute.Route, messageTypeAttribute.MessageType)].Remove(handlerToRemove);
    //                _messageTypes.Remove((routeAttribute.Route, messageTypeAttribute.MessageType));
    //            }
    //        }
    //    }

    //    public void AddSubscription<T, TH>(string serialPort)
    //        where T : IMessage
    //        where TH : IHandler<IMessage>
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public IEnumerable<SubscriptionInfo> GetHandlersForAddress(IAddress address)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public Type GetMessageTypeByAddress(IAddress address)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public bool HasSubscriptionsForAddress(IAddress address)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public bool HasSubscriptionsForMessage<T>(string serialPort) where T : IMessage
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public void RemoveSubscription<T, TH>(string serialPort)
    //        where T : IMessage
    //        where TH : IHandler<IMessage>
    //    {
    //        throw new NotImplementedException();
    //    }
    //}
}
