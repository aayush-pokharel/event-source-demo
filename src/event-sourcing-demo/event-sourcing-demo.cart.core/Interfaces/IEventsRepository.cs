using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace event_sourcing_demo.cart.core.Interfaces
{
    public interface IEventsRepository<TA, TKey>
        where TA : class, IAggregateRoot<TKey>
    {
        Task AppendAsync(TA aggregateRoot);
        Task<TA> RehydrateAsync(TKey key);
    }
}
