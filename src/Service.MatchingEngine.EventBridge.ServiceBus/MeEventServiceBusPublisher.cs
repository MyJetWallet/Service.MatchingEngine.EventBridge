﻿using System.Threading.Tasks;
using DotNetCoreDecorators;
using Google.Protobuf;
using JetBrains.Annotations;
using ME.Contracts.OutgoingMessages;
using MyServiceBus.TcpClient;

namespace Service.MatchingEngine.EventBridge.ServiceBus
{
    [UsedImplicitly]
    public class MeEventServiceBusPublisher : IPublisher<MeEvent>
    {
        private readonly MyServiceBusTcpClient _client;
        private readonly string _topic;

        public MeEventServiceBusPublisher(MyServiceBusTcpClient client, string topic = "spot-me-event")
        {
            _client = client;
            _topic = topic;
            _client.CreateTopicIfNotExists(_topic, 100000);
        }

        public async ValueTask PublishAsync(MeEvent valueToPublish)
        {
            var bytesToSend = valueToPublish.ToByteArray();
            await _client.PublishAsync(_topic, bytesToSend, true);
        }
    }
}