using event_sourcing_demo.cart.core.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace event_sourcing_demo.cart.domain.Commands
{
    public class CreateCartLineItem : INotification
    {
        public CreateCartLineItem(Guid lineItemId, Guid cartId, Guid productId, int quantity)
        {
            LineItemId = lineItemId;
            CartId = cartId;
            ProductId = productId;
            Quantity = quantity;
        }
        public Guid LineItemId { get; }
        public Guid CartId { get; }
        public Guid ProductId { get; }
        public int Quantity { get; }
    }
    public class CreateCartLineItemHandler : INotificationHandler<CreateCartLineItem>
    {
        private readonly IEventsService<CartLineItem, Guid> _lineItemEventsService;
        private readonly IEventsService<Product, Guid> _productService;
        private readonly IEventsService<Cart, Guid> _cartService;
        public CreateCartLineItemHandler(IEventsService<CartLineItem, Guid> lineItemEventsService,
            IEventsService<Product, Guid> productService,
            IEventsService<Cart, Guid> cartService)
        {
            _lineItemEventsService = lineItemEventsService;
            _productService = productService;
            _cartService = cartService;
        }
        public async Task Handle(CreateCartLineItem command, CancellationToken cancellationToken)
        {
            var cart = await _cartService.RehydrateAsync(command.CartId);
            var product = await _productService.RehydrateAsync(command.ProductId);

            if (cart == null)
                throw new ArgumentOutOfRangeException(nameof(command.CartId), ": invalid Cart Id");
            if (product == null)
                throw new ArgumentOutOfRangeException(nameof(command.ProductId), ": invalid Product Id");

            var lineItem = new CartLineItem(command.LineItemId, product, cart, command.Quantity);
            await _lineItemEventsService.PersistAsync(lineItem);
        }
    }
}
