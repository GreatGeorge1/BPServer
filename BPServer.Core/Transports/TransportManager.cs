using BPServer.Core.MessageBus.Messages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace BPServer.Core.Transports
{
    public class TransportManager : ITransportManager
    {
        protected readonly List<ITransport> _transportSubscriptions;

        public event EventHandler<TransportAddedEventArgs> TransportAdded;

        private readonly ILogger log;

        public TransportManager(ILogger<TransportManager> logger)
        {
            log = logger ?? throw new ArgumentNullException(nameof(logger));
            _transportSubscriptions = new List<ITransport>();
        }

        protected void OnTransportAdded(TransportAddedEventArgs e)
        {
            TransportAdded?.Invoke(this, e);
        }

        public void AddTransport(ITransport transport)
        {
            if (!(_transportSubscriptions.Find(x => x.Name.Equals(transport.Name)) is null))
            {
                log.LogWarning($"Transport with name: '{transport.Name}' already registered");
                return;
            }
            _transportSubscriptions.Add(transport);
            log.LogDebug("Transport added");
            OnTransportAdded(new TransportAddedEventArgs(transport.Name, transport.GetType()));
        }

        public ITransport GetTransportByName(string name)
        {
            var transport = _transportSubscriptions.FirstOrDefault(x => x.Name.Equals(name));
            if (transport is null)
            {
                return null;
            }
            return transport;
        }

        public void RemoveTransport(ITransport transport)
        {
            foreach(var item in _transportSubscriptions)
            {
                if (item.Name.Equals(transport.Name))
                {
                    //var unsub = item.Unsubscribe;
                    _transportSubscriptions.Remove(item);
                    //unsub.Dispose();
                }
            }
        }

        public void Clear()
        {
            if (_transportSubscriptions.Any())
            {
                _transportSubscriptions.Clear();
            }
        }

        public IEnumerable<ITransport> GetTransports()
        {
            return _transportSubscriptions.AsReadOnly();
        }
    }
}
