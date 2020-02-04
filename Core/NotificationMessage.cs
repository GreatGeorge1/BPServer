using BPServer.Core.Exceptions;

namespace BPServer.Core
{
    public class NotificationMessage : Message
    {
        public NotificationMessage(byte[] message) : base(message)
        {
            if (IsTypeOf(message, MessageType.Notification) == false)
            {
                throw new MessageTypeException($"{nameof(message)} MessageType is not {MessageType.Notification}");
            }
        }

        public NotificationMessage(byte Route, byte[] Value) : base(MessageType.Notification, Route, Value)
        {
        }

        public new const MessageType Type = MessageType.Notification;
    }
}
