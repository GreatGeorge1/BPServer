﻿using BPServer.Core.Handlers;

namespace BPServer.Core.MessageBus
{
    using Autofac;
    using BPServer.Core.Messages;
    using BPServer.Core.Transports;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Text;
    using System.Threading.Tasks;

    public partial class InMemoryMessageBus : IMessageBus
    {
        private readonly string AUTOFAC_SCOPE_NAME = "bpserver_message_bus";
        private readonly ITransportManager _transportManager;
        private readonly IMessageBusSubscriptionManager _subscriptionManager;
        private readonly ILifetimeScope _autofac;

        public InMemoryMessageBus(ITransportManager transportManager,
            IMessageBusSubscriptionManager subscriptionManager,
            ILifetimeScope autofac)
        {
            _transportManager = transportManager ?? throw new ArgumentNullException(nameof(transportManager));
            _transportManager.MessageReceived += OnMessageReceived;
            _subscriptionManager = subscriptionManager ?? throw new ArgumentNullException(nameof(subscriptionManager));
            _autofac = autofac ?? throw new ArgumentNullException(nameof(autofac));
        }

        protected async void OnMessageReceived(object sender, MessageReceivedEventArgs e)
        {
            var serialPort = e.Transport.Name;
            Debug.WriteLine($"MessageBus OnMessageReceived, TransportName: '{serialPort}'");
            await ProcessMessage(e.Message, serialPort);
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
                            await ExceptionReceivedHandler(new ExceptionReceivedEventArgs(e, concreteType));
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

            Console.WriteLine(ex.ToString(), "ERROR handling message: {ExceptionMessage} - Context: {@ExceptionContext}", ex.Message, context.FullName);

            return Task.CompletedTask;
        }


        public void Publish(IMessage message, string serialPort)
        {
            var transport = _transportManager.GetTransportByName(serialPort);
            transport.PushDataAsync(message);
        }

        public void Subscribe<T>(string serialPort) where T : IHandler<IMessage, ICommand>
        {
            _subscriptionManager.AddSubscription<T>(serialPort);
        }

        public void Unsubscribe<T>(string serialPort) where T : IHandler<IMessage, ICommand>
        {
            _subscriptionManager.RemoveSubscription<T>(serialPort);
        }
    }
}
