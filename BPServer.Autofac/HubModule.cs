using Autofac;
using BPServer.Core.MessageBus;
using BPServer.Core.Sagas;
using BPServer.Core.Transports;
using System;
using System.Collections.Generic;
using System.Text;

namespace BPServer.Autofac
{
    public class HubModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<TransportManager>().As<ITransportManager>().SingleInstance();
            builder.RegisterType<InMemoryMessageBusSubscriptionsManager>().As<IMessageBusSubscriptionManager>().SingleInstance();
            builder.RegisterType<MessageBus>().As<IMessageBus>().SingleInstance();
            builder.RegisterType<SagasManager>().As<ISagasManager>().SingleInstance();
        }
    }
}
