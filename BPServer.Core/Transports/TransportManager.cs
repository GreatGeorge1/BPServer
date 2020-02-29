using BPServer.Core.MessageBus.Messages;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace BPServer.Core.Transports
{
    public class TransportManager : ITransportManager, IDisposable
    {
        private bool isDisposed;
        protected readonly List<ITransport> _transports;

        public event EventHandler<MessageReceivedEventArgs> MessageReceived;
        public event EventHandler<TransportAddedEventArgs> TransportAdded;

        private readonly ILogger log;

        public TransportManager(ILogger logger)
        {
            log = logger ?? throw new ArgumentNullException(nameof(logger));
            _transports = new List<ITransport>();
        }

        protected void OnTransportAdded(TransportAddedEventArgs e)
        {
            TransportAdded?.Invoke(this, e);
        }

        protected void OnMessageReceived(MessageReceivedEventArgs e)
        {
            MessageReceived?.Invoke(this, e);
            //Console.WriteLine("+");
        }

        protected void OnDataRecieved(object sender, IMessage message)
        {
            log.Verbose("Transportmanager DataReceived");
            ITransport transport = (ITransport)sender;
            OnMessageReceived(new MessageReceivedEventArgs(message, transport));
        }

        public void AddTransport(ITransport transport)
        {
            if(!(_transports.Find(x => x.Name.Equals(transport.Name)) is null))
            {
                log.Warning($"Transport with name: '{transport.Name}' already registered");
                return;
            }
            transport.DataReceived += OnDataRecieved;
            _transports.Add(transport);
            log.Verbose("Transport added");
            OnTransportAdded(new TransportAddedEventArgs(transport.Name, transport.GetType()));
        }

        public ITransport GetTransportByName(string name)
        {
            var transport = _transports.FirstOrDefault(x => x.Name.Equals(name));
            if(transport is null)
            {
                return default;
            }
            return transport;
        }

        public void RemoveTransport(ITransport transport)
        {
            if (_transports.Remove(transport))
            {
                transport.DataReceived -= OnDataRecieved;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (isDisposed) return;
            if (_transports.Any())
            {
                foreach(var transport in _transports)
                {
                    transport.DataReceived -= OnDataRecieved;
                }
                _transports.Clear();
            }

            isDisposed = true;
        }
    }
}
