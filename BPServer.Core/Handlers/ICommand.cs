using BPServer.Core.Attributes;

namespace BPServer.Core.Handlers
{
    /// <summary>
    /// marker
    /// </summary>
    public interface ICommand
    {
        byte Command { get; }
    }
}
