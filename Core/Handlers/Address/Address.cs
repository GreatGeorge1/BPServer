using System;

namespace BPServer.Core.Handlers
{
    public class Address : IAddress
    {
        public Address(IRoute route, string serialPort)
        {
            if (string.IsNullOrWhiteSpace(serialPort))
            {
                throw new ArgumentException("message", nameof(serialPort));
            }

            Route = route ?? throw new ArgumentNullException(nameof(route));
            SerialPort = serialPort;
        }

        public IRoute Route { get; protected set; }

        public string SerialPort { get; protected set; }
    }
}
