namespace BPServer.Core.Handlers
{
    public class Route : IRoute
    {
        public Route(byte command, byte messageType)
        {
            Command = command;
            MessageType = messageType;
        }

        public byte Command { get; protected set; }

        public byte MessageType { get; protected set; }
    }
}
