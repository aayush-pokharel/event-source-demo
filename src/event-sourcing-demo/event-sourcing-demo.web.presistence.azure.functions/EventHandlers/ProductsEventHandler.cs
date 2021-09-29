using event_sourcing_demo.cart.core.EventBus;
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

        public async Task Handle(EventReceived<CartLineItemCreated> @event, CancellationToken cancellationToken)
        {
            _logger.LogInformation("updating product stock for aggregate {AggregateId} ...", @event.Event.AggregateId);

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
            var stock = product.Stock - @event.Event.Quantity;

            if (stock < 0)
                throw new ArgumentException("Stock to be subtracted cannot be greater than the product stock.");

            var updatedProduct = new ProductDetails(@event.Event.ProductId, product.Name, stock, product.Price);
            await _productContainer.ReplaceItemAsync(updatedProduct, @event.Event.ProductId.ToString(),
                new PartitionKey(@event.Event.AggregateId.ToString()),
                null, cancellationToken);

            _logger.LogInformation("updated product {AggregateId}", @event.Event.AggregateId);
        }

        public async Task Handle(EventReceived<CartLineItemQuantityAdded> @event, CancellationToken cancellationToken)
        {
            _logger.LogInformation("updating product stock for aggregate {AggregateId} ...", @event.Event.AggregateId);

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

            var stock = product.Stock - @event.Event.Quantity;

            if (stock < 0)
                throw new ArgumentException("Stock to be subtracted cannot be greater than the product stock.");

            var updatedProduct = new ProductDetails(@event.Event.ProductId, product.Name, stock, product.Price);
            await _productContainer.ReplaceItemAsync(updatedProduct, @event.Event.ProductId.ToString(),
                new PartitionKey(@event.Event.AggregateId.ToString()),
                null, cancellationToken);

            _logger.LogInformation("updated product {AggregateId}", @event.Event.AggregateId);
        }

        public async Task Handle(EventReceived<CartLineItemQuantityRemoved> @event, CancellationToken cancellationToken)
        {
            _logger.LogInformation("updating product stock for aggregate {AggregateId} ...", @event.Event.AggregateId);

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

            var stock = product.Stock + @event.Event.Quantity;

            var updatedProduct = new ProductDetails(@event.Event.ProductId, product.Name, stock, product.Price);
            await _productContainer.ReplaceItemAsync(updatedProduct, @event.Event.ProductId.ToString(),
                new PartitionKey(@event.Event.AggregateId.ToString()),
                null, cancellationToken);

            _logger.LogInformation("updated product {AggregateId}", @event.Event.AggregateId);
        }

        public async Task Handle(EventReceived<ProductCreated> @event, CancellationToken cancellationToken)
        {
            _logger.LogInformation("creating product for aggregate {AggregateId} ...", @event.Event.AggregateId);

            var productId = @event.Event.AggregateId;

            var product = new ProductDetails(@event.Event.AggregateId, @event.Event.Name, @event.Event.Stock, @event.Event.Price);
            await _productContainer.UpsertItemAsync(product,
                new PartitionKey(@event.Event.AggregateId.ToString()),
                null, cancellationToken);

            _logger.LogInformation("created product {AggregateId}", @event.Event.AggregateId);
        }

        public Task Handle(EventReceived<ProductBought> notification, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
