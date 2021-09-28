using event_sourcing_demo.cart.core.Interfaces;
using System.Threading.Tasks;

namespace event_sourcing_demo.cart.core.Interfaces
{
    public interface IEventProducer<in TA, in TKey>
        where TA : IAggregateRoot<TKey>
    {
        Task DispatchAsync(TA aggregateRoot);
    }
}