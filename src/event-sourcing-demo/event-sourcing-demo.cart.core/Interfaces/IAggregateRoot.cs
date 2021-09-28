using System;
using System.Collections.Generic;
using System.Text;

namespace event_sourcing_demo.cart.core.Interfaces
{
    public interface IAggregateRoot<out TKey> : IEntity<TKey>
    {
        long Version { get; }
        IReadOnlyCollection<IDomainEvent<TKey>> Events { get; }
        void ClearEvents();
    }
}
