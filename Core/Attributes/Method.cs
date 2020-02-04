namespace BPServer.Core.Attributes
{
    using BPServer.Core.Messages;

    [System.AttributeUsage(System.AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public sealed class Method : System.Attribute
    {
        public MessageType messageType;

        public Method(MessageType messageType)
        {
            this.messageType = messageType;
        }
    }
}
