using BPServer.Core.Handlers;

namespace BPServer.Core.Attributes
{
    [System.AttributeUsage(System.AttributeTargets.Class)]
    public sealed class CommandByteAttribute : System.Attribute
    {
        public byte Command { get; }

        public CommandByteAttribute(byte command)
        {
            Command = command;
        }
    }
}
