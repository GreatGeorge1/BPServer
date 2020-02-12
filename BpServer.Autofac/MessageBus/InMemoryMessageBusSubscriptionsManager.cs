using BPServer.Core.Handlers;

namespace BPServer.Core.MessageBus
{
    using BPServer.Core.Attributes;
    using BPServer.Core.Extentions;
    using BPServer.Core.Messages;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class InMemoryMessageBusSubscriptionsManager : IMessageBusSubscriptionManager
    {
        private readonly Dictionary<IAddress, HashSet<SubscriptionInfo>> _handlers;
        private readonly Dictionary<byte,Type> _commandTypes;
        private readonly Dictionary<byte, Type> _messageTypes;

        public bool IsEmpty => !_handlers.Any();

        public event EventHandler<IAddress> AddressRemoved;

        protected virtual void OnAddressRemoved(IAddress e)
        {
            EventHandler<IAddress> handler = AddressRemoved;
            handler?.Invoke(this, e);
        }

        public void AddSubscription<T>(string transportName) where T : IHandler<IMessage, ICommand>
        {
            if (string.IsNullOrWhiteSpace(transportName)) throw new ArgumentException("message", nameof(transportName));
            var type =typeof(T);
            var hashset = GetGenericTypeArguments(type);
            var commandTypes = FindCommandTypes(hashset);
            if (commandTypes.Count() > 1) throw new ArgumentException($"Handler of type: '{typeof(T)}' has more than one ICommand");

            var messageTypes = FindMessageTypes(hashset);
            var addresses = GetAddresses(commandTypes.Single(), messageTypes, transportName, type);
            foreach(var item in addresses)
            {
                var temp=_handlers.GetValueOrDefault(item.Key);
                if (temp is null)
                {
                    var subs = new HashSet<SubscriptionInfo>();
                    subs.Add(item.Value);
                    _handlers.Add(item.Key, subs);
                }
                else
                {
                    temp.Add(item.Value);
                }
                _commandTypes.TryAdd(item.Key.Route.Command, item.Value.CommandType);
                _messageTypes.TryAdd(item.Key.Route.MessageType, item.Value.MessageType);
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
            return input.Where(x => typeof(ICommand).IsAssignableFrom(x) && x.GetInterfaces().Contains(typeof(ICommand)))
        }

        private IEnumerable<Type> FindMessageTypes(IEnumerable<Type> input)
        {
            if (input is null) throw new ArgumentNullException(nameof(input));
            return input.Where(x => typeof(IMessage).IsAssignableFrom(x) && x.GetInterfaces().Contains(typeof(IMessage)));
        }

        private Dictionary<IAddress,SubscriptionInfo> GetAddresses(Type commandType, IEnumerable<Type> messageTypes, string transportName, Type handler)
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
            var temp = _handlers.GetValueOrDefault(address);
            if(temp is null)
            {
                return default;
            }
            return temp;
        }

        public Type GetMessageTypeByByte(byte input)
        {
            var temp = _messageTypes.GetValueOrDefault(input);
            if (temp is null) return default;
            return temp;
        }

        public bool HasSubscriptionsForAddress(IAddress address)
        {

            if(_handlers.TryGetValue(address, out HashSet<SubscriptionInfo> subs))
            {
                if (subs.Count > 0)
                {
                    return true;
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
                if(item.Key.SerialPort.Equals(transportName) && item.Key.Route.Command == commandByte)
                {
                    return true;
                }
            }
            return false;
        }

        public void RemoveSubscription<T>(string transportName) where T : IHandler<IMessage, ICommand>
        {

            //OnAddressRemoved(address);
            throw new NotImplementedException();
        }
    }
}
