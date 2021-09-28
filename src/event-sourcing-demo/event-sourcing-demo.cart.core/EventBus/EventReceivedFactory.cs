using System;
using System.Collections.Generic;
using System.Text;

namespace event_sourcing_demo.cart.core.EventBus
{
    public static class EventReceivedFactory
    {
        public static EventReceived<TE> Create<TE>(TE @event)
        {
            return new EventReceived<TE>(@event);
        }
    }
}
