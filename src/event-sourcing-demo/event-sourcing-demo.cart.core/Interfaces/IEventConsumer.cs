using System.Threading;
using System.Threading.Tasks;

namespace event_sourcing_demo.cart.core.Interfaces
{
    public interface IEventConsumer
    {
        Task ConsumeAsync(CancellationToken stoppingToken);
    }

    public interface IEventConsumer<TA, out TKey> : IEventConsumer where TA : IAggregateRoot<TKey>
    {
        event EventReceivedHandler<TKey> EventReceived;
    }

    public delegate Task EventReceivedHandler<in TKey>(object sender, IDomainEvent<TKey> e);
}