namespace BPServer.Core.MessageBus
{
    using BPServer.Core.Extentions;
    using BPServer.Core.MessageBus.Attributes;
    using BPServer.Core.MessageBus.Handlers;
    using BPServer.Core.MessageBus.Handlers.Address;
    using BPServer.Core.MessageBus.Messages;
    using Serilog;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class InMemoryMessageBusSubscriptionsManager : IMessageBusSubscriptionManager
    {
        private readonly Dictionary<IAddress, HashSet<SubscriptionInfo>> _handlers;
        private readonly Dictionary<byte,Type> _commandTypes;
        private readonly Dictionary<byte, Type> _messageTypes;
        private readonly ILogger log;

        public InMemoryMessageBusSubscriptionsManager(ILogger logger)
        {
            if (logger is null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            _handlers = new Dictionary<IAddress, HashSet<SubscriptionInfo>>();
            _commandTypes = new Dictionary<byte, Type>();
            _messageTypes = new Dictionary<byte, Type>();
            log = logger;
        }

        public bool IsEmpty => !_handlers.Any();

        public event EventHandler<IAddress> AddressRemoved;

        protected virtual void OnAddressRemoved(IAddress e)
        {
            EventHandler<IAddress> handler = AddressRemoved;
            handler?.Invoke(this, e);
        }

        public void AddSubscription<T>(string transportName) where T : IHandler
        {
            if (string.IsNullOrWhiteSpace(transportName)) throw new ArgumentException("message", nameof(transportName));
            var type =typeof(T);
            var typeSet = GetGenericTypeArguments(type);
            var commandTypes = FindCommandTypes(typeSet);
            var messageTypes = FindMessageTypes(typeSet);
            DoAddSubscription(GetAddresses(commandTypes.Single(), messageTypes, transportName, type));
        }

        private void DoAddSubscription(Dictionary<IAddress, SubscriptionInfo> addresses)
        {
            if (addresses is null) throw new ArgumentNullException(nameof(addresses));
            foreach (var item in addresses)
            {
                var temp = _handlers.GetValueOrDefault(item.Key);
                if (temp is null)
                {
                    var subs = new HashSet<SubscriptionInfo>();
                    subs.Add(item.Value);
                    _handlers.Add(item.Key, subs);
                    log.Information("Subscribed '{@Handler}' on '{@Address}'", item.Value.HandlerType.Name,item.Value.Address);
                }
                else
                {
                    temp.Add(item.Value);
                }
                if(!_commandTypes.TryAdd(item.Key.Route.Command, item.Value.CommandType))
                {
                    log.Debug("tryadd command failed on '{@CommandType}'", item.Value.CommandType);
                }
                if(!_messageTypes.TryAdd(item.Key.Route.MessageType, item.Value.MessageType))
                {
                    log.Debug("tryadd message failed on '{@MessageType}'", item.Value.MessageType);
                }
            }
          
        }

        private IEnumerable<Type> GetGenericTypeArguments(Type type)
        {
            if (type is null) throw new ArgumentNullException(nameof(type));
            return type.GetInterfaces().Where(x => x.IsGenericType).SelectMany(x => x.GenericTypeArguments).ToHashSet();
        }

        private IEnumerable<Type> FindCommandTypes(IEnumerable<Type> input)
        {
            if (input is null) throw new ArgumentNullException(nameof(input));
            return input.Where(x => typeof(ICommand).IsAssignableFrom(x) && x.GetInterfaces().Contains(typeof(ICommand)));
        }

        private IEnumerable<Type> FindMessageTypes(IEnumerable<Type> input)
        {
            if (input is null) throw new ArgumentNullException(nameof(input));
            return input.Where(x => typeof(IMessage).IsAssignableFrom(x) && x.GetInterfaces().Contains(typeof(IMessage)));
        }

        private Dictionary<IAddress, SubscriptionInfo> GetAddresses(Type commandType, IEnumerable<Type> messageTypes, string transportName, Type handler)
        {
            if (commandType is null) throw new ArgumentNullException(nameof(commandType));
            if (messageTypes is null) throw new ArgumentNullException(nameof(messageTypes));
            if (string.IsNullOrWhiteSpace(transportName)) throw new ArgumentException("message", nameof(transportName));
            var addresses = new Dictionary<IAddress, SubscriptionInfo>();

            foreach (var message in messageTypes)
            {
                var commandByte = commandType.GetAttributeValue((CommandByteAttribute cbyte) => cbyte.Command);
                var messageByte = message.GetAttributeValue((MessageTypeAttribute mtype) => mtype.MessageType);
                var address = new Address(new Route(commandByte,messageByte),transportName);
                var sub = SubscriptionInfo.Typed(address,commandType,message,handler);
                addresses.Add(address, sub);
            }
            return addresses;
        }

        public void Clear()
        {
            _handlers.Clear();
            _messageTypes.Clear();
            _commandTypes.Clear();
        }

        public Type GetCommandTypeByByte(byte input)
        {
            var temp = _commandTypes.GetValueOrDefault(input);
            if(temp is null) return default;
            return temp;
        }

        public IEnumerable<SubscriptionInfo> GetHandlersForAddress(IAddress address)
        {
            foreach (var item in _handlers.Keys)
            {
                if (item.TransportName.Equals(address.TransportName)
                && item.Route.Command == address.Route.Command
                && item.Route.MessageType == address.Route.MessageType)
                {
                    return _handlers.GetValueOrDefault(item);
                }
            }
            return default;
        }

        public Type GetMessageTypeByByte(byte input)
        {
            var temp = _messageTypes.GetValueOrDefault(input);
            if (temp is null) return default;
            return temp;
        }

        public bool HasSubscriptionsForAddress(IAddress address)
        {
            foreach(var item in _handlers.Keys)
            {
                if(item.TransportName.Equals(address.TransportName)
                && item.Route.Command == address.Route.Command 
                && item.Route.MessageType == address.Route.MessageType)
                {
                    return _handlers.Values.Count > 0;
                }
            }
            return false;
        }

        public bool HasSubscriptionsForCommand<T>(string transportName) where T : ICommand
        {
            if (string.IsNullOrWhiteSpace(transportName))
            {
                throw new ArgumentException("message", nameof(transportName));
            }

            var commandByte = typeof(T).GetAttributeValue((CommandByteAttribute cmd) => cmd.Command);
            foreach(var item in _handlers)
            {
                if(item.Key.TransportName.Equals(transportName) && item.Key.Route.Command == commandByte)
                {
                    return true;
                }
            }
            return false;
        }

        public void RemoveSubscription<T>(string transportName) where T : IHandler
        {

            //OnAddressRemoved(address);
            throw new NotImplementedException();
        }
    }
}
