using System;
using System.Collections.Generic;
using System.Text;

namespace event_sourcing_demo.cart.core.Interfaces
{
    public interface IDomainEvent<out TKey>
    {
        long AggregateVersion { get; }
        TKey AggregateId { get; }
        DateTime Timestamp { get; }
    }
}
