namespace BPServer.Core.MessageBus.Attributes
{
    [System.AttributeUsage(System.AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public sealed class MessageTypeAttribute : System.Attribute
    {
        public byte MessageType { get; }

        public MessageTypeAttribute(byte messageType)
        {
            MessageType = messageType;
        }
    }
}
