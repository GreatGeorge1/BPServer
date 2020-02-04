namespace BPServer.Core.Handlers
{
    using BPServer.Core.Attributes;
    using BPServer.Core.Messages;
    using System.Threading.Tasks;
    public interface INotificationHandler : IHandler
    {
        [Method(MessageType.Notification)]
        Task Notification(NotificationMessage input);
    }
}
