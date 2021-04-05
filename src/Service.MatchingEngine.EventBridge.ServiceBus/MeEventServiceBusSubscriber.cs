using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
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
        private static readonly ActivitySource ActivitySource = new ActivitySource("MyJetWallet");

        private readonly List<Func<IReadOnlyList<OutgoingEvent>, ValueTask>> _list = new();

        public MeEventServiceBusSubscriber(MyServiceBusTcpClient client, string queueName, TopicQueueType queryType, string topic = default)
        {
            if (string.IsNullOrEmpty(topic))
                topic = "spot-me-events";

            client.Subscribe(topic, queueName, queryType, HandleMeEvents);
            
        }

        public void Subscribe(Func<IReadOnlyList<OutgoingEvent>, ValueTask> callback)
        {
            _list.Add(callback);
        }

        private OutgoingEvent Deserializer(ReadOnlyMemory<byte> data)
        {
            return OutgoingEvent.Parser.ParseFrom(data.ToArray());
        }

        private async ValueTask HandleMeEvents(IConfirmationContext ctx, IReadOnlyList<IMyServiceBusMessage> batch)
        {
            IReadOnlyList<OutgoingEvent> itms = batch.Select(e => Deserializer(e.Data)).ToImmutableList();

            using var activity = ActivitySource.StartActivity("Handle events OutgoingEvent")?.AddTag("event-name", "OutgoingEvent")?.AddTag("event-count", itms.Count);

            foreach (var subscriber in _list)
                await subscriber(itms);
            
        }
    }
}