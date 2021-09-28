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
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace event_sourcing_demo.web.presistence.azure.functions.EventHandlers
{
    public class CartEventsHandler :
        INotificationHandler<EventReceived<CartCreated>>,
        INotificationHandler<EventReceived<CartLineItemCreated>>,
        INotificationHandler<EventReceived<CartLineItemQuantityAdded>>,
        INotificationHandler<EventReceived<CartLineItemQuantityRemoved>>
    {
        private readonly ILogger<CartEventsHandler> _logger;
        private readonly Container _cartContainer;
        private readonly Container _productContainer;

        public CartEventsHandler(IDbContainerProvider containerProvider, ILogger<CartEventsHandler> logger)
        {
            if (containerProvider == null)
                throw new ArgumentNullException(nameof(containerProvider));

            _logger = logger;

            _cartContainer = containerProvider.GetContainer("CartDetails");
            _productContainer = containerProvider.GetContainer("ProductDetails");
        }
        public async Task Handle(EventReceived<CartCreated> @event, CancellationToken cancellationToken)
        {
            _logger.LogInformation("creating cart details for {AggregateId} ...", @event.Event.AggregateId);

            var partitionKey = new PartitionKey(@event.Event.AggregateId.ToString());

            var cart = new CartDetails(@event.Event.AggregateId, @event.Event.ShopName, null, Money.Zero(Currency.USDollar));

            var response = await _cartContainer.UpsertItemAsync(cart, partitionKey, cancellationToken: cancellationToken);
            if (response.StatusCode != HttpStatusCode.Created)
            {
                var msg = $"an error has occurred while processing an event: {response.Diagnostics}";
                throw new Exception(msg);
            }

            _logger.LogInformation("created cart details {AggregateId}", @event.Event.AggregateId);
        }

        public async Task Handle(EventReceived<CartLineItemCreated> @event, CancellationToken cancellationToken)
        {
            var partitionKey = new PartitionKey(@event.Event.CartId.ToString());

            var response = await _cartContainer.ReadItemAsync<CartDetails>(@event.Event.CartId.ToString(),
                partitionKey,
                null, cancellationToken);

            var cart = response.Resource;
            //get product associated with the product
            var product = await _productContainer.ReadItemAsync<ProductDetails>(@event.Event.ProductId.ToString(),
                partitionKey,
                null,
                cancellationToken);
            //get existing line items
            var lineItems = (cart.CartLineItems ?? Enumerable.Empty<Guid>()).ToList();

            lineItems.Add(@event.Event.AggregateId);

            //get current balance
            var balance = cart.TotalAmount ?? Money.Zero(Currency.USDollar);
            var newBalance = balance.Add(@event.Event.Quantity * product.Resource.Price.Value);

            var updatedCart = new CartDetails(cart.Id, cart.ShopName, lineItems, newBalance);
            await _cartContainer.ReplaceItemAsync(updatedCart, @event.Event.CartId.ToString(), partitionKey, null, cancellationToken);

            _logger.LogInformation($"updated cart detail line items {@event.Event.AggregateId}");
        }

        public async Task Handle(EventReceived<CartLineItemQuantityAdded> @event, CancellationToken cancellationToken)
        {
            var partitionKey = new PartitionKey(@event.Event.CartId.ToString());

            var response = await _cartContainer.ReadItemAsync<CartDetails>(@event.Event.CartId.ToString(),
                partitionKey,
                null, cancellationToken);

            var cart = response.Resource;

            //get product associated with the product
            var product = await _productContainer.ReadItemAsync<ProductDetails>(@event.Event.ProductId.ToString(),
                partitionKey,
                null,
                cancellationToken);
            //get current balance
            var balance = cart.TotalAmount ?? Money.Zero(Currency.USDollar);

            var newBalance = balance.Add(@event.Event.Quantity * product.Resource.Price.Value);

            var updatedCart = new CartDetails(cart.Id, cart.ShopName, cart.CartLineItems, newBalance);

            await _cartContainer.ReplaceItemAsync(updatedCart, @event.Event.CartId.ToString(), partitionKey, null, cancellationToken);
        }

        public async Task Handle(EventReceived<CartLineItemQuantityRemoved> @event, CancellationToken cancellationToken)
        {
            var partitionKey = new PartitionKey(@event.Event.CartId.ToString());

            var response = await _cartContainer.ReadItemAsync<CartDetails>(@event.Event.CartId.ToString(),
                partitionKey,
                null, cancellationToken);

            var cart = response.Resource;

            //get product associated with the product
            var product = await _productContainer.ReadItemAsync<ProductDetails>(@event.Event.ProductId.ToString(),
                partitionKey,
                null,
                cancellationToken);
            //get current balance
            var balance = cart.TotalAmount ?? Money.Zero(Currency.USDollar);

            var newBalance = balance.Subtract(@event.Event.Quantity * product.Resource.Price.Value);

            var updatedCart = new CartDetails(cart.Id, cart.ShopName, cart.CartLineItems, newBalance);

            await _cartContainer.ReplaceItemAsync(updatedCart, @event.Event.CartId.ToString(), partitionKey, null, cancellationToken);
        }
    }
}
