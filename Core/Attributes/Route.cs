using System;
using System.Collections.Generic;
using System.Text;

namespace BPServer.Core.Attributes
{
    [System.AttributeUsage(System.AttributeTargets.Class)]
    public sealed class Route : System.Attribute
    {
        public byte route;

        public Route(byte route)
        {
            this.route = route;
        }
    }

    [System.AttributeUsage(System.AttributeTargets.Method, AllowMultiple = true)]
    public sealed class Method : System.Attribute
    {
        public MessageType messageType;

        public Method(MessageType messageType)
        {
            this.messageType = messageType;
        }
    }
}
