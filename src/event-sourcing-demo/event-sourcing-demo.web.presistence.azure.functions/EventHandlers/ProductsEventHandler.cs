using event_sourcing_demo.cart.core.EventBus;
using event_sourcing_demo.cart.domain.Events;
using event_sourcing_demo.presistence.azure;
using MediatR;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace event_sourcing_demo.web.presistence.azure.functions.EventHandlers
{
    public class ProductsEventHandler :
        INotificationHandler<EventReceived<CartLineItemCreated>>,
        INotificationHandler<EventReceived<CartLineItemQuantityAdded>>,
        INotificationHandler<EventReceived<CartLineItemQuantityRemoved>>,
        INotificationHandler<EventReceived<ProductCreated>>,
        INotificationHandler<EventReceived<ProductBought>>
    {
        private readonly ILogger<ProductsEventHandler> _logger;
        private readonly Container _cartLineItemContainer;
        private readonly Container _productContainer;
        public ProductsEventHandler(IDbContainerProvider containerProvider, 
            ILogger<ProductsEventHandler> logger)
        {
            if (containerProvider == null)
                throw new ArgumentNullException(nameof(containerProvider));

            _logger = logger;
            _cartLineItemContainer = containerProvider.GetContainer("CartLineItemDetails");
            _productContainer = containerProvider.GetContainer("ProductDetails");
        }

        public Task Handle(EventReceived<CartLineItemCreated> @event, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task Handle(EventReceived<CartLineItemQuantityAdded> @event, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task Handle(EventReceived<CartLineItemQuantityRemoved> @event, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task Handle(EventReceived<ProductCreated> @event, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task Handle(EventReceived<ProductBought> @event, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
