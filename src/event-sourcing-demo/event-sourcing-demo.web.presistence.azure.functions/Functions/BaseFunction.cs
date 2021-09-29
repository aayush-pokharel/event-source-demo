using event_sourcing_demo.cart.core.EventBus;
using event_sourcing_demo.cart.core.Interfaces;
using MediatR;
using Microsoft.Azure.ServiceBus;
using System;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace event_sourcing_demo.web.presistence.azure.functions.Functions
{
    public abstract class BaseFunction
    {
        private readonly IEventSerializer _eventSerializer;
        private readonly IMediator _mediator;

        protected BaseFunction(IEventSerializer eventSerializer, IMediator mediator)
        {
            _eventSerializer = eventSerializer ?? throw new ArgumentNullException(nameof(eventSerializer));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        protected async Task HandleMessage(Message msg)
        {
            var eventType = msg.UserProperties["type"] as string;
            var domainEvent = _eventSerializer.Deserialize<Guid>(eventType, msg.Body);
            if (null == domainEvent)
                throw new SerializationException($"unable to deserialize event {eventType} : {msg.Body}");

            var @event = EventReceivedFactory.Create((dynamic)domainEvent);
            await _mediator.Publish(@event, CancellationToken.None);
        }
    }
}