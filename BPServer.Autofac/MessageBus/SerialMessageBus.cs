using Autofac;
using BPServer.Core.Transports;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Serilog;
using BPServer.Core.MessageBus.Messages;
using BPServer.Core.MessageBus.Handlers.Address;
using BPServer.Core.MessageBus.Handlers;

namespace BPServer.Core.MessageBus
{
    public partial class SerialMessageBus : IMessageBus, IDisposable
    {
        private readonly string AUTOFAC_SCOPE_NAME = "bpserver_serial_message_bus";
        private readonly ITransportManager _transportManager;
        private readonly IMessageBusSubscriptionManager _subscriptionManager;
        private readonly ILifetimeScope _autofac;
        private readonly ILogger log;

        public SerialMessageBus(ITransportManager transportManager,
            IMessageBusSubscriptionManager subscriptionManager,
            ILifetimeScope autofac,
            ILogger logger)
        {
            _transportManager = transportManager ?? throw new ArgumentNullException(nameof(transportManager));
            _transportManager.MessageReceived += OnMessageReceived;
            _subscriptionManager = subscriptionManager ?? throw new ArgumentNullException(nameof(subscriptionManager));
            _autofac = autofac ?? throw new ArgumentNullException(nameof(autofac));
            log = logger ?? throw new ArgumentNullException(nameof(logger));
            log.Information("MessageBus Created");
        }

        protected async void OnMessageReceived(object sender, MessageReceivedEventArgs e)
        {
            var serialPort = e.Transport.Name;
            log.Verbose($"MessageBus OnMessageReceived, TransportName: '{serialPort}'");
            await ProcessMessage(e.Message, serialPort).ConfigureAwait(false);
        }

        protected async Task<bool> ProcessMessage(IMessage message, string serialPort)
        {
            var processed = false;
            var address = new Address(new Route(message.Command, (byte)message.Type), serialPort);
            if (_subscriptionManager.HasSubscriptionsForAddress(address))
            {
                processed = true;
                using (var scope = _autofac.BeginLifetimeScope(AUTOFAC_SCOPE_NAME))
                {
                    var subscriptions = _subscriptionManager.GetHandlersForAddress(address);
                 
                    foreach(var subscription in subscriptions)
                    {
                        var handler = scope.ResolveOptional(subscription.HandlerType);
                        if (handler is null) continue;
                        var messageType = _subscriptionManager.GetMessageTypeByByte((byte)message.Type);
                        var commandType = _subscriptionManager.GetCommandTypeByByte(message.Command);
                        var concreteType = typeof(IHandler<,>).MakeGenericType(new Type[] { messageType, commandType });
                        try
                        {
                            await (Task)concreteType.GetMethod("Handle").Invoke(handler, new object[] { message, address });
                        }
                        catch(Exception e)
                        {
                            processed = false;
                            await ExceptionReceivedHandler(new ExceptionReceivedEventArgs(e, concreteType)).ConfigureAwait(false);
                        }
                    }
                }
               
            }
            return processed;
        }
        private Task ExceptionReceivedHandler(ExceptionReceivedEventArgs e)
        {
            var ex = e.Exception;
            var context = e.ExceptionReceivedContext;

            log.Warning(ex.ToString(), "ERROR handling message: {ExceptionMessage} - Context: {@ExceptionContext}", ex.Message, context.FullName);

            return Task.CompletedTask;
        }


        public  async Task Publish(IMessage message, string serialPort)
        {
            var transport = _transportManager.GetTransportByName(serialPort);
            await transport.PushDataAsync(message).ConfigureAwait(false);
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
            }

            disposed = true;
        }
    }
}
