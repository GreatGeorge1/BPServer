using BPServer.Core.Handlers;

namespace BPServer.Core.Attributes
{
    [System.AttributeUsage(System.AttributeTargets.Class)]
    public sealed class CommandSubscriptionAttribute : System.Attribute
    {
        public ICommand Command { get; }

        public CommandSubscriptionAttribute(ICommand command)
        {
            Command = command;
        }
    }
}
