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
    public class MeEventServiceBusSubscriber : ISubscriber<IReadOnlyList<MeEvent>>
    {
        private readonly List<Func<IReadOnlyList<MeEvent>, ValueTask>> _list = new();

        public MeEventServiceBusSubscriber(MyServiceBusTcpClient client, string queueName, bool deleteOnDisconnect, string topic = default)
        {
            if (string.IsNullOrEmpty(topic))
                topic = "spot-me-events";

            client.Subscribe(topic, queueName, deleteOnDisconnect, Subscriber);
        }

        public void Subscribe(Func<IReadOnlyList<MeEvent>, ValueTask> callback)
        {
            _list.Add(callback);
        }

        private MeEvent Deserializer(ReadOnlyMemory<byte> data)
        {
            return MeEvent.Parser.ParseFrom(data.ToArray());
        }

        private async ValueTask Subscriber(IReadOnlyList<IMyServiceBusMessage> batch)
        {
            IReadOnlyList<MeEvent> itms = batch.Select(e => Deserializer(e.Data)).ToImmutableList();

            foreach (var subscriber in _list)
                await subscriber(itms);
            
        }
    }
}