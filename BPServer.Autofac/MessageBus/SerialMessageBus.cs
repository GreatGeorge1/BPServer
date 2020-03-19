using Autofac;
using BPServer.Core.Transports;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using BPServer.Core.MessageBus.Messages;
using BPServer.Core.MessageBus.Handlers.Address;
using BPServer.Core.MessageBus.Handlers;
using Microsoft.Extensions.Logging;

namespace BPServer.Core.MessageBus
{
    public partial class SerialMessageBus : IMessageBus, IDisposable
    {
        private readonly string AUTOFAC_SCOPE_NAME = "bpserver_serial_message_bus";
        private readonly ITransportManager _transportManager;
        private readonly IMessageBusSubscriptionManager _subscriptionManager;
        private readonly ILifetimeScope _autofac;
        private readonly ILogger log;

        public SerialMessageBus(
            IMessageBusSubscriptionManager subscriptionManager,
            ILifetimeScope autofac,
            ILogger<SerialMessageBus> logger, ILogger<TransportManager> logger1)
        {
            _transportManager = new TransportManager(logger1);
            _transportManager.TransportAdded += OnTransportAdded;
            _subscriptionManager = subscriptionManager ?? throw new ArgumentNullException(nameof(subscriptionManager));
            _autofac = autofac ?? throw new ArgumentNullException(nameof(autofac));
            log = logger ?? throw new ArgumentNullException(nameof(logger));
            log.LogDebug("MessageBus Created");
        }

        protected async Task<bool> ProcessMessage(IMessage message, IAddress address)
        {
            log.LogDebug("Process message");
            var processed = true;
            using (var scope = _autofac.BeginLifetimeScope(AUTOFAC_SCOPE_NAME))
            {
                var subscriptions = _subscriptionManager.GetHandlersForAddress(address);

                foreach (var subscription in subscriptions)
                {
                    log.LogDebug($"ProcessMessage foreach, transport: '{address.TransportName}'");
                    try
                    {
                        var handler = scope.Resolve(subscription.HandlerType);
                        if (handler is null) continue;
                        var messageType = _subscriptionManager.GetMessageTypeByByte((byte)message.Type);
                        var commandType = _subscriptionManager.GetCommandTypeByByte(message.Command);
                        var concreteType = typeof(IHandler<,>).MakeGenericType(new Type[] { messageType, commandType });
                        await (Task)concreteType.GetMethod("Handle").Invoke(handler, new object[] { message, address });
                    }
                    catch (Exception e)
                    {
                        processed = false;
                        await ExceptionReceivedHandler(new ExceptionReceivedEventArgs(e, message, address.TransportName)).ConfigureAwait(false);
                    }
                }
            }
            return processed;
        }
        private Task ExceptionReceivedHandler(ExceptionReceivedEventArgs e)
        {
            var ex = e.Exception;
            //var context = e.ExceptionReceivedContext;

            log.LogWarning(ex.ToString(), "ERROR handling message: {ExceptionMessage} - Exchange: {@exchange}", ex.Message,
                e.Exchange
                );

            return Task.CompletedTask;
        }

        public async Task Publish(IMessage message, string exchange)
        {
            var address = new Address(message.Command, message.Type, exchange);
            if (_subscriptionManager.HasSubscriptionsForAddress(address))
            {
                if (await ProcessMessage(message, address))
                {
                    return;
                }
                else
                {
                    log.LogWarning($"Message NOT processed: '{BitConverter.ToString(message.Raw)}'");
                }
            }
            else
            {
                var transport = _transportManager.GetTransportByName(exchange);
                if (transport is null)
                {
                    log.LogWarning($"{exchange} not exist in TrasnportManager");
                    return;
                }
                await transport.PushDataAsync(message).ConfigureAwait(false);
            }
        }

        public void Subscribe<T>(string serialPort) where T : IHandler
        {
            _subscriptionManager.AddSubscription<T>(serialPort);
        }

        public void Unsubscribe<T>(string serialPort) where T : IHandler
        {
            _subscriptionManager.RemoveSubscription<T>(serialPort);
        }

        bool disposed = false;

        public event EventHandler<TransportAddedEventArgs> TransportAdded;

        protected void OnTransportAdded(object sender, TransportAddedEventArgs e)
        {
            TransportAdded?.Invoke(this, e);
        }


        // Public implementation of Dispose pattern callable by consumers.
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // Protected implementation of Dispose pattern.
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                _subscriptionManager.Clear();
                _transportManager.Clear();
            }

            disposed = true;
        }

        public void AddTransport(ITransport transport)
        {
            _transportManager.AddTransport(transport);
            transport.SetMessageBus(this);
        }

        public void RemoveTransport(ITransport transport) => _transportManager.RemoveTransport(transport);
    }
}
