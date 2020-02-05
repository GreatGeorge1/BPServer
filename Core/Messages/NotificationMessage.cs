namespace BPServer.Core.Messages
{
    using BPServer.Core.Attributes;
    using BPServer.Core.Exceptions;

    [MessageType((byte)MessageType.Notification)]
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
