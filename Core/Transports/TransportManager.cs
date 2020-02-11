﻿using BPServer.Core.Messages;
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

        public TransportManager()
        {
            _transports = new List<ITransport>();
        }

        protected virtual void OnMessageReceived(MessageReceivedEventArgs e)
        {
            EventHandler<MessageReceivedEventArgs> handler = MessageReceived;
            handler?.Invoke(this, e);
        }

        protected void OnDataRecieved(object sender, IMessage message)
        {
            OnMessageReceived(new MessageReceivedEventArgs(message, sender as ITransport));
        }

        public void AddTransport(ITransport transport)
        {
            if(!(_transports.Find(x => x.Name.Equals(transport.Name)) is null))
            {
                Debug.WriteLine($"Transport with name: '{transport.Name}' already registered");
                return;
            }
            transport.DataReceived += OnDataRecieved;
            _transports.Add(transport);
        }

        public ITransport GetTransportByName(string name)
        {
            var transport = _transports.FirstOrDefault(x => x.Name.Equals(name));
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
