using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotNetCoreDecorators;
using ME.Contracts.OutgoingMessages;
using Microsoft.Extensions.Logging;
using MyJetWallet.MatchingEngine.EventReader;
using MyJetWallet.MatchingEngine.EventReader.BaseReader;
using MyJetWallet.Sdk.Service;
using Newtonsoft.Json;

namespace Service.MatchingEngine.EventBridge.Services
{
    public class OutgoingEventHandler : IMatchingEngineSubscriber<OutgoingEvent>
    {
        private readonly IPublisher<OutgoingEvent> _publisher;
        private readonly ILogger<OutgoingEventHandler> _logger;
        private long _lastNumber;

        public OutgoingEventHandler(IPublisher<OutgoingEvent> publisher, ILogger<OutgoingEventHandler> logger)
        {
            _publisher = publisher;
            _logger = logger;
        }

        public async Task Process(IList<CustomQueueItem<OutgoingEvent>> batch)
        {
            using var activity = MyTelemetry.StartActivity("Process ME events");

            _lastNumber.AddToActivityAsTag("start-number");

            batch.Count.AddToActivityAsTag("count-events");
            batch.Min(e => e.Value.Header.SequenceNumber).AddToActivityAsTag("min-sequence-number");
            batch.Max(e => e.Value.Header.SequenceNumber).AddToActivityAsTag("max-sequence-number");

            var list = new List<Task>();
            var number = _lastNumber;

            try
            {
                foreach (var item in batch)
                {
                    if (item.Value.Header.SequenceNumber > number)
                    {
                        list.Add(_publisher.PublishAsync(item.Value).AsTask());
                        number = item.Value.Header.SequenceNumber;
                    }
                }

                await Task.WhenAll(list);

                _lastNumber = number;

                _lastNumber.AddToActivityAsTag("end-number");
            }
            catch (Exception ex)
            {
                ex.FailActivity();
                _logger.LogError(ex, "cannot publish messages from ME Count: {count}. LastNumber: {lastNumber}", batch.Count, _lastNumber);
                await Task.Delay(5000);
                throw;
            }
        }
    }
}