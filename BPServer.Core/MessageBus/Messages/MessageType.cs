namespace BPServer.Core.MessageBus.Messages
{
    public enum MessageType
    {
        NotSet = 0,
        Request = 0xD1,
        RequestResponse = 0xD2,
        Command = 0xD3,
        Notification = 0xD5,
        CommandResponse = 0xD6,
        ACK = 0xD7,
        NACK = 0xD8
    }
}