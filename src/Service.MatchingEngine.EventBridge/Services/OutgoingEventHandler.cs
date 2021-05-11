using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotNetCoreDecorators;
using Grpc.Core;
using ME.Contracts.OutgoingMessages;
using Microsoft.Extensions.Logging;
using MyJetWallet.Sdk.Service;

namespace Service.MatchingEngine.EventBridge.Services
{
    public class OutgoingEventHandler : GrpcOutgoingEventsService.GrpcOutgoingEventsServiceBase
    {
        private readonly IPublisher<OutgoingEvent> _publisher;
        private readonly ILogger<OutgoingEventHandler> _logger;
        private long _lastNumber;

        public OutgoingEventHandler(IPublisher<OutgoingEvent> publisher, ILogger<OutgoingEventHandler> logger)
        {
            _publisher = publisher;
            _logger = logger;
        }

        public override async Task<PublishRequestResult> PublishEvents(MessageWrapper request,
            ServerCallContext context)
        {
            using var activity = MyTelemetry.StartActivity("Process ME events");

            _lastNumber.AddToActivityAsTag("start-number");

            request.Events.Count.AddToActivityAsTag("count-events");
            var minSid = request.Events.Min(e => e.Header.SequenceNumber);
            minSid.AddToActivityAsTag("min-sequence-number");
            var maxSid = request.Events.Max(e => e.Header.SequenceNumber);
            maxSid.AddToActivityAsTag("max-sequence-number");

            var list = new List<Task>();
            var number = _lastNumber;

            try
            {
                foreach (var item in request.Events)
                {
                    //if (item.Value.Header.SequenceNumber > number)
                    {
                        list.Add(_publisher.PublishAsync(item).AsTask());
                        number = item.Header.SequenceNumber;
                    }
                }

                await Task.WhenAll(list);

                _lastNumber = number;

                _lastNumber.AddToActivityAsTag("end-number");

                _logger.LogDebug(
                    "Success. Publish messages from ME Count: {count}. LastNumber: {lastNumber}. MinNumber: {minNumber}. MaxNumber: {maxNumber}",
                    request.Events.Count, _lastNumber, minSid, maxSid);
                return new PublishRequestResult()
                {
                    Published = true
                };
            }
            catch (Exception ex)
            {
                ex.FailActivity();
                _logger.LogError(ex,
                    "cannot publish messages from ME Count: {count}. LastNumber: {lastNumber}. MinNumber: {minNumber}. MaxNumber: {maxNumber}",
                    request.Events.Count, _lastNumber, minSid, maxSid);
                await Task.Delay(5000);
                return new PublishRequestResult()
                {
                    Published = false,
                    Reason = ex.Message
                };
            }
        }
    }
}