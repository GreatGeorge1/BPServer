﻿namespace BPServer.Core.Exceptions
{
    using System;

    public class DomainException : Exception
    {
        public DomainException(string message) : base(message)
        {
        }

        public DomainException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public DomainException()
        {
        }
    }

    public class MessageTypeException : DomainException
    {
        public MessageTypeException(string message) : base(message)
        {
        }

        public MessageTypeException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public MessageTypeException()
        {
        }
    }

}
