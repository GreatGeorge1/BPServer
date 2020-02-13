namespace BPServer.Core.Handlers
{
    using BPServer.Core.Attributes;
    using BPServer.Core.Messages;
    using System.Threading.Tasks;
    public interface INotificationHandler<in TCommand> : IHandler<NotificationMessage, TCommand>
        where TCommand : ICommand
    {
    }
}
