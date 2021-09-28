using event_sourcing_demo.cart.core.EventBus;
using event_sourcing_demo.cart.domain.Events;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace event_sourcing_demo.web.presistence.azure.functions.EventHandlers
{
    public class CartLineItemsEventsHandler :
        INotificationHandler<EventReceived<CartLineItemCreated>>,
        INotificationHandler<EventReceived<CartLineItemQuantityAdded>>,
        INotificationHandler<EventReceived<CartLineItemQuantityRemoved>>
    {
        public Task Handle(EventReceived<CartLineItemCreated> notification, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task Handle(EventReceived<CartLineItemQuantityAdded> notification, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task Handle(EventReceived<CartLineItemQuantityRemoved> notification, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
