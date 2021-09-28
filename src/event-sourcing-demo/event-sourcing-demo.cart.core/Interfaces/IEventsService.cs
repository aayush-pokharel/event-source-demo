using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace event_sourcing_demo.cart.core.Interfaces
{
    public interface IEventsService<TA, TKey>
        where TA : class, IAggregateRoot<TKey>
    {
        Task PersistAsync(TA aggregateRoot);
        Task<TA> RehydrateAsync(TKey key);
    }
}
