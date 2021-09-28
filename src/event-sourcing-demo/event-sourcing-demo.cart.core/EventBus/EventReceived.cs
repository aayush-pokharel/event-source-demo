using MediatR;

namespace event_sourcing_demo.cart.core.EventBus
{
    public class EventReceived<TE> : INotification
    {
        public EventReceived(TE @event)
        {
            Event = @event;
        }

        public TE Event { get; }
    }
}
