namespace BPServer.Core.Messages
{
    using BPServer.Core.Handlers;
    using System;

    public interface IMessageContext
    {
        IAddress Address { get; }
        IMessage Message { get; }
        Type  HandlerType { get; }
    }
}