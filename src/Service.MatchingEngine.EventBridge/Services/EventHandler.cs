using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DotNetCoreDecorators;
using ME.Contracts.OutgoingMessages;
using Microsoft.Extensions.Logging;
using MyJetWallet.MatchingEngine.EventReader;
using MyJetWallet.MatchingEngine.EventReader.BaseReader;
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
            foreach (var item in batch)
            {
                if (item.Value.Header.SequenceNumber > _lastNumber)
                {
                    try
                    {
                        await _publisher.PublishAsync(item.Value);
                        _lastNumber = item.Value.Header.SequenceNumber;
                    }
                    catch(Exception ex)
                    {
                        _logger.LogError(ex, "cannot publish message: {OutgoingEventJson}", JsonConvert.SerializeObject(item.Value));
                        await Task.Delay(5000);
                    }
                }
            }
        }
    }
}