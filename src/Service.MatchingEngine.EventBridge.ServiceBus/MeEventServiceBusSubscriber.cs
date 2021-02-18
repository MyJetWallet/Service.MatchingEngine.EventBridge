using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DotNetCoreDecorators;
using JetBrains.Annotations;
using ME.Contracts.OutgoingMessages;
using MyServiceBus.TcpClient;

namespace Service.MatchingEngine.EventBridge.ServiceBus
{
    [UsedImplicitly]
    public class MeEventServiceBusSubscriber : ISubscriber<MeEvent>
    {
        private readonly List<Func<MeEvent, ValueTask>> _list = new List<Func<MeEvent, ValueTask>>();

        public MeEventServiceBusSubscriber(MyServiceBusTcpClient client, string queueName, bool deleteOnDisconnect, string topic = "spot-me-event")
        {
            client.Subscribe(topic, queueName, deleteOnDisconnect,
                async bytes =>
                {
                    var itm = Deserializer(bytes.Data);
                    foreach (var subscribers in _list)
                        await subscribers(itm);
                });
        }

        public void Subscribe(Func<MeEvent, ValueTask> callback)
        {
            _list.Add(callback);
        }

        private MeEvent Deserializer(ReadOnlyMemory<byte> data)
        {
            return MeEvent.Parser.ParseFrom(data.ToArray());
        }
    }
}