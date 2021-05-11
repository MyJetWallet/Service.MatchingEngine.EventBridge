using Autofac;
using ME.Contracts.OutgoingMessages;
using MyJetWallet.Sdk.Service;
using MyServiceBus.TcpClient;
using Service.MatchingEngine.EventBridge.ServiceBus;
using Service.MatchingEngine.EventBridge.Services;

namespace Service.MatchingEngine.EventBridge.Modules
{
    public class ServiceModule: Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var serviceBusClient = new MyServiceBusTcpClient(Program.ReloadedSettings(model => model.ServiceBusHostPort), ApplicationEnvironment.HostName);

            builder.RegisterInstance(serviceBusClient).AsSelf().SingleInstance();

            builder.RegisterMeEventPublisher(serviceBusClient, Program.Settings.TopicName);

            builder
                .RegisterType<OutgoingEventHandler>()
                .As<OutgoingEventsService.OutgoingEventsServiceBase>()
                .SingleInstance();
        }
    }
}