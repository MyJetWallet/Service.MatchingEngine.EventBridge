using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using DotNetCoreDecorators;
using JetBrains.Annotations;
using ME.Contracts.OutgoingMessages;
using MyServiceBus.Abstractions;
using MyServiceBus.TcpClient;

namespace Service.MatchingEngine.EventBridge.ServiceBus
{
    [UsedImplicitly]
    public class MeEventServiceBusSubscriber : ISubscriber<IReadOnlyList<OutgoingEvent>>
    {
        private readonly List<Func<IReadOnlyList<OutgoingEvent>, ValueTask>> _list = new();

        public MeEventServiceBusSubscriber(MyServiceBusTcpClient client, string queueName, TopicQueueType queryType, string topic = default)
        {
            if (string.IsNullOrEmpty(topic))
                topic = "spot-me-events";

            client.Subscribe(topic, queueName, queryType, Subscriber);
            
        }

        public void Subscribe(Func<IReadOnlyList<OutgoingEvent>, ValueTask> callback)
        {
            _list.Add(callback);
        }

        private OutgoingEvent Deserializer(ReadOnlyMemory<byte> data)
        {
            return OutgoingEvent.Parser.ParseFrom(data.ToArray());
        }

        private async ValueTask Subscriber(IConfirmationContext ctx, IReadOnlyList<IMyServiceBusMessage> batch)
        {
            //Console.WriteLine($"Receive: {batch.Count}");

            IReadOnlyList<OutgoingEvent> itms = batch.Select(e => Deserializer(e.Data)).ToImmutableList();

            foreach (var subscriber in _list)
                await subscriber(itms);
            
        }
    }
}