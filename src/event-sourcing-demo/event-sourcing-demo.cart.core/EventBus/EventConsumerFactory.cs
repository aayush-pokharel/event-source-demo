﻿using event_sourcing_demo.cart.core.Interfaces;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Threading;
using System.Threading.Tasks;

namespace event_sourcing_demo.cart.core.EventBus
{
    public class EventConsumerFactory : IEventConsumerFactory
    {
        private readonly IServiceScopeFactory scopeFactory;

        public EventConsumerFactory(IServiceScopeFactory scopeFactory)
        {
            this.scopeFactory = scopeFactory;
        }

        public IEventConsumer Build<TA, TKey>() where TA : IAggregateRoot<TKey>
        {
            using var scope = scopeFactory.CreateScope();
            var consumer = scope.ServiceProvider.GetRequiredService<IEventConsumer<TA, TKey>>();

            async Task onEventReceived(object s, IDomainEvent<TKey> e)
            {
                var @event = EventReceivedFactory.Create((dynamic)e);

                using var innerScope = scopeFactory.CreateScope();
                var mediator = innerScope.ServiceProvider.GetRequiredService<IMediator>();
                await mediator.Publish(@event, CancellationToken.None);
            }
            consumer.EventReceived += onEventReceived;

            return consumer;
        }
    }
}
