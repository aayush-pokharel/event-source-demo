﻿using event_sourcing_demo.cart.core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace event_sourcing_demo.cart.core.Events
{
    public class EventsService<TA, TKey> : IEventsService<TA, TKey> where TA : class, IAggregateRoot<TKey>
    {
        private readonly IEventsRepository<TA, TKey> _eventsRepository;
        private readonly IEventProducer<TA, TKey> _eventProducer;

        public EventsService(IEventsRepository<TA, TKey> eventsRepository, IEventProducer<TA, TKey> eventProducer)
        {
            _eventsRepository = eventsRepository ?? throw new ArgumentNullException(nameof(eventsRepository));
            _eventProducer = eventProducer ?? throw new ArgumentNullException(nameof(eventProducer));
        }

        public async Task PersistAsync(TA aggregateRoot)
        {
            if (null == aggregateRoot)
                throw new ArgumentNullException(nameof(aggregateRoot));

            if (!aggregateRoot.Events.Any())
                return;

            await _eventsRepository.AppendAsync(aggregateRoot);
            await _eventProducer.DispatchAsync(aggregateRoot);

            aggregateRoot.ClearEvents();
        }

        public Task<TA> RehydrateAsync(TKey key)
        {
            return _eventsRepository.RehydrateAsync(key);
        }
    }
}
