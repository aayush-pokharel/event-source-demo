using event_sourcing_demo.cart.core.EventBus;
using event_sourcing_demo.cart.domain;
using event_sourcing_demo.cart.domain.Events;
using event_sourcing_demo.presistence.azure;
using event_sourcing_demo.web.core.Queries.Models;
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
    public class CartLineItemsEventsHandler :
        INotificationHandler<EventReceived<CartLineItemCreated>>,
        INotificationHandler<EventReceived<CartLineItemQuantityAdded>>,
        INotificationHandler<EventReceived<CartLineItemQuantityRemoved>>
    {
        private readonly ILogger<CartLineItemsEventsHandler> _logger;
        private readonly Container _cartLineItemContainer;
        private readonly Container _productContainer;

        public CartLineItemsEventsHandler(IDbContainerProvider containerProvider, ILogger<CartLineItemsEventsHandler> logger)
        {
            if (containerProvider == null)
                throw new ArgumentNullException(nameof(containerProvider));

            _logger = logger;
            _cartLineItemContainer = containerProvider.GetContainer("CartLineItemDetails");
            _productContainer = containerProvider.GetContainer("ProductDetails");
        }
        public async Task Handle(EventReceived<CartLineItemCreated> @event, CancellationToken cancellationToken)
        {
            _logger.LogInformation("creating cart line item details for aggregate {AggregateId} ...", @event.Event.AggregateId);

            var productId = @event.Event.ProductId;
            var productResponse = await _productContainer.ReadItemAsync<ProductDetails>(productId.ToString(), 
                new PartitionKey(productId.ToString()), 
                null, cancellationToken);
            var product = productResponse.Resource;
            if(null == product)
            {
                var msg = $"unable to find product by id {@event.Event.ProductId}";
                _logger.LogWarning(msg);
                throw new ArgumentOutOfRangeException(nameof(@event.Event.ProductId), msg);
            }

            var balance = Money.Zero(Currency.USDollar).Add(product.Price.Value * @event.Event.Quantity);
            var newLineItem = new CartLineItemDetails(@event.Event.AggregateId, @event.Event.CartId, @event.Event.ProductId, @event.Event.Quantity, product.Name, balance);
            await _cartLineItemContainer.UpsertItemAsync(newLineItem, new PartitionKey(@event.Event.AggregateId.ToString()),
                null, cancellationToken);

            _logger.LogInformation("created cart line item {AggregateId}", @event.Event.AggregateId);
        }

        public async Task Handle(EventReceived<CartLineItemQuantityAdded> @event, CancellationToken cancellationToken)
        {
            _logger.LogInformation("adding quantity for cart line item details for aggregate {AggregateId} ...", @event.Event.AggregateId);

            var productId = @event.Event.ProductId;
            var productResponse = await _productContainer.ReadItemAsync<ProductDetails>(productId.ToString(),
                new PartitionKey(productId.ToString()),
                null, cancellationToken);
            var lineItemId = @event.Event.AggregateId;
            var lineItemResponse = await _cartLineItemContainer.ReadItemAsync<CartLineItemDetails>(lineItemId.ToString(),
                new PartitionKey(lineItemId.ToString()),
                null, cancellationToken);

            var product = productResponse.Resource;
            var lineItem = lineItemResponse.Resource;
            if (null == product)
            {
                var msg = $"unable to find product by id {@event.Event.ProductId}";
                _logger.LogWarning(msg);
                throw new ArgumentOutOfRangeException(nameof(@event.Event.ProductId), msg);
            }
            if(null == lineItem)
            {
                var msg = $"unable to find lineitem by id {@event.Event.AggregateId}";
                _logger.LogWarning(msg);
                throw new ArgumentOutOfRangeException(nameof(@event.Event.AggregateId), msg);
            }

            var balance = lineItem.TotalAmount.Add(product.Price.Value * @event.Event.Quantity);
            var quantity = lineItem.Quantity + @event.Event.Quantity;
            var updatedLineItem = new CartLineItemDetails(@event.Event.AggregateId, @event.Event.CartId, @event.Event.ProductId, quantity, product.Name, balance);
            await _cartLineItemContainer.ReplaceItemAsync(updatedLineItem, @event.Event.AggregateId.ToString(), 
                new PartitionKey(@event.Event.AggregateId.ToString()),
                null, cancellationToken);

            _logger.LogInformation("added quantity cart line item {AggregateId}", @event.Event.AggregateId);
        }

        public async Task Handle(EventReceived<CartLineItemQuantityRemoved> @event, CancellationToken cancellationToken)
        {
            _logger.LogInformation("removing quantity cart line item details for aggregate {AggregateId} ...", @event.Event.AggregateId);

            var productId = @event.Event.ProductId;
            var productResponse = await _productContainer.ReadItemAsync<ProductDetails>(productId.ToString(),
                new PartitionKey(productId.ToString()),
                null, cancellationToken);
            var lineItemId = @event.Event.AggregateId;
            var lineItemResponse = await _cartLineItemContainer.ReadItemAsync<CartLineItemDetails>(lineItemId.ToString(),
                new PartitionKey(lineItemId.ToString()),
                null, cancellationToken);

            var product = productResponse.Resource;
            var lineItem = lineItemResponse.Resource;
            if (null == product)
            {
                var msg = $"unable to find product by id {@event.Event.ProductId}";
                _logger.LogWarning(msg);
                throw new ArgumentOutOfRangeException(nameof(@event.Event.ProductId), msg);
            }
            if (null == lineItem)
            {
                var msg = $"unable to find lineitem by id {@event.Event.AggregateId}";
                _logger.LogWarning(msg);
                throw new ArgumentOutOfRangeException(nameof(@event.Event.AggregateId), msg);
            }

            var balance = lineItem.TotalAmount.Subtract(product.Price.Value * @event.Event.Quantity);
            var quantity = lineItem.Quantity + @event.Event.Quantity;

            if (quantity < 0)
                throw new ArgumentException("Quanity to be subtracted cannot be greater than the line item quantity.");

            var updatedLineItem = new CartLineItemDetails(@event.Event.AggregateId, @event.Event.CartId, @event.Event.ProductId, quantity, product.Name, balance);
            await _cartLineItemContainer.ReplaceItemAsync(updatedLineItem, @event.Event.AggregateId.ToString(),
                new PartitionKey(@event.Event.AggregateId.ToString()),
                null, cancellationToken);

            _logger.LogInformation("removed quantity cart line item {AggregateId}", @event.Event.AggregateId);
        }
    }
}
