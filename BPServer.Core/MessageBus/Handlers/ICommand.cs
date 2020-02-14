namespace BPServer.Core.MessageBus.Handlers
{
    /// <summary>
    /// marker
    /// </summary>
    public interface ICommand
    {
        byte Command { get; }
    }
}
