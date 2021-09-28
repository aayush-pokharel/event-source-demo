using System;
using System.Collections.Generic;
using System.Text;

namespace event_sourcing_demo.cart.core.Interfaces
{
    public interface IEventConsumerFactory
    {
        IEventConsumer Build<TA, TKey>() where TA : IAggregateRoot<TKey>;
    }
}
