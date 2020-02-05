namespace BPServer.Core.Handlers
{
    using BPServer.Core.Messages;

    public interface IRequestHandler : IHandler<RequestMessage>
    {
    }

    public interface IResponseHandler : IHandler<RequestResponseMessage>, IAcknowledgeHandler
    {
    }
}
