using event_sourcing_demo.cart.core.Interfaces;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace event_sourcing_demo.presistence.azure
{
    public class EventProducer<TA, TKey> : IEventProducer<TA, TKey>
        where TA : IAggregateRoot<TKey>
    {
        private readonly ILogger<EventProducer<TA, TKey>> _logger;

        private readonly ITopicClient _topicClient;

        private readonly IEventSerializer _eventSerializer;

        public EventProducer(ITopicClientFactory topicFactory, string topicBaseName, IEventSerializer eventSerializer, ILogger<EventProducer<TA, TKey>> logger)
        {
            if (topicFactory == null)
                throw new ArgumentNullException(nameof(topicFactory));
            if (string.IsNullOrWhiteSpace(topicBaseName))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(topicBaseName));

            _logger = logger;
            _eventSerializer = eventSerializer;
            var aggregateType = typeof(TA);

            this.TopicName = $"{topicBaseName}-{aggregateType.Name}";
            _topicClient = topicFactory.Build(this.TopicName);
        }

        public async Task DispatchAsync(TA aggregateRoot)
        {
            if (null == aggregateRoot)
                throw new ArgumentNullException(nameof(aggregateRoot));

            if (!aggregateRoot.Events.Any())
                return;

            _logger.LogInformation("publishing " + aggregateRoot.Events.Count + " events for {AggregateId} ...", aggregateRoot.Id);

            var messages = aggregateRoot.Events.Select(@event =>
            {
                var eventType = @event.GetType();

                var serialized = _eventSerializer.Serialize(@event);

                var message = new Message(serialized)
                {
                    CorrelationId = aggregateRoot.Id.ToString(),
                    UserProperties =
                    {
                        {"aggregate", aggregateRoot.Id.ToString()},
                        {"type", eventType.AssemblyQualifiedName}
                    }
                };
                return message;
            }).ToList();

            await _topicClient.SendAsync(messages);
        }

        public string TopicName { get; }
    }
}
