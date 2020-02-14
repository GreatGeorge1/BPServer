using BPServer.Core.Extentions;
using BPServer.Core.MessageBus.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPServer.Core.MessageBus.Messages
{
    public class MessageFactory : IMessageFactory
    {
        private readonly Dictionary<byte, Type> messageTypes;
        public MessageFactory()
        {
            var type = typeof(IMessage);
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => type.IsAssignableFrom(p) && !p.IsAbstract && !p.IsInterface).ToHashSet();
            messageTypes = new Dictionary<byte, Type>();
            foreach (var item in types)
            {
                var key = item.GetAttributeValue((MessageTypeAttribute mtype) => mtype.MessageType);
                messageTypes.TryAdd(key, item);
            }
        }

        public bool CreateMessage(byte[] input, out IMessage message)
        {
            message = null;
            if (Message.IsValid(input))
            {
                var type = messageTypes.GetValueOrDefault(input[1]);
                if (!(type is null))
                {
                    message = (IMessage)Activator.CreateInstance(type, new object[] { input });
                    return true;
                }
            }
            return false;
        }
    }
}
