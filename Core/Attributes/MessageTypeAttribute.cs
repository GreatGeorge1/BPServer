namespace BPServer.Core.Attributes
{
    using BPServer.Core.Messages;

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
