namespace BPServer.Core.Handlers
{
    using BPServer.Core.Attributes;
    using BPServer.Core.Messages;
    using System.Threading.Tasks;

    public interface IRequestController : IAcknowledgeHandler
    {
        [Method(MessageType.Request)]
        Task<RequestResponseMessage> Request(RequestMessage input);
    }
}
