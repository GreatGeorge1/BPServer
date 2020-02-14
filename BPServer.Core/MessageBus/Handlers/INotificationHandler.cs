using BPServer.Core.MessageBus.Messages;

namespace BPServer.Core.MessageBus.Handlers
{
    public interface INotificationHandler<in TCommand> : IHandler<NotificationMessage, TCommand>
        where TCommand : ICommand
    {
    }
}
