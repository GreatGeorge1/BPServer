namespace BPServer.Core.Handlers
{
    public interface IRoute
    {
        byte Command { get; }
        byte MessageType { get; }
    }
}
