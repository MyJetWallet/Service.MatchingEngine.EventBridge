using Autofac;
using DotNetCoreDecorators;
using JetBrains.Annotations;
using ME.Contracts.OutgoingMessages;
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
            MyServiceBusTcpClient client, string topic = "spot-me-event")
        {
            builder
                .RegisterInstance(new MeEventServiceBusPublisher(client, topic))
                .As<IPublisher<MeEvent>>()
                .SingleInstance();
        }

        /// <summary>
        /// Register ISubscriber for ClientRegistrationMessage
        /// </summary>
        public static void RegisterMeEventSubscriber(this ContainerBuilder builder,
            MyServiceBusTcpClient client, string queueName, bool deleteOnDisconnect, string topic = "spot-me-event")
        {
            builder
                .RegisterInstance(new MeEventServiceBusSubscriber(client, queueName, deleteOnDisconnect, topic))
                .As<ISubscriber<MeEvent>>()
                .SingleInstance();
        }
    }
}