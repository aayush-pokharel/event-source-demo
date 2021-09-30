using event_sourcing_demo.cart.core.Interfaces;
using event_sourcing_demo.cart.domain;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace event_sourcing_demo.web.api.Workers
{
    public class EventsConsumerWorker : BackgroundService
    {
        private readonly IEventConsumerFactory _eventConsumerFactory;

        public EventsConsumerWorker(IEventConsumerFactory eventConsumerFactory)
        {
            _eventConsumerFactory = eventConsumerFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            IEnumerable<IEventConsumer> consumers = new[]
            {
                _eventConsumerFactory.Build<Cart, Guid>(),
                _eventConsumerFactory.Build<CartLineItem, Guid>(),
                _eventConsumerFactory.Build<Product, Guid>()
            };
            var tc = Task.WhenAll(consumers.Select(c => c.ConsumeAsync(stoppingToken)));
            await tc;
        }
    }
}
