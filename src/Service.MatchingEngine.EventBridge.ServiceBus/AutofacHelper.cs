using System.Collections.Generic;
using Autofac;
using DotNetCoreDecorators;
using JetBrains.Annotations;
using ME.Contracts.OutgoingMessages;
using MyServiceBus.Abstractions;
using MyServiceBus.TcpClient;

namespace Service.MatchingEngine.EventBridge.ServiceBus
{
    [UsedImplicitly]
    public static class AutofacHelper
    {
        /// <summary>
        /// Register IPublisher for ClientRegistrationMessage
        /// </summary>
        public static void RegisterMeEventPublisher(this ContainerBuilder builder,
            MyServiceBusTcpClient client, string topic = default)
        {
            builder
                .RegisterInstance(new MeEventServiceBusPublisher(client, topic))
                .As<IPublisher<OutgoingEvent>>()
                .SingleInstance();
        }

        /// <summary>
        /// Register ISubscriber for ClientRegistrationMessage
        /// </summary>
        public static void RegisterMeEventSubscriber(this ContainerBuilder builder,
            MyServiceBusTcpClient client, string queueName, TopicQueueType queryType, string topic = default)
        {
            builder
                .RegisterInstance(new MeEventServiceBusSubscriber(client, queueName, queryType, topic))
                .As<ISubscriber<IReadOnlyList<OutgoingEvent>>>()
                .SingleInstance();
        }
    }
}