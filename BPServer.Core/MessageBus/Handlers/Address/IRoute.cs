namespace BPServer.Core.MessageBus.Handlers.Address
{
    public interface IRoute
    {
        byte Command { get; }
        byte MessageType { get; }
    }
}
