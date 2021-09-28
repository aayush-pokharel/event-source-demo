using event_sourcing_demo.cart.core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace event_sourcing_demo.cart.core.Models
{
    public abstract class BaseDomainEvent<TA, TKey> : IDomainEvent<TKey>
        where TA : IAggregateRoot<TKey>
    {
        /// <summary>
        /// for deserialization
        /// </summary>
        protected BaseDomainEvent() { }

        /// <summary>
        /// TODO: Must be a better way since every subclass has to call this
        /// </summary>
        /// <param name="aggregateRoot"></param>
        protected BaseDomainEvent(TA aggregateRoot)
        {
            if (aggregateRoot is null)
                throw new ArgumentNullException(nameof(aggregateRoot));

            this.AggregateVersion = aggregateRoot.Version;
            this.AggregateId = aggregateRoot.Id;
            this.Timestamp = DateTime.UtcNow;
        }

        public long AggregateVersion { get; private set; }
        public TKey AggregateId { get; private set; }
        public DateTime Timestamp { get; private set; }
    }
}
