namespace BPServer.Core.Handlers
{
    using BPServer.Core.Attributes;
    using BPServer.Core.Messages;
    using System.Threading.Tasks;

    /// <summary>
    /// marker
    /// </summary>
    public interface IHandler<in TMessage> : IHandler
        where TMessage : IMessage
    {
        Task Handle(TMessage input);
    }

    public interface IHandler
    {
        public byte Route();
    }

    //public interface IHandler<TMessage,TResponse>
    //    where TMessage : IMessage
    //    where TResponse : IMessage
    //{
    //    Task<TResponse> Handle(TMessage input);
    //}
}
