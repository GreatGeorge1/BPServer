namespace BPServer.Core
{
    using BPServer.Core.Attributes;
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// marker
    /// </summary>
    public interface IHandler
    {
    }

    public interface INotificationController : IHandler
    {
        [Method(MessageType.Notification)]
        Task Notification(NotificationMessage input);
    }

    public interface IAcknowledgeHandler : IHandler
    {
        [Method(MessageType.ACK)]
        Task Acknowledge(AcknowledgMessage input);

        [Method(MessageType.NACK)]
        Task NegativeAcknowledge(NegativeAcknowledgeMessage input);
    }

    public interface IRequestController : IAcknowledgeHandler
    {
        [Method(MessageType.Request)]
        Task<RequestResponseMessage> Request(RequestMessage input);
    }


    /// <summary>
    /// marker
    /// </summary>
    public interface ICommand
    {
        
    }

    public interface ICommandHandler<TCommand> : IAcknowledgeHandler
        where TCommand : ICommand
    {

    }

    public interface ICommandResponseHandler<TCommand> : IAcknowledgeHandler
       where TCommand : ICommand
    {
        [Method(MessageType.CommandResponse)]
        Task CommandResponse(CommandResponseMessage input);
    }
}
